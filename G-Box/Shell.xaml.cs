using System;
using System.Windows;

using G_Box.ViewModels;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using MessageModule.ReceiveMsg;
using MessageModule.SendMsg;
using System.Net.Sockets;
using XWT = Xceed.Wpf.Toolkit;
using ToolClass.String;
using ToolClass.StoryBoard;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Modularity;
using MessageModule.MessageTypes;
using Configure;
using Configure.Models;
using Configure.DataAccess;
using System.Diagnostics;
using ToolClass.Logger;

namespace G_Box
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class Shell : Window
    {
        #region 变量

        ReceiveMsgOrder receiveMsgOrder;  //消息结构
        IEventAggregator receive_Aggregator;
        IEventAggregator send_Aggregator;
        SubscriptionToken subscriptionToken;

        IModuleManager moduleManager;

        /// <summary>
        /// 获取服务器端参数
        /// </summary>
        ConfigModel configModel;

        /// <summary>
        /// 获取客户端版本信息
        /// </summary>
        VersionModel versionModel;

        /// <summary>
        /// 连接信息类 
        /// </summary>
        ConnectParam connectParam;

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        private readonly ILogger ilogger;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public Shell(IEventAggregator eventAggregator, IModuleManager moduleManager)
        {
            InitializeComponent();

            this.receive_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator;

            this.moduleManager = moduleManager;

            //定义消息结构
            receiveMsgOrder = new ReceiveMsgOrder();
            //获取服务器端参数
            configModel = ConfigAccess.GetConfig();
            //获取客户端版本信息
            versionModel = VersionAccess.GetConfig();
            //获取连接信息类的单例
            connectParam = ConnectParam.GetInstance();
            //获取日志记录实例
            this.ilogger = ILogger.GetInstance();

            //订阅接收信息事件
            RequestEvent();

            //定义接收消息线程
            connectParam.ListenerMsgThread = new Thread(ListenerMsgThreadMethod);
            connectParam.ListenerMsgThread.IsBackground = true;

            //先建立连接及SSL通道,再发送
            ConnectToServer(MessageTypes.UPD + versionModel.AppVs + MessageTypes.NSP + versionModel.UpdateTime, //确认更新
                configModel.LoginIP, int.Parse(configModel.LoginPort));
        }

        [Import]
        ShellViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }

        #endregion

        #region IEventAggregator订阅接收信息事件(各模块->主程序(客户端->服务器))

        public void RequestEvent()
        {
            SendMsgEvent sendMsgEvent = send_Aggregator.GetEvent<SendMsgEvent>();

            if (subscriptionToken != null)
                sendMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = sendMsgEvent.Subscribe(OnReceiveMsg, ThreadOption.UIThread, false, (message) => { return true; });
        }

        public void OnReceiveMsg(string message)
        {
            if (connectParam.IsConnect == true)
            {
                //直接发送加密信息  
                AsyncSendMessage(connectParam, RijndaelProcessor.EncryptString(message, connectParam.RandomKey));
            }
            else
            {
                //先建立连接及SSL通道,再发送
                ConnectToServer(message, configModel.LoginIP, int.Parse(configModel.LoginPort));
            }

            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        #endregion

        #region 接收消息

        /// <summary>
        /// 侦听信息命令线程方法
        /// </summary>
        private void ListenerMsgThreadMethod(object connectState)
        {
            ConnectParam connectParam = connectState as ConnectParam;
            string receiveString = null;

            while (connectParam.IsExit == false)
            {
                try
                {
                    receiveString = connectParam.sr.ReadLine();
                }
                catch (Exception e)
                {
                    if (connectParam.IsExit == false)
                    {
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //关闭连接
                            CloseConnect(connectParam);

                            XWT.MessageBox.Show("与服务器断开连接！\n错误:" + e.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);

                        }), System.Windows.Threading.DispatcherPriority.Normal);
                    }
                    break;
                }
                if (receiveString != null)
                {
                    receiveString = RijndaelProcessor.DencryptString(receiveString, connectParam.RandomKey);

                  //  ilogger.Logger(string.Format("接收到的数据:{0}", receiveString));

                    //解密、分割
                    string[] Msg = receiveString.Split(MessageTypes.PSP.ToCharArray());
                     
                    //存储
                    receiveMsgOrder.ModuleType = Msg[0];
                    receiveMsgOrder.Sign = int.Parse(Msg[1]);
                    receiveMsgOrder.MsgContent = Msg[2];

                    if (receiveMsgOrder.ModuleType == MessageTypes.UPD)
                    {
                        DealUpdate(receiveMsgOrder.MsgContent);
                    }
                    else
                    {
                        //广播
                        receive_Aggregator.GetEvent<ReceiveMsgEvent>().Publish(new ReceiveMsgOrder {ModuleType=Msg[0],Sign=int.Parse(Msg[1]),MsgContent=Msg[2] });

                        //连接成功，但操作异常
                        if (receiveMsgOrder.Sign == 0 && receiveMsgOrder.ModuleType == MessageTypes.CON)
                        {
                            break; 
                        }

                        //登陆成功，返回“账号+昵称+头像”，并切换到大厅服务器
                        if (receiveMsgOrder.Sign == 1 && receiveMsgOrder.ModuleType == MessageTypes.LOG)
                        { 
                            CloseConnect(connectParam); 
                            //建立新的连接及SSL通道，然后发送信息（CON++ID+账号）
                            ConnectToServer(MessageTypes.CON + receiveMsgOrder.MsgContent.Split(MessageTypes.NSP.ToCharArray())[0] + MessageTypes.NSP + receiveMsgOrder.MsgContent.Split(MessageTypes.NSP.ToCharArray())[1], configModel.HallIP, int.Parse(configModel.HallPort));

                            break;
                        } 
                    }
                }
            } 
        }

        #endregion

        #region 方法

        /// <summary>
        /// 处理更新消息
        /// </summary>
        /// <param name="msg"></param>
        private void DealUpdate(string msg)
        {
            //新版本可以更新
            if (msg.Length > 3)
            {
                string[] msgs = msg.Split(MessageTypes.NSP.ToCharArray());

                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBoxResult dr;
                    dr = XWT.MessageBox.Show("软件有新的版本可以更新：\n当前版本："
                        + versionModel.AppVs + "\n新版本：" + msgs[0] + "\n更新时间：" + msgs[1] + "\n是否更新到最新版本？",
                        "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:

                            try
                            {
                                Process.Start(@"UpdateApp.exe");
                                Environment.Exit(0);
                            }
                            catch { XWT.MessageBox.Show("更新程序启动失败！"); }

                            break;

                        case MessageBoxResult.No:
                            ; break;
                    }

                }), System.Windows.Threading.DispatcherPriority.Normal);


            }
            else//版本无更新
            {
                switch (msg)
                {
                    //更新数据
                    case MessageTypes.CUP:
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                Process.Start(@"UpdateApp.exe");
                                Environment.Exit(0);
                            }
                            catch { XWT.MessageBox.Show("更新程序启动失败！"); }

                        }), System.Windows.Threading.DispatcherPriority.Normal);

                        break;

                    //不用更新
                    case MessageTypes.NUP:
                        break;
                }
            }
        }

        #endregion

        #region 发送处理

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="isSSL">是否建立了SSL</param>
        public void ConnectToServer(string message, string ip, int port)
        {
            connectParam.IsConnect = false;

            connectParam.tcpClient = new TcpClient();
            connectParam.Msg = message;
            //获得Key
            connectParam.RandomKey = GetString.GetRandomCode(GetString.Code.StrAndNum, 8);

            connectParam.tcpClient.BeginConnect(ip, port, new AsyncCallback(ConnectCallBackF), connectParam); //异步连接          
        }

        /// <summary>
        /// 连接服务器异步回调
        /// </summary>
        /// <param name="message">发送的信息</param>
        public void ConnectCallBackF(IAsyncResult ar)
        {
            ConnectParam connectParam = (ConnectParam)ar.AsyncState;
            try
            {
                connectParam.tcpClient.EndConnect(ar);
                connectParam.IsConnect = true; 

                connectParam.GetTcpClient();

                //连接成功,然后开启线程等待接收消息
                connectParam.ListenerMsgThread.Start(connectParam);

                //先建立SSL通道(利用公钥将对称key加密发给服务器)
                AsyncSendMessage(connectParam, RSAProcessor.RSAEncrypt(configModel.PublicKey, connectParam.RandomKey) + MessageTypes.KEY);

                //发送第一条用对称key加密的信息
                AsyncSendMessage(connectParam, RijndaelProcessor.EncryptString(connectParam.Msg, connectParam.RandomKey));
            }
            catch
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    StoryboardManager.StopStoryboard("Story_Login");
                    XWT.MessageBox.Show("未能连接到服务器,请检查网络状态!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);

                }), System.Windows.Threading.DispatcherPriority.Normal);

                if (connectParam.tcpClient != null)
                {
                    connectParam.tcpClient = null;
                }
                connectParam.IsConnect = false;
            }
        }

        /// <summary>
        /// 向服务器发送数据
        /// </summary>
        /// <param name="message">发送的信息</param>
        public void AsyncSendMessage(ConnectParam connectParam, string message)
        {
            try
            {
                connectParam.sw.WriteLine(message);
                connectParam.sw.Flush();
            }
            catch
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    StoryboardManager.StopStoryboard("Story_Login");
                    XWT.MessageBox.Show("数据发送失败,请重试!", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect(ConnectParam connectParam)
        {
            connectParam.Close();

            connectParam.ListenerMsgThread = new Thread(ListenerMsgThreadMethod);
            connectParam.ListenerMsgThread.IsBackground = true;
            //ListenerMsgThread.Start();

            connectParam.IsExit = false;
        }

        #endregion

    }
}
