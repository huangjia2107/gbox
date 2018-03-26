using ConfigAccess;
using DataAccess.Access;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ToolClass.Logger;

namespace G_Box.Server
{
    class DownServer
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
        /// log对象，指向日志级别
        /// </summary>
        readonly ILogger ilogger;

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
        List<Down> downList = new List<Down>(); 

        #endregion

        #region 构造函数

        public DownServer(ConfigModel configModel, ILogger ilogger)
        {
            //获取服务器端参数
            this.configModel = configModel;
            this.ilogger = ilogger; 
        }

        #endregion

        #region 方法

        /// <summary>
        /// 启动侦听
        /// </summary>
        public bool StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(configModel.DownIP), int.Parse(configModel.DownPort));
                tcpListener.Start();
            }
            catch (Exception ex)
            {
                if (tcpListener != null)
                    tcpListener = null;

                ilogger.Logger(string.Format("游戏下载服务器侦听异常：{0}", ex.Message));
                return false; 
            } 

            this.ilogger.Logger(string.Format("游戏下载服务器开启侦听：{0}", configModel.DownIP+":"+configModel.DownPort));
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

                ilogger.Logger(string.Format("游戏下载服务器开启异常：{0}", ex.Message)); 
                return false; 
            }

            this.ilogger.Logger("游戏下载服务器接收消息线程已启动，等待客户连接中...");
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
                ilogger.Logger(string.Format("关闭游戏下载服务器异常：{0}", ex.Message));
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
                    Down down = new Down(tcpClient);

                    Thread threadReceive = new Thread(ReceiveData);
                    threadReceive.IsBackground = true;
                    threadReceive.Start(down);

                    downList.Add(down);
                     
                    this.ilogger.Logger(string.Format("用户{0}已连接,准备下载.", tcpClient.Client.RemoteEndPoint));
                }
            }
        }

        /// <summary>
        /// 处理接收的客户端数据
        /// </summary> 
        private void ReceiveData(object userState)
        {
            Down down = userState as Down;
            TcpClient tcpClient = down.client;

            try
            {
                Hashtable hashTable = GetFilePathAndUpdate(down.br.ReadString());
                string filepath = currentPath+hashTable["path"].ToString(); //根据ID得到文件路径  
                string gametime=hashTable["time"].ToString(); //版本时间

                this.ilogger.Logger(string.Format("游戏下载服务器开始向用户{0}发送文件.", tcpClient.Client.RemoteEndPoint));

                FileInfo fileInfo = new FileInfo(filepath);

                down.bw.Write(fileInfo.Name.ToLower()); //文件名
                down.bw.Write(fileInfo.Length); //文件大小 
                down.bw.Write(gametime); //版本时间

                using (FileStream fs = fileInfo.OpenRead())
                {
                    byte[] buffer = new byte[down.bufferSize];

                    while (down.readL < fileInfo.Length)
                    {
                        down.size = fs.Read(buffer, 0, down.bufferSize);
                        down.bw.Write(buffer, 0, down.size);
                        down.readL += down.size;
                    }
                }

                this.ilogger.Logger(string.Format("游戏下载服务器向用户{0}发送文件完成.", tcpClient.Client.RemoteEndPoint)); 
                RemoveUser(down);
            }
            catch (Exception ex)
            {
                this.ilogger.Logger(string.Format("游戏下载服务器与用户{0}链接异常：{1}", tcpClient.Client.RemoteEndPoint, ex.Message));  
                RemoveUser(down);
            }
        }

        /// <summary>
        /// 移除用户
        /// </summary> 
        private void RemoveUser(Down down)
        {
            this.ilogger.Logger(string.Format("游戏下载服务器与用户{0}断开连接.", down.client.Client.RemoteEndPoint)); 
            downList.Remove(down);
            down.Close();
            this.ilogger.Logger(string.Format("游戏下载服务器当前用户连接数：{0}.", downList.Count));  
        }

        /// <summary>
        /// 获取游戏目录
        /// </summary> 
        private Hashtable GetFilePathAndUpdate(string id)
        {
            Hashtable hashTable = new Hashtable(); 

            try
            {
                hashTable.Add("path", Access.GetGamePath(id));
                hashTable.Add("time", Access.GetGameTime(id));
            }
            catch (Exception ex)
            {
                this.ilogger.Logger(string.Format("获取游戏路径与时间版本出现异常：{0}.", ex.Message));
                return null;
            }

            return hashTable;
        }

        #endregion
    }
}
