using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SudokuClient.Network
{
    public class NetworkClient
    {
        private const int PORT = 9999;
        private const int BUFFER_SIZE = 4096;

        private TcpClient _client;
        private NetworkStream _stream;

        public event Action<string> OnMessageReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;

        public bool IsConnected => _client?.Connected ?? false;

        public bool Connect(string host)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(host, PORT);
                _stream = _client.GetStream();

                OnConnected?.Invoke();

                Thread listenThread = new Thread(ListenFromServer);
                listenThread.IsBackground = true;
                listenThread.Start();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect failed: " + ex.Message);
                return false;
            }
        }

        public void Send(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send failed: " + ex.Message);
            }
        }

        private void ListenFromServer()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            try
            {
                while (IsConnected)
                {
                    int bytesRead = _stream.Read(buffer, 0, BUFFER_SIZE);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnMessageReceived?.Invoke(message);
                }
            }
            catch
            {
                OnDisconnected?.Invoke();
            }
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}