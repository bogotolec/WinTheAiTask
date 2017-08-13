using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WinTheAITask
{
    class ConnectionManager
    {
        public ConnectionManager()
        {

        }

        public void StartListen()
        {
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            IPEndPoint EP = new IPEndPoint(IP, 6969);

            Socket ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ListenSocket.Bind(EP);
            ListenSocket.Listen(32);

            Console.WriteLine("Start listening");

            while (true)
            {
                Socket Handler = ListenSocket.Accept();
                Connection Connection = new Connection(Handler);

                Console.WriteLine("New connection: " + Handler.RemoteEndPoint.ToString());

                Thread ThreadHandler = new Thread(Connection.Start);

                ThreadHandler.Start();
            }
        }
    }
}
