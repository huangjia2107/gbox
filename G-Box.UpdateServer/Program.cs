using System;
using ToolClass.GZip;

namespace G_Box.UpdateServer
{
    class Program
    {
        static void Main(string[] args)
        {
            UpdateServer updateServer = new UpdateServer();
            updateServer.StartServer();

            Console.ReadLine();
        }
    }
}
