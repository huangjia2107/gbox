using Configure;
using Configure.Models;
using IntroductionModule.ViewModels;
using MessageModule.AddGameMsg;
using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XWT = Xceed.Wpf.Toolkit;

namespace IntroductionModule.Views
{
    /// <summary>
    /// DownView.xaml 的交互逻辑
    /// </summary> 
    public partial class DownView : Window
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        static string _local_games_path = currentPath + "Games";


        /// <summary>
        /// 窗体ViewModel
        /// </summary>
        DownViewModel downViewModel;

        /// <summary>
        /// 获取服务器端参数
        /// </summary>
        ConfigModel configModel;

        IEventAggregator addgame_Aggregator;

        #endregion

        #region 构造函数

        public DownView(DownViewModel _downViewModel, IEventAggregator addgame_Aggregator)
        {
            InitializeComponent();

            this.DataContext = _downViewModel;
            this.addgame_Aggregator = addgame_Aggregator;

            configModel = ConfigAccess.GetConfig();
            downViewModel = _downViewModel;

            DeleteTemp();
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="gameid">游戏ID</param>
        /// <param name="listViewModel">下载ViewModel</param>
        public void BeginDown(out bool IsCanExcute, string gameid, ListViewModel listViewModel)
        {
            if (downViewModel.AllDowns.Count == 3)
            {
                XWT.MessageBox.Show("抱歉,最多同时添加三个任务.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCanExcute = false;
                return;
            }

            foreach (ListViewModel list in downViewModel.AllDowns)
            {
                if (list.ID == gameid)
                {
                    XWT.MessageBox.Show("该游戏已经在下载列表中.", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsCanExcute = false;
                    return;
                }
            }

            IsCanExcute = true;

            downViewModel.AllDowns.Add(listViewModel);

            StateObject stateObject = new StateObject();
            stateObject.gameID = gameid;
            stateObject.listViewModel = listViewModel;

            listViewModel.DownName = "连接中.";
            ConnectToServer(configModel.DownIP, configModel.DownPort, stateObject);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 删除缓存文件
        /// </summary>
        private void DeleteTemp()
        {
            string[] filepath = Directory.GetFiles(_local_games_path);
            foreach (string path in filepath)
            {
                if (path.IndexOf(".temp") > -1)
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// 重写OnClosing方法
        /// </summary>
        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    e.Cancel = true;
        //    this.Hide();
        //} 

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void ConnectToServer(string ip, string port, StateObject stateObject)
        {
            stateObject.newClient = new TcpClient();
            stateObject.newClient.BeginConnect(ip, int.Parse(port), new AsyncCallback(ConnCallBackF), stateObject); //异步连接          
        }

        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="connectParam">连接参数</param>
        private delegate void ReceiveFileDelegate(ConnectParam connectParam);

        /// <summary>
        /// 连接服务器异步回调
        /// </summary>
        private void ConnCallBackF(IAsyncResult ar)
        {
            StateObject stateObject = (StateObject)ar.AsyncState;
            try
            {
                stateObject.newClient.EndConnect(ar);
                stateObject.listViewModel.DownName = "连接成功.";

                ConnectParam connectParam = new ConnectParam(stateObject.newClient);
                connectParam.bw.Write(stateObject.gameID); //发送游戏ID
                connectParam.stateObject = stateObject;

                downViewModel.connectList.Add(connectParam);

                stateObject.listViewModel.DownName = "准备接收.";

                ReceiveFileDelegate d = new ReceiveFileDelegate(ReceiveData);
                d.BeginInvoke(connectParam, new AsyncCallback(CallBackF), null);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    XWT.MessageBox.Show("未能与服务器取得连接！错误：" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    downViewModel.AllDowns.Remove(stateObject.listViewModel);
                    this.Close();

                }), System.Windows.Threading.DispatcherPriority.Normal);

            }
        }

        /// <summary>
        /// 发送文件异步回调
        /// </summary>
        private void CallBackF(IAsyncResult arr)
        {
            AsyncResult ar = (AsyncResult)arr;
            ReceiveFileDelegate d = (ReceiveFileDelegate)ar.AsyncDelegate;

            try
            {
                d.EndInvoke(arr);
            }
            catch { }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="connectParam">连接参数</param>
        private void ReceiveData(ConnectParam connectParam)
        {
            try
            {
                connectParam.stateObject.listViewModel.DownName = connectParam.br.ReadString(); //获取文件名 

                long downSize = connectParam.br.ReadInt64();  //获取文件大小 
                connectParam.stateObject.listViewModel.DownSize = ChangeToSize(downSize); //转换为MB 

                string gametime = connectParam.br.ReadString(); //获取版本时间

                using (connectParam.fs = new FileStream(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName + ".temp", FileMode.Create))
                {
                    byte[] buffer = new byte[connectParam.bufferSize];

                    connectParam.StartTimer(); //开启计时器

                    while (connectParam.readL < downSize)
                    {
                        connectParam.size = connectParam.br.Read(buffer, 0, connectParam.bufferSize);
                        connectParam.fs.Write(buffer, 0, connectParam.size);
                        connectParam.readL += connectParam.size;

                        if (connectParam.sec != 0)
                        {
                            long downSpeed = connectParam.readL / connectParam.sec;

                            connectParam.stateObject.listViewModel.DownSpeed = ChangeToSize(downSpeed) + "/S"; //速度 
                        }
                        connectParam.stateObject.listViewModel.DownProgress = String.Format("{0:N1}", (double)connectParam.readL * 100 / downSize);
                    }
                }

                if (File.Exists(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName))
                {
                    try
                    {
                        File.Delete(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName + ".temp");
                    }
                    catch { }
                }
                else
                {
                    File.Move(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName + ".temp",
                        connectParam.gamePath + connectParam.stateObject.listViewModel.DownName);
                }

                //将游戏ID发到“我的游戏模块”
                addgame_Aggregator.GetEvent<AddGameMsgEvent>().Publish(
                    new AddGameMsgOrder { GameId = connectParam.stateObject.gameID, GameName = connectParam.stateObject.listViewModel.DownName, GameTime = gametime });
            }
            catch
            {
                if (File.Exists(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName + ".temp"))
                    File.Delete(connectParam.gamePath + connectParam.stateObject.listViewModel.DownName + ".temp");
            }
            finally
            {
                connectParam.StopTimerAndClose();

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    downViewModel.AllDowns.Remove(connectParam.stateObject.listViewModel);  //下载完成，移除
                    downViewModel.connectList.RemoveAll(conn => conn.stateObject.gameID == connectParam.stateObject.gameID);

                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        /// <summary>
        /// 大小转换
        /// </summary>
        private string ChangeToSize(long filesize)
        {
            if (filesize / (1024 * 1024) >= 1024)
            {
                return String.Format("{0:N1}", (double)filesize / (1024 * 1024 * 1024)) + "GB";
            }
            else if (filesize / 1024 >= 1024)
            {
                return String.Format("{0:N1}", (double)filesize / (1024 * 1024)) + "MB";
            }
            else if (filesize >= 1024)
            {
                return String.Format("{0:N1}", (double)filesize / (1024)) + "KB";
            }
            else
            {
                return String.Format("{0:N1}", (double)filesize / (1024)) + "B";
            }
        }

        #endregion

    }
}
