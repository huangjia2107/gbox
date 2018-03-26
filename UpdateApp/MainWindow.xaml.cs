
using System;
using System.Windows;
using System.Windows.Controls;

using Configure.Models;
using Configure;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using ToolClass.GZip;
using System.Diagnostics;

namespace UpdateApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量

        /// <summary>
        /// 获取服务器端参数
        /// </summary>
        ConfigModel configModel; 

        /// <summary>
        /// 获取ViewModel对象
        /// </summary>
        ViewModel viewModel;

        /// <summary>
        /// 当前应用路径
        /// </summary>
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            viewModel = this.DataContext as ViewModel;
        }

        #endregion

        #region Window_Loaded

        private void updateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.ConnectStatus = "获得服务器参数...";
            configModel = ConfigAccess.GetConfig();

            viewModel.ConnectStatus = "连接服务器...";
            ConnectToServer(configModel.UpdateIP, configModel.UpdatePort);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 创建缓存目录
        /// </summary>
        /// <param name="path">路径</param>
        private void CreateTempPath(string path)
        {
            if (!File.Exists(path) && path != "")
            {
                try
                {
                    viewModel.ConnectStatus = "创建缓存目录...";

                    Directory.CreateDirectory(path);
                }
                catch { }
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void ConnectToServer(string ip, string port)
        {
            TcpClient tcpClient = null;

            tcpClient = new TcpClient();
            tcpClient.BeginConnect(ip, int.Parse(port), new AsyncCallback(ConnCallBackF), tcpClient); //异步连接          
        }

        /// <summary>
        /// 委托
        /// </summary>
        /// <param name="connectParam">连接参数</param>
        private delegate void ReceiveFileDelegate(ConnectParam connectParam);

        /// <summary>
        /// 连接服务器异步回调
        /// </summary>
        public void ConnCallBackF(IAsyncResult ar)
        {
            TcpClient newClient = (TcpClient)ar.AsyncState;
            try
            {
                newClient.EndConnect(ar);
                //连接成功
                viewModel.ConnectStatus = "连接成功...";

                ConnectParam connectParam = new ConnectParam(newClient);
                CreateTempPath(connectParam.tempPath);//创建缓存路径

                viewModel.ConnectStatus = "准备接收文件...";

                ReceiveFileDelegate d = new ReceiveFileDelegate(ReceiveData);
                d.BeginInvoke(connectParam, new AsyncCallback(CallBackF), null);
            }
            catch
            {
                MessageBox.Show("未能与服务器取得连接！");
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
            Dispatcher.Invoke(new Action(() =>
            {
                Grid.SetRow(receiveStatus, 1);
                Grid.SetRow(connStatus, 2);

            }), System.Windows.Threading.DispatcherPriority.Normal);

            viewModel.FileName = connectParam.br.ReadString(); //文件名 

            long fileSize = connectParam.br.ReadInt64();  //文件大小 
            string fileSize_m = ChangeToSize(fileSize); //转换为MB 

            using (connectParam.fs = new FileStream(connectParam.tempPath + viewModel.FileName, FileMode.Create))
            {
                byte[] buffer = new byte[connectParam.bufferSize];

                connectParam.StartTimer(); //开启计时器

                while (connectParam.readL < fileSize)
                {
                    connectParam.size = connectParam.br.Read(buffer, 0, connectParam.bufferSize);
                    connectParam.fs.Write(buffer, 0, connectParam.size);
                    connectParam.readL += connectParam.size;

                    viewModel.FileSize = ChangeToSize(connectParam.readL) + "/" + fileSize_m;
                    if (connectParam.sec != 0)
                    {
                        long downSpeed = connectParam.readL / connectParam.sec;

                        viewModel.DownSpeed = ChangeToSize(downSpeed)+"/S"; //速度
                        viewModel.LeftTime = new TimeSpan(0, 0, Convert.ToInt32((fileSize - connectParam.readL) / downSpeed)).ToString(); //剩余时间
                    }
                    viewModel.PgBarValue = String.Format("{0:N1}", (double)connectParam.readL * 100 / fileSize);
                }
            }

            connectParam.StopTimerAndClose();

            DecompressFile(connectParam);
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

        /// <summary>
        /// 解压文件
        /// </summary>
        private void DecompressFile(ConnectParam connectParam)
        {
            viewModel.UpdateStatus = "开始更新文件...";

            Dispatcher.Invoke(new Action(() =>
            {
                Grid.SetRow(updateStatus, 1);
                Grid.SetRow(receiveStatus, 3);

            }), System.Windows.Threading.DispatcherPriority.Normal);

            viewModel.UpdateStatus = "解压文件...";
            try
            {
                GZip.Decompress(connectParam.tempPath,connectParam.tempPath, "updatefiles");
                viewModel.UpdateStatus = "解压完成...";
            }
            catch (Exception ex)
            {
                viewModel.UpdateStatus = "解压出错："+ex.Message;
                return;
            }

            //备份，更新，还原，清理
            DealUpdate dealUpdate = DealUpdate.GetInstance();
            dealUpdate.ConfigUpdate(connectParam,viewModel);

            try
            {
                viewModel.UpdateStatus = "登陆程序重新启动中...";
                Process.Start(@"G-Box.exe");
            }
            catch
            {
                MessageBox.Show("启动程序失败,请手动自行启动！"); 
            }
            finally
            {
                viewModel.UpdateStatus = "等待关闭更新程序...";
                Environment.Exit(0);
            }
        } 

        #endregion 
    }
}
