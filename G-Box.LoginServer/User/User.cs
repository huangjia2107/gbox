
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace G_Box.LoginServer
{
    public class User
    {
        public TcpClient client { get; private set; }
        public StreamWriter sw { get; private set; }
        public StreamReader sr { get; private set; }
        public Thread threadReceive { get; set; }
        public bool IsExit = false;

        public string cardWord { get; set; }
        public string userName { get; set; }
        public string randomKey { get; set; }
        public string userRMail { get; set; } //注册邮箱
        public string userVMail { get; set; } //验证邮箱
        public string regNum { get; set; }  //注册码
        public string verNum { get; set; }  //验证码

        public User(TcpClient client)
        {
            this.client = client;
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
    }
}
