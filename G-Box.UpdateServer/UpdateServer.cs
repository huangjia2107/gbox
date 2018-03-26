using ConfigAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace G_Box.UpdateServer
{
    class UpdateServer
    {
        #region 变量

        /// <summary>
        /// 当前应用路径
        /// </summary>
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;

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

        /// <summary>
        /// 存储用户
        /// </summary>
        List<User> userList = new List<User>();

        /// <summary>
        /// 可更新的文件
        /// </summary>
        string[] updateFiles;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary> 
        public UpdateServer()
        {
            //获取服务器端参数
            this.configModel =ConfigData.GetConfig();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 启动侦听
        /// </summary>
        public void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Parse(configModel.UpdateIP), int.Parse(configModel.UpdatePort));
            tcpListener.Start();
            Console.WriteLine("===================更新服务器开启==================");
            Console.WriteLine("==开始在{0}:{1}监听客户连接...            ==", configModel.UpdateIP, configModel.UpdatePort);

            //定义接收消息线程
            ListenerMsgThread = new Thread(new ThreadStart(ListenerMsgThreadMethod));
            ListenerMsgThread.IsBackground = true;
            ListenerMsgThread.Start();
            Console.WriteLine("==接收消息线程已启动，等待客户连接中...          ==");
            Console.WriteLine("=====================初始化完成====================");
        }

        /// <summary>
        /// 侦听客户端连接请求
        /// </summary>
        private void ListenerMsgThreadMethod()
        {
            TcpClient tcpClient = null;
            while (IsExit == false)
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

                    userList.Add(user);

                    Console.WriteLine("用户{0}已连接！", tcpClient.Client.RemoteEndPoint);
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

            try
            { 
                updateFiles = GetUpdateFile(currentPath + configModel.UpdatePath);

                if (updateFiles.Length > 1)
                {
                    Console.Write("该目录下不应出现多个文件！");
                    return;
                }

                Console.WriteLine("开始向用户{0}发送文件...", tcpClient.Client.RemoteEndPoint);

                FileInfo fileInfo= new FileInfo(updateFiles[0]);

                user.bw.Write(fileInfo.Name); //文件名
                user.bw.Write(fileInfo.Length); //文件大小 

                using (FileStream fs = fileInfo.OpenRead())
                {
                    byte[] buffer = new byte[user.bufferSize];

                    while (user.readL < fileInfo.Length)
                    {
                        user.size = fs.Read(buffer, 0, user.bufferSize);
                        user.bw.Write(buffer, 0, user.size);
                        user.readL += user.size;
                    }
                }

                Console.WriteLine("向用户{0}发送文件完成！", tcpClient.Client.RemoteEndPoint); 
                RemoveUser(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine("用户{0}连接异常：{1}", tcpClient.Client.RemoteEndPoint,ex.Message);
                RemoveUser(user);
            }
        }

        /// <summary>
        /// 移除用户
        /// </summary> 
        private void RemoveUser(User user)
        {
            Console.WriteLine("与用户{0}断开连接！", user.client.Client.RemoteEndPoint);
            userList.Remove(user);
            user.Close(); 
            Console.WriteLine("当前用户连接数：{0}", userList.Count);
        }

        /// <summary>
        /// 获取$path$目录下所有文件
        /// </summary>
        /// <param name="path">路径</param>
        private string[] GetUpdateFile(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    string[] files = Directory.GetFiles(@path,"*", SearchOption.TopDirectoryOnly);
                    return files;
                }
                catch
                {
                    Console.WriteLine("获取文件失败:路径{0}", path);
                    return null;
                }
            }
            return null;
        }

        #endregion
    }
}
