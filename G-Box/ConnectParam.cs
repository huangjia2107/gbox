using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace G_Box
{
    public class ConnectParam
    {
        #region 单例定义

        private static ConnectParam instance = null;

        private ConnectParam() { }

        /// <summary>
        /// 返回唯一实例
        /// </summary> 
        public static ConnectParam GetInstance()
        {
            if (instance == null)
                instance = new ConnectParam();

            return instance;
        }

        #endregion

        #region 字段

        public bool IsExit = false; //是否正常退出
        public bool IsConnect = false; //是否连接成功 

        #endregion

        #region 属性

        public string RandomKey { get; set; } //随机生成的对称加密key 
        public string Msg { get; set; } //消息
        public Thread ListenerMsgThread { get; set; }
        public TcpClient tcpClient { get; set; }
        public StreamWriter sw { get; private set; }
        public StreamReader sr { get; private set; }

        #endregion

        #region 方法

        /// <summary>
        /// 获取Tcp流
        /// </summary>
        public void GetTcpClient()
        { 
            NetworkStream networkStream = tcpClient.GetStream();
            sw = new StreamWriter(networkStream, Encoding.UTF8);
            sr = new StreamReader(networkStream, Encoding.UTF8);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (tcpClient != null)
            {
                sw.Close();
                sr.Close();
                tcpClient.Close(); 
            }
            IsConnect = false;
            IsExit = true;

            if (ListenerMsgThread.IsAlive)
            { 
                ListenerMsgThread.Join(5000);
                ListenerMsgThread = null;
            } 
        } 

        #endregion
    }
}
