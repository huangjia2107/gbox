
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
 
using System.IO; 
using ConfigAccess;
using System.Diagnostics;
using SWF = System.Windows.Forms;
using ToolClass.GZip;
using ToolClass.ProcessInstance;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ToolClass.Logger;
using MessageModule.MessageTypes;
using DataAccess.Models;
using DataAccess.Access;

namespace G_Box.Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 变量

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        private readonly ILogger ilogger;

        /// <summary>
        /// 当前应用路径
        /// </summary>
        string currentPath = AppDomain.CurrentDomain.BaseDirectory; 

        /// <summary>
        /// 获取侦听参数
        /// </summary>
        ConfigModel configModel;

        /// <summary>
        /// 连接状态ViewModel
        /// </summary>
        ConnViewModel connViewModel;

        /// <summary>
        /// 大厅服务器
        /// </summary>
        HallServer hallServer;

        /// <summary>
        /// 下载服务器
        /// </summary>
        DownServer downServer;

        #endregion

        #region 构造函数

        public MainWindow()
        {
            InitializeComponent();

            //获取服务器端参数
            configModel = ConfigData.GetConfig();
            //获取日志记录实例
            this.ilogger = ILogger.GetInstance();
            //获取实例
            connViewModel = connView.DataContext as ConnViewModel;
        }

        #endregion

        #region 按钮事件

        /// <summary>
        /// 服务器管理
        /// </summary> 
        private void Manage_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Name)
            {
                #region 启动登录服务器
                case "loginStart":
                    try
                    {
                        if (ProcessInstance.IsRuning("G-Box.LoginServer"))
                        {
                            MessageBox.Show("登录服务器已经启动！");
                        }
                        else
                        {
                            Process.Start(@"G-Box.LoginServer.exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("启动登录服务器出错！");
                        ilogger.Logger(string.Format("启动登录服务器异常:{0}", ex.Message));
                    }
                    break;
                #endregion

                #region 启动大厅服务器
                case "hallStart":

                    hallServer = new HallServer(configModel,ilogger,connViewModel);
                    downServer = new DownServer(configModel,ilogger);

                    if (hallServer.StartServer() && downServer.StartServer())
                    {
                        hallStart.IsEnabled = false;
                        hallClose.IsEnabled = true;
                        hallStatus.Text = "已启动.";
                        downStatus.Text = "已启动.";
                    }
                    else
                    {
                        MessageBox.Show("启动失败,请重试！"); 
                    }

                    break;
                #endregion

                #region 关闭大厅服务器
                case "hallClose":
                    if (hallServer.StopServer() && downServer.StopServer())
                    {
                        hallStart.IsEnabled = true;
                        hallClose.IsEnabled = false;
                        hallStatus.Text = "未启动.";
                        downStatus.Text = "未启动.";
                    }
                    else
                    {
                        MessageBox.Show("停止失败,请重试！");
                    }
                    
                    break;
                #endregion

                #region 启动更新服务器
                case "updateStart":

                    try
                    {
                        if (ProcessInstance.IsRuning("G-Box.UpdateServer"))
                        {
                            MessageBox.Show("更新服务器已经启动！");
                        }
                        else
                        {
                            Process.Start(@"G-Box.UpdateServer.exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("启动更新服务器出错！");
                        ilogger.Logger(string.Format("启动更新服务器异常:{0}", ex.Message));
                    }
                    break;
                #endregion

                #region 浏览更新文件夹
                case "updateScan":
                    try
                    {
                        SWF.FolderBrowserDialog fbd = new SWF.FolderBrowserDialog();
                        fbd.ShowDialog();
                        updatePath.Text = fbd.SelectedPath;  //文件夹路径  
                    }
                    catch (Exception ex)
                    {
                        ilogger.Logger(string.Format("浏览文件夹异常:{0}", ex.Message));
                    }
                    break;
                #endregion

                #region 压缩更新文件
                case "updateZip":
                    if (File.Exists(currentPath + "UpdatePath\\updatefiles"))
                    {
                        MessageBox.Show("更新文件已经存在！");
                        return;
                    }

                    try
                    {
                        updateZip.IsEnabled = false;
                        updateZip.Content = "Wait.";
                        string path = updatePath.Text.Trim();

                        Task t = new Task(() =>
                            {
                                if (path != null && path != "")
                                {
                                    //指定文件夹下的压缩
                                    GZip.Compress(@path + "\\", currentPath + "UpdatePath\\", "updatefiles");
                                }
                                else
                                {
                                    //默认文件夹
                                    GZip.Compress(currentPath + "UpdatePath\\Backup\\", currentPath + "UpdatePath\\", "updatefiles");
                                }
                            });
                        t.Start();
                        t.ContinueWith((task) =>
                            {
                                if (!task.IsFaulted && task.IsCompleted)
                                {
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        updateZip.IsEnabled = true;
                                        updateZip.Content = "压缩";
                                        updatePath.Text = "";

                                        MessageBox.Show("压缩执行完毕！");

                                    }), System.Windows.Threading.DispatcherPriority.Normal);
                                }
                            });

                    }
                    catch (Exception ex)
                    {
                        ilogger.Logger(string.Format("压缩文件异常:{0}", ex.Message));
                    }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// 服务器配置
        /// </summary> 
        private void Config_Click(object sender, RoutedEventArgs e)
        {
            if (!Check(loginIP.Text, loginPort.Text))
                return;

            Button btn = sender as Button;
            switch (btn.Name)
            {
                #region 配置登录服务器
                case "loginApply":
                    try
                    {
                        ConfigData.SetConfig(loginIP.Text.Trim(), loginPort.Text.Trim(), ConfigData.Type.login);
                        loginIP.Text = "";  loginPort.Text = "";
                        MessageBox.Show("配置完成！");
                    }
                    catch { MessageBox.Show("修改异常,请重试..."); }
                    break;
                case "loginRestore":
                    try
                    {
                        ConfigData.SetRestore(ConfigData.Type.login);
                        MessageBox.Show("已恢复默认！");
                    }
                    catch { MessageBox.Show("恢复异常,请重试..."); }
                    break;
                #endregion

                #region 配置大厅服务器
                case "hallApply":
                    try
                    {
                        ConfigData.SetConfig(hallIP.Text.Trim(), hallPort.Text.Trim(), ConfigData.Type.hall);
                        hallIP.Text = "";  hallPort.Text = "";
                        MessageBox.Show("配置完成！");
                    }
                    catch { MessageBox.Show("修改异常,请重试..."); }
                    break;
                case "hallRestore":
                    try
                    {
                        ConfigData.SetRestore(ConfigData.Type.hall);
                        MessageBox.Show("已恢复默认！");
                    }
                    catch { MessageBox.Show("恢复异常,请重试..."); }
                    break;
                #endregion

                #region 配置游戏下载服务器
                case "downApply":
                    try
                    {
                        ConfigData.SetConfig(downIP.Text.Trim(), downPort.Text.Trim(), ConfigData.Type.down);
                        downIP.Text = ""; downPort.Text = "";
                        MessageBox.Show("配置完成！");
                    }
                    catch { MessageBox.Show("修改异常,请重试..."); }
                    break;
                case "downRestore":
                    try
                    {
                        ConfigData.SetRestore(ConfigData.Type.down);
                        MessageBox.Show("已恢复默认！");
                    }
                    catch { MessageBox.Show("恢复异常,请重试..."); }
                    break;
                #endregion

                #region 配置更新服务器
                case "updateApply":
                    try
                    {
                        ConfigData.SetConfig(updateIP.Text.Trim(), updatePort.Text.Trim(), ConfigData.Type.update);
                        updateIP.Text = "";  updatePort.Text = "";
                        MessageBox.Show("配置完成！");
                    }
                    catch { MessageBox.Show("修改异常,请重试..."); }
                    break;
                case "updateRestore":
                    try
                    {
                        ConfigData.SetRestore(ConfigData.Type.update);
                        MessageBox.Show("已恢复默认！");
                    }
                    catch { MessageBox.Show("恢复异常,请重试..."); }
                    break;
                #endregion
            }
        }

        /// <summary>
        /// 消息管理
        /// </summary> 
        private void Msg_Click(object sender, RoutedEventArgs e)
        { 
            Button btn = sender as Button;

            switch (btn.Name)
            {
                #region 发布消息
                case "sendMsg":
                    if (titleBox.Text.Trim() == "" || contentBox.Text.Trim() == "")
                    {
                        MessageBox.Show("请填写完整信息！");
                    }

                    sendMsg.IsEnabled = false;
                    sendMsg.Content = "等待."; 

                    NoticeInfo noticeInfo = new NoticeInfo { MsgTitle=titleBox.Text.Trim(),MsgContent=contentBox.Text.Trim()};

                    try
                    {
                        Access.InsertNotice(noticeInfo); 
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("插入数据异常："+ex.Message);
                        sendMsg.IsEnabled = true;
                        sendMsg.Content = "发送";
                        return;
                    }

                    foreach (User u in connViewModel.AllUsers)
                    {
                        u.sw.WriteLine(RijndaelProcessor.EncryptString(MessageTypes.MSG + MessageTypes.PSP + "1" + MessageTypes.PSP+
                        noticeInfo.MsgType + MessageTypes.NSP + noticeInfo.MsgTitle + MessageTypes.NSP + noticeInfo.MsgContent + MessageTypes.NSP + noticeInfo.MsgPublish, u.randomKey));
                        u.sw.Flush();
                    }

                    sendMsg.IsEnabled = true;
                    sendMsg.Content = "发送";
                    titleBox.Text = "";
                    contentBox.Text = "";
                    MessageBox.Show("信息发布成功");

                    break;
                #endregion

                #region 清空消息记录
                case "clearMsg":

                    clearMsg.IsEnabled = false;
                    clearMsg.Content = "等待.";

                    try
                    {
                        Access.ClearNotice(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("清空数据异常：" + ex.Message);
                        clearMsg.IsEnabled = false;
                        clearMsg.Content = "等待.";
                        return;
                    }

                    clearMsg.IsEnabled = true;
                    clearMsg.Content = "清空已发布信息记录";
                    MessageBox.Show("数据清空成功!");

                    break;
                #endregion
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 检查合法性
        /// </summary> 
        private bool Check(string ip, string port)
        {
            if (!Regex.IsMatch(ip.Trim() + ":" + port.Trim(), @"^(?<ip>(?:25[0-5]|2[0-4]\d|1\d{0,2}|[1-9]\d?)\.(?:(?:25[0-5]|2[0-4]\d|1\d{0,2}|\d{1,2})\.){2}(?:25[0-5]|2[0-4]\d|1\d{0,2}|\d{1,2}))(:(?<port>\d{0,4}))?$"))
            {
                MessageBox.Show("您输入IP或端口不合法！");
                return false;
            }

            return true;
        }

        #endregion 
    }
}
