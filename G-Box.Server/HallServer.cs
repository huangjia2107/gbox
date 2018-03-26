using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessageModule.MessageTypes;
using ConfigAccess; 
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using ToolClass.Logger;

namespace G_Box.Server
{
    class HallServer
    {
        #region 变量

        /// <summary>
        /// 获取侦听参数
        /// </summary>
        ConfigModel configModel;

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        readonly ILogger ilogger;

        /// <summary>
        /// 连接状态ViewModel
        /// </summary>
        ConnViewModel connViewModel;

        /// <summary>
        /// 侦听类
        /// </summary>
        TcpListener tcpListener;

        /// <summary>
        /// 接收消息线程
        /// </summary>
        Thread ListenerMsgThread;

        /// <summary>
        /// 是否正常退出
        /// </summary>
        bool IsExit = false;

        /// <summary>
        /// 信息处理的方法类
        /// </summary>
        DealMessage dealMessage;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary> 
        public HallServer(ConfigModel configModel, ILogger ilogger, ConnViewModel connViewModel)
        {
            //获取服务器端参数
            this.configModel = configModel;
            this.ilogger = ilogger;
            this.connViewModel = connViewModel;

            //实例化方法类
            dealMessage = new DealMessage(ilogger, connViewModel);
        }

        #endregion

        #region 方法

        /// <summary>
        /// 启动侦听
        /// </summary>
        public bool StartServer()
        {
            IsExit = false;
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(configModel.HallIP), int.Parse(configModel.HallPort));
                tcpListener.Start();
            }
            catch (Exception ex)
            {
                if (tcpListener != null)
                    tcpListener = null;

                ilogger.Logger(string.Format("大厅服务器侦听异常：{0}", ex.Message));
                return false; 
            }

            ilogger.Logger(string.Format("大厅服务器开启侦听：{0}", configModel.HallIP + ":" + configModel.HallPort));

            try
            {
                //定义接收消息线程
                ListenerMsgThread = new Thread(new ThreadStart(ListenerMsgThreadMethod));
                ListenerMsgThread.IsBackground = true;
                ListenerMsgThread.Start();
            }
            catch (Exception ex) 
            {
                if (ListenerMsgThread.IsAlive)
                {
                    ListenerMsgThread.Join(1000);
                }
                ilogger.Logger(string.Format("大厅服务器开启异常：{0}", ex.Message)); 
                return false; 
            }

            ilogger.Logger("大厅服务器接收消息线程已启动，等待客户连接中...");
            return true;
        }

        /// <summary>
        /// 关闭侦听
        /// </summary> 
        public bool StopServer()
        {
            try
            {
                IsExit = true;

                if (tcpListener != null)
                    tcpListener = null;

                if (ListenerMsgThread.IsAlive)
                {
                    ListenerMsgThread.Join(1000);
                }
            }
            catch (Exception ex)
            {
                ilogger.Logger(string.Format("关闭大厅服务器异常：{0}", ex.Message));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 侦听客户端连接请求
        /// </summary>
        private void ListenerMsgThreadMethod()
        {
            TcpClient tcpClient = null;
            while (true)
            {
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                }
                catch
                {
                    tcpClient = null;
                }

                if (tcpClient != null)
                {
                    User user = new User(tcpClient);

                    Thread threadReceive = new Thread(ReceiveData);
                    threadReceive.IsBackground = true;
                    threadReceive.Start(user);

                    AddUser(user);

                    ilogger.Logger(string.Format("用户{0}已连接.", tcpClient.Client.RemoteEndPoint));
                }
            }
        }

        /// <summary>
        /// 处理接收的客户端数据
        /// </summary> 
        private void ReceiveData(object userState)
        {
            User user = userState as User;
            TcpClient tcpClient = user.client;

            while (IsExit == false)
            {
                string receiveString = null;
                try
                {
                    receiveString = user.sr.ReadLine();
                }
                catch { receiveString = null; }

                if (receiveString == null)
                {
                    if (user.IsExit == false)
                    {
                        ilogger.Logger(string.Format("用户{0}断开连接.", tcpClient.Client.RemoteEndPoint));
                        RemoveUser(user);
                    }

                    break;
                }

                //解析消息或KEY
                ControlMsgOrKey(user, receiveString);
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary> 
        private void AddUser(User user)
        {
            if (!connViewModel.IsExist(user))
            {
                user.LoginTime = DateTime.Now.ToString("G");
                user.RunStatus = "已连接.";

                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    connViewModel.AllUsers.Add(user);

                }), System.Windows.Threading.DispatcherPriority.Normal);

            }
        }

        /// <summary>
        /// 移除用户
        /// </summary> 
        private void RemoveUser(User user)
        {
            if (connViewModel.IsExist(user))
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    connViewModel.AllUsers.Remove(user);

                    if (user.userID.ToString()!="0")
                        dealMessage.DealOFF(user); 

                }), System.Windows.Threading.DispatcherPriority.Normal);
            }

            user.Close();
            ilogger.Logger(string.Format("当前用户连接数：{0}.", connViewModel.AllUsers.Count));
        }

        /// <summary>
        /// 解析消息或KEY
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="receiveString">收到的加密后的字符串</param>
        private void ControlMsgOrKey(User user, string receiveString)
        {
            //根据私钥，解析出随机对称密钥
            if (receiveString.IndexOf(MessageTypes.KEY) > -1)
            {
                string key = RSAProcessor.RSADecrypt(configModel.PrivateKey,
                     receiveString.Substring(0, receiveString.Length - MessageTypes.KEY.Length));

                if (Regex.IsMatch(key, @"^[a-z0-9]{8}$"))
                    user.randomKey = key;
                else
                {
                    ilogger.Logger(string.Format("用户{0}KEY({1})不符合规范,执行断开.", user.client.Client.RemoteEndPoint, key));
                    RemoveUser(user);
                    user.IsExit = true;

                    return;
                }
            }
            //根据不同消息标志，做出不同处理
            else
            {
                //直接用对称密钥解密信息
                receiveString = RijndaelProcessor.DencryptString(receiveString, user.randomKey);

                //排查信息长度，最少3位
                if (receiveString.Length < 3)
                {
                    ilogger.Logger(string.Format("用户{0}信息长度{1}不符合规范,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Length));
                    RemoveUser(user);
                    return;
                }

                //排查是否为非法标志
                if (Array.IndexOf(MessageTypes.HallSignArray, receiveString.Substring(0, 3)) < 0)
                {
                    ilogger.Logger(string.Format("用户{0}用户{0}信息标志{1}非法,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Substring(0, 3)));
                    RemoveUser(user);
                    return;
                }

                var dealMethod = typeof(DealMessage).GetMethod(
                    ActionInTable(
                    //字符串标志转换为枚举
                      (MessageTypes.HallSign)Enum.Parse(
                          typeof(MessageTypes.HallSign),
                          receiveString.Substring(0, 3)
                    )));

                //提取消息，避免为单纯的标志，故做一判断
                receiveString = receiveString.Length > 3 ? receiveString.Substring(3, receiveString.Length - 3) : receiveString;

                //执行方法
                dealMethod.Invoke(
                    dealMessage, //封装了方法的类的实例
                    new object[] { user, receiveString } //传入方法的参数
                    );
            }
        }

        /// <summary>
        /// 根据枚举，返回相应处理方法名称
        /// </summary>
        /// <param name="sign">枚举标志</param>
        /// <returns>处理方法名称</returns>
        private string ActionInTable(MessageTypes.HallSign sign)
        {
            string[] methods = { "DealCON", "DealSET", "DealMSG", "DealFRI", "DealACH", "DealSFR", "DealAFR", "DealAGR", "DealNGR", "DealDFR", "DealMFR", "DealIFR", "DealAGA","DealGAM" };

            return methods[(int)sign];
        }

        #endregion 
    }
}
