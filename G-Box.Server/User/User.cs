using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;

using System.Net.Sockets;
using System.IO;
using Microsoft.Practices.Prism.ViewModel;

namespace G_Box.Server
{
    public class User : NotificationObject
    {
        public TcpClient client { get; private set; }
        public StreamWriter sw { get; private set; }
        public StreamReader sr { get; private set; }
        public int userID { get; set; }
        public string cardWord { get; set; }
        public string randomKey { get; set; }
        public bool IsExit = false;

        private string ipAndPort;
        private string loginTime;
        private string runStatus;

        public User(TcpClient client)
        {
            this.client = client;
            this.IPAndPort = client.Client.RemoteEndPoint.ToString();

            NetworkStream networkStream = client.GetStream();
            sw = new StreamWriter(networkStream);
            sr = new StreamReader(networkStream);
        }

        public void Close()
        {
            sw.Close();
            sr.Close();
            client.Close();
        }

        /// <summary>
        /// IP与端口
        /// </summary>
        public string IPAndPort
        {
            get { return ipAndPort; }
            set
            {
                if (value == ipAndPort)
                    return;

                ipAndPort = value;

                base.RaisePropertyChanged("IPAndPort");
            } 
        }

        /// <summary>
        /// 登录时间
        /// </summary>
        public string LoginTime
        {
            get { return loginTime; }
            set
            {
                if (value == loginTime)
                    return;

                loginTime = value;

                base.RaisePropertyChanged("LoginTime");
            }
        }

        /// <summary>
        /// 用户活动状态
        /// </summary>
        public string RunStatus
        {
            get { return runStatus; }
            set
            {
                if (value == runStatus)
                    return;

                runStatus = value;

                base.RaisePropertyChanged("RunStatus");
            }
        }
    }
}
