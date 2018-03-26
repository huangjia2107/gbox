using System;
using System.IO;
using System.Net.Sockets;
using System.Timers;

namespace IntroductionModule
{
    public class ConnectParam
    { 
        public int bufferSize = 2048;// 101376;  //缓冲大小
        public int size = 0;
        public long readL = 0;
        public bool IsOver = false;
        public string gamePath = AppDomain.CurrentDomain.BaseDirectory+"Games\\"; //更新的缓存路径  

        int i=0;
        public int sec 
        { 
            get 
            { 
                return i; 
            } 
            private set 
            { 
                value = i; 
            } 
        }

        public StateObject stateObject { get; set; }
        public FileStream fs { get; set; }
        public TcpClient client { get; private set; }
        public BinaryWriter bw { get; private set; }
        public BinaryReader br { get; private set; }
        public Timer timer { get; private set; }

        public ConnectParam(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            bw = new BinaryWriter(networkStream);
            br = new BinaryReader(networkStream);
        }

        public void StartTimer()
        {
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(TimerTick);
            timer.Interval = 1000;
            timer.Enabled = true;   
        }

        public void StopTimerAndClose()
        {
            this.timer.Stop();
            Close();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.i++;
        }

        private void Close()
        {
            if (client != null)
            {
                bw.Close();
                br.Close();
                client.Close();
            }
        }
    }
}

