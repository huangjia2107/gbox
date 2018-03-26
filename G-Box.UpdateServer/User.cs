
using System.Net.Sockets;
using System.IO;

namespace G_Box.UpdateServer
{
    public class User
    {
        public int bufferSize = 2048;// 101376;  //缓冲大小
        public int size = 0;
        public long readL = 0;

        public TcpClient client { get; private set; }
        public BinaryWriter bw { get; private set; }
        public BinaryReader br { get; private set; }  

        public User(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            bw = new BinaryWriter(networkStream);
            br = new BinaryReader(networkStream);
        }

        public void Close()
        {
            bw.Close();
            br.Close();
            client.Close();
        }
    }
}
