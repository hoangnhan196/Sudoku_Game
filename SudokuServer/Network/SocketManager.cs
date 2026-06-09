using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SudokuServer.Network
{
    internal class SocketManager
    {
        public class Y2Server
        {

            private const int BUFFER_SIZE = 1024;
            private const int PORT_NUMBER = 9999;

            static ASCIIEncoding encoding = new ASCIIEncoding();

            public static void Main()
            {
                try
                {
                    IPAddress address = IPAddress.Parse("127.0.0.1");

                    TcpListener listener = new TcpListener(address, PORT_NUMBER);

                    // 1. listen
                    listener.Start();

                    Console.WriteLine("Server started on " + listener.LocalEndpoint);
                    Console.WriteLine("Waiting for a connection...");

                    Socket socket = listener.AcceptSocket();
                    Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

                    // 2. receive
                    byte[] data = new byte[BUFFER_SIZE];
                    int bytesReceived = socket.Receive(data);

                    string str = encoding.GetString(data, 0, bytesReceived);
                    Console.WriteLine("Received: " + str);

                    // 3. send
                    socket.Send(encoding.GetBytes("Hello " + str));

                    // 4. close
                    socket.Close();
                    listener.Stop();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                Console.Read();
            }
        }
    }

}
