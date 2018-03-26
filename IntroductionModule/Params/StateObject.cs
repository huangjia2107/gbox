using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace IntroductionModule
{
    public class StateObject
    {
        public string gameID { get; set; }
        public TcpClient newClient { get; set; }
        public ListViewModel listViewModel { get; set; }
    }
}
