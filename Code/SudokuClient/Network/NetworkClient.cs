using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SudokuClient.Models;

namespace SudokuClient.Network
{
    /// <summary>
    /// Result returned from UDP LAN server discovery.
    /// </summary>
    public class DiscoveryResult
    {
        public string ServerIP { get; set; } = string.Empty;
        public int ServerPort { get; set; }
    }

    public class NetworkClient
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private CancellationTokenSource? _cts;
        private int _port = 0;

        public event Action<NetworkMessage>? OnMessageReceived;
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public bool IsConnected => _client?.Connected ?? false;

        /// <summary>
        /// Sends a UDP broadcast to discover the Sudoku server on the LAN.
        /// Returns a DiscoveryResult with the server's IP and port, or null if not found.
        /// </summary>
        public static async Task<DiscoveryResult?> DiscoverServerAsync(int timeoutMs = 3000)
        {
            try
            {
                using var udp = new UdpClient();
                udp.EnableBroadcast = true;

                byte[] request = Encoding.UTF8.GetBytes("SUDOKU_DISCOVER");
                var broadcastEp = new IPEndPoint(IPAddress.Broadcast, 10000);

                // Send broadcast discovery request
                await udp.SendAsync(request, request.Length, broadcastEp);

                // Wait for response with timeout
                using var cts = new CancellationTokenSource(timeoutMs);

                try
                {
                    var receiveTask = udp.ReceiveAsync(cts.Token);
                    var result = await receiveTask;

                    string response = Encoding.UTF8.GetString(result.Buffer);
                    // Expected format: "SUDOKU_SERVER|<port>"
                    if (response.StartsWith("SUDOKU_SERVER|"))
                    {
                        string[] parts = response.Split('|');
                        if (parts.Length >= 2 && int.TryParse(parts[1], out int port))
                        {
                            return new DiscoveryResult
                            {
                                ServerIP = result.RemoteEndPoint.Address.ToString(),
                                ServerPort = port
                            };
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Timeout - no server found
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Discovery failed: " + ex.Message);
            }

            return null;
        }

        public bool Connect(string host, int port = 9999)
        {
            try
            {
                _port = port;
                _client = new TcpClient();
                _client.Connect(host, _port);
                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
                _cts = new CancellationTokenSource();

                OnConnected?.Invoke();

                Task.Run(() => ListenFromServerAsync(_cts.Token));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect failed: " + ex.Message);
                return false;
            }
        }

        public async Task SendAsync(NetworkMessage message)
        {
            if (!IsConnected || _writer == null) return;
            try
            {
                string json = JsonSerializer.Serialize(message);
                await _writer.WriteLineAsync(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send failed: " + ex.Message);
                Disconnect();
            }
        }

        private async Task ListenFromServerAsync(CancellationToken token)
        {
            try
            {
                while (IsConnected && !token.IsCancellationRequested && _reader != null)
                {
                    string? line = await _reader.ReadLineAsync(token);
                    if (line == null) break;

                    try
                    {
                        var msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                        if (msg != null)
                        {
                            OnMessageReceived?.Invoke(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Deserialize error: " + ex.Message);
                    }
                }
            }
            catch (Exception)
            {
                // Disconnected
            }
            finally
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _reader?.Dispose();
            _writer?.Dispose();
            _stream?.Close();
            _client?.Close();

            _reader = null;
            _writer = null;
            _stream = null;
            _client = null;

            OnDisconnected?.Invoke();
        }
    }
}