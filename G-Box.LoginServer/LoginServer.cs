using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessageModule.MessageTypes;
using ConfigAccess; 
using System.Text.RegularExpressions;
using ToolClass.Logger;

namespace G_Box.LoginServer
{
    class LoginServer
    {
        #region 变量

        /// <summary>
        /// 获取侦听参数
        /// </summary>
        ConfigModel configModel;  

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

        ILogger ilogger;

        /// <summary>
        /// 存储用户
        /// </summary>
        List<User> userList = new List<User>(); 

        /// <summary>
        /// 信息处理的方法类
        /// </summary>
        DealMessage dealMessage; 

        #endregion 

        #region 构造函数 

        public LoginServer()
        {
            //获取服务器端参数
            this.configModel = ConfigData.GetConfig();

            ilogger = ILogger.GetInstance();
            //实例化方法类
            dealMessage = new DealMessage(); 
        }

        #endregion

        #region 方法

        /// <summary>
        /// 启动侦听
        /// </summary>
        public void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Parse(configModel.LoginIP),int.Parse(configModel.LoginPort));
            tcpListener.Start();

            ilogger.Logger("登录服务器开启.");
            ilogger.Logger(string.Format("开始在{0}:{1}监听客户连接.", configModel.LoginIP, configModel.LoginPort));

            Console.WriteLine("===================登录服务器开启==================");
            Console.WriteLine("==开始在{0}:{1}监听客户连接...            ==", configModel.LoginIP, configModel.LoginPort); 

            //定义接收消息线程
            ListenerMsgThread = new Thread(new ThreadStart(ListenerMsgThreadMethod));
            ListenerMsgThread.IsBackground = true;
            ListenerMsgThread.Start();

            ilogger.Logger("接收消息线程已启动，等待客户连接中."); 

            Console.WriteLine("==接收消息线程已启动，等待客户连接中...          ==");
            Console.WriteLine("=====================初始化完成====================");
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
                    user.threadReceive = threadReceive; 
                    threadReceive.Start(user);

                    userList.Add(user);

                    Console.WriteLine("用户{0}已连接.",tcpClient.Client.RemoteEndPoint);
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
                        ilogger.Logger(string.Format("与用户{0}断开连接.", tcpClient.Client.RemoteEndPoint));
                        Console.WriteLine("与用户{0}断开连接.", tcpClient.Client.RemoteEndPoint); 
                        RemoveUser(user); 
                    }

                    break;
                }

                //解析消息或KEY
                ControlMsgOrKey(user,receiveString); 
            }

            if (user.threadReceive.IsAlive)
            { 
                user.threadReceive.Join(2000);
                user.threadReceive = null;
            } 
        }

        /// <summary>
        /// 移除用户
        /// </summary> 
        private void RemoveUser(User user)
        {
            userList.Remove(user);
            user.Close();
            ilogger.Logger(string.Format("当前用户连接数：{0}", userList.Count));
            Console.WriteLine("当前用户连接数：{0}", userList.Count);
        }

        /// <summary>
        /// 解析消息或KEY
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <param name="receiveString">收到的加密后的字符串</param>
        private void ControlMsgOrKey(User user,string receiveString)
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
                    Console.WriteLine("用户{0}KEY({1})不符合规范,执行断开.", user.client.Client.RemoteEndPoint, key);
                    RemoveUser(user);
                    user.IsExit = true;

                    return;
                }
            }
            //根据不同消息标志，做出不同处理
            else
            {
                //直接用对称密钥解密信息
                receiveString = RijndaelProcessor.DencryptString(receiveString,user.randomKey);

                //排查信息长度，最少3位
                if (receiveString.Length < 3)
                {
                    ilogger.Logger(string.Format("用户{0}信息长度{1}不符合规范,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Length));
                    Console.WriteLine("用户{0}信息长度{1}不符合规范,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Length);
                    RemoveUser(user);
                    return;
                }

                //排查是否为非法标志
                if (Array.IndexOf(MessageTypes.LoginSignArray, receiveString.Substring(0, 3)) < 0)
                {
                    ilogger.Logger(string.Format("用户{0}信息标志{1}非法,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Substring(0, 3)));
                    Console.WriteLine("用户{0}信息标志{1}非法,执行断开.", user.client.Client.RemoteEndPoint, receiveString.Substring(0, 3));
                    RemoveUser(user);
                    return;
                }

                var dealMethod = typeof(DealMessage).GetMethod(
                    ActionInTable(
                       //字符串标志转换为枚举
                      (MessageTypes.LoginSign)Enum.Parse(
                          typeof(MessageTypes.LoginSign),
                          receiveString.Substring(0,3)
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
        private string ActionInTable(MessageTypes.LoginSign sign)
        {
            string[] methods = { "DealUpdate","DealLogin", "DealRNU", "DealRegister","DealVNU", "DealReset" };

            return methods[(int)sign];
        }
         
        #endregion

    }
}
