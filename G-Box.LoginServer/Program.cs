using ConfigAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace G_Box.LoginServer
{
    class Program
    { 
        static void Main(string[] args)
        {
            LoginServer loginServer = new LoginServer();
            loginServer.StartServer(); 

            Console.ReadLine();
        } 
    }
}
