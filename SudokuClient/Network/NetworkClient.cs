using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SudokuClient.Models;

namespace SudokuClient.Network
{
    public class NetworkClient
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private CancellationTokenSource? _cts;
        private readonly object _lock = new object(); // ✅ tránh race condition

        private int _port = 9999;

        // ── Events ──────────────────────────────────────────────
        public event Action<NetworkMessage>? OnMessageReceived;
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        // ── State ────────────────────────────────────────────────
        public bool IsConnected => _client?.Connected ?? false;

        // ── Connect ──────────────────────────────────────────────
        public bool Connect(string host, int port = 9999)
        {
            try
            {
                _port = port;
                _client = new TcpClient();
                _client.ConnectAsync(host, _port).Wait(5000); // ✅ timeout 5 giây

                if (!_client.Connected)
                {
                    Console.WriteLine("Connect timeout.");
                    return false;
                }

                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
                _cts = new CancellationTokenSource();

                OnConnected?.Invoke();

                // Bắt đầu lắng nghe server trên background thread
                Task.Run(() => ListenFromServerAsync(_cts.Token));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect failed: " + ex.Message);
                return false;
            }
        }

        // ── Send ─────────────────────────────────────────────────
        public async Task SendAsync(NetworkMessage message)
        {
            if (!IsConnected || _writer == null) return;

            try
            {
                string json = JsonSerializer.Serialize(message);
                await _writer.WriteLineAsync(json); // ✅ WriteLine để server ReadLine được
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send failed: " + ex.Message);
                Disconnect();
            }
        }

        // Overload đồng bộ tiện dùng trong UI (WinForms/WPF)
        public void Send(NetworkMessage message)
        {
            _ = SendAsync(message); // fire-and-forget
        }

        // ── Listen ───────────────────────────────────────────────
        private async Task ListenFromServerAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && _reader != null)
                {
                    string? line = await _reader.ReadLineAsync(); // ✅ tương thích .NET 6+
                    if (line == null) break; // server đóng kết nối

                    if (token.IsCancellationRequested) break;

                    try
                    {
                        var msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                        if (msg != null)
                        {
                            OnMessageReceived?.Invoke(msg);
                        }
                    }
                    catch (JsonException ex)
                    {
                        // Bỏ qua gói tin lỗi, tiếp tục nhận
                        Console.WriteLine("Bad JSON from server: " + ex.Message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Bình thường khi gọi Disconnect() → bỏ qua
            }
            catch (Exception ex)
            {
                Console.WriteLine("Listen error: " + ex.Message);
            }
            finally
            {
                Disconnect(); // ✅ đảm bảo luôn cleanup
            }
        }

        // ── Disconnect ───────────────────────────────────────────
        public void Disconnect()
        {
            lock (_lock) // ✅ chống gọi đồng thời từ nhiều thread
            {
                if (_client == null) return; // đã disconnect rồi

                try { _cts?.Cancel(); } catch { }

                try { _reader?.Dispose(); } catch { }
                try { _writer?.Dispose(); } catch { }
                try { _stream?.Close(); } catch { }
                try { _client?.Close(); } catch { }

                _reader = null;
                _writer = null;
                _stream = null;
                _client = null;
                _cts = null;
            }

            OnDisconnected?.Invoke(); // ✅ gọi ngoài lock để tránh deadlock
        }
    }
}