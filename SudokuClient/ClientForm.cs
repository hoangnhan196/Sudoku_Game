using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuClient.Network;
using SudokuClient.Models;
using System.Collections.Generic;

namespace SudokuClient
{
    public partial class ClientForm : Form
    {
        private NetworkClient _network = new NetworkClient();
        private Button[,] _cells = new Button[9, 9];
        private int _selectedRow = -1;
        private int _selectedCol = -1;
        private bool _isReady = false;
        private bool _isOffline = false;
        private SudokuClient.GameLogic.SudokuEngine? _offlineEngine;
        private System.Windows.Forms.Timer _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        private DateTime _gameStartTime;
        private int _penaltySeconds = 0;
        private bool _moveCooldown = false;
        private System.Windows.Forms.Timer _cooldownTimer = new System.Windows.Forms.Timer { Interval = 500 };

        public ClientForm()
        {
            InitializeComponent();
            InitializeBoard();

            _network.OnConnected += Network_OnConnected;
            _network.OnMessageReceived += Network_OnMessageReceived;
            _network.OnDisconnected += Network_OnDisconnected;
            _gameTimer.Tick += GameTimer_Tick;
            _cooldownTimer.Tick += (s, ev) => { _moveCooldown = false; _cooldownTimer.Stop(); };
        }

        private void InitializeBoard()
        {
            int cellSize = 50;
            pnlBoard.Size = new Size(9 * cellSize + 2, 9 * cellSize + 2); // Adjust for borders

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(cellSize, cellSize),
                        Location = new Point(c * cellSize, r * cellSize),
                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Tag = new Point(r, c)
                    };

                    // Thicker borders for 3x3 grids (simulated via margin or just flatstyle)
                    btn.FlatAppearance.BorderColor = Color.Gray;
                    if (c % 3 == 0) btn.Location = new Point(btn.Location.X + 2, btn.Location.Y);
                    if (r % 3 == 0) btn.Location = new Point(btn.Location.X, btn.Location.Y + 2);

                    btn.Click += Cell_Click;
                    btn.KeyPress += Cell_KeyPress;

                    _cells[r, c] = btn;
                    pnlBoard.Controls.Add(btn);
                }
            }
        }

        private void Cell_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Point pt)
            {
                // Unhighlight old
                if (_selectedRow != -1 && _selectedCol != -1)
                {
                    _cells[_selectedRow, _selectedCol].BackColor = _cells[_selectedRow, _selectedCol].ForeColor == Color.Black ? Color.White : Color.LightYellow;
                }

                _selectedRow = pt.X;
                _selectedCol = pt.Y;
                btn.BackColor = Color.LightBlue;
                btn.Focus();
            }
        }

        private async void Cell_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (_selectedRow == -1 || _selectedCol == -1) return;
            if (_moveCooldown) return; // Cooldown 0.5s chưa hết
            if (char.IsDigit(e.KeyChar) && e.KeyChar != '0')
            {
                int val = int.Parse(e.KeyChar.ToString());

                // Bắt đầu cooldown 0.5s
                _moveCooldown = true;
                _cooldownTimer.Start();

                if (_isOffline && _offlineEngine != null)
                {
                    HandleOfflineMove(_selectedRow, _selectedCol, val);
                }
                else
                {
                    var msg = new NetworkMessage
                    {
                        Type = "CLIENT_MOVE",
                        Row = _selectedRow,
                        Col = _selectedCol,
                        Value = val
                    };
                    await _network.SendAsync(msg);
                }
            }
        }

        private void HandleOfflineMove(int r, int c, int val)
        {
            if (_offlineEngine == null) return;

            bool isCorrect = _offlineEngine.CheckMove(r, c, val);
            if (isCorrect)
            {
                _offlineEngine.ApplyMove(r, c, val);
                _cells[r, c].Text = val.ToString();
                _cells[r, c].Enabled = false;
                _cells[r, c].BackColor = Color.LightGreen;
                txtChatLog.AppendText($"[Offline] Bạn điền ĐÚNG {val} ở ({r},{c})\n");

                if (_offlineEngine.IsGameFinished())
                {
                    _gameTimer.Stop();
                    var elapsed = DateTime.Now - _gameStartTime;
                    int totalSec = (int)elapsed.TotalSeconds + _penaltySeconds;
                    int m = totalSec / 60;
                    int s = totalSec % 60;
                    MessageBox.Show($"Chúc mừng! Bạn đã hoàn thành!\nThời gian: {m:D2}:{s:D2} (phạt +{_penaltySeconds}s)", "Thắng cuộc");
                    panelGame.Visible = false;
                    panelLogin.Visible = true;
                    _isOffline = false;
                }
            }
            else
            {
                _penaltySeconds += 15;
                txtChatLog.AppendText($"[Offline] Bạn điền SAI {val} ở ({r},{c}) — +15s phạt! (Tổng phạt: {_penaltySeconds}s)\n");
                var btn = _cells[r, c];
                btn.BackColor = Color.LightCoral;
                var flashTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                flashTimer.Tick += (s2, ev) => { btn.BackColor = Color.White; flashTimer.Stop(); flashTimer.Dispose(); };
                flashTimer.Start();
            }
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _gameStartTime;
            int totalSec = (int)elapsed.TotalSeconds + _penaltySeconds;
            int m = totalSec / 60;
            int s = totalSec % 60;
            int rawMin = (int)elapsed.TotalSeconds / 60;
            int rawSec = (int)elapsed.TotalSeconds % 60;
            string penaltyStr = _penaltySeconds > 0 ? $" (+{_penaltySeconds}s phạt)" : "";
            string mode = _isOffline ? "Offline" : "Online";
            lblGameStatus.Text = $"⏱ {rawMin:D2}:{rawSec:D2}{penaltyStr} — {mode}";
        }

        private async void Network_OnConnected()
        {
            Invoke((Action)(async () =>
            {
                lblStatus.Text = "Đã kết nối! Đang vào sảnh...";
                var msg = new NetworkMessage
                {
                    Type = "CLIENT_CONNECT",
                    Username = txtName.Text
                };
                await _network.SendAsync(msg);
            }));
        }

        private void Network_OnDisconnected()
        {
            Invoke((Action)(() =>
            {
                lblStatus.Text = "Mất kết nối từ Server!";
                panelLogin.Visible = true;
                panelLobby.Visible = false;
                panelGame.Visible = false;
                btnConnect.Enabled = true;
            }));
        }

        private void Network_OnMessageReceived(NetworkMessage msg)
        {
            Invoke((Action)(() =>
            {
                switch (msg.Type)
                {
                    case "SERVER_CONNECT_RESPONSE":
                        if (msg.Success == true)
                        {
                            panelLogin.Visible = false;
                            panelLobby.Visible = true;
                            panelGame.Visible = false;

                            // Ask for room list
                            _network.SendAsync(new NetworkMessage { Type = "CLIENT_GET_ROOMS" });
                        }
                        else
                        {
                            lblStatus.Text = msg.Message;
                            btnConnect.Enabled = true;
                            _network.Disconnect();
                        }
                        break;

                    case "SERVER_PLAYER_LIST":
                        lstLobbyPlayers.Items.Clear();
                        lstGamePlayers.Items.Clear();
                        if (msg.Players != null)
                        {
                            foreach (var p in msg.Players)
                            {
                                string status = p.IsReady ? "Sẵn sàng" : "Chờ";
                                string penaltyInfo = p.PenaltySeconds > 0 ? $" | phạt +{p.PenaltySeconds}s" : "";
                                string display = $"{p.Username} ({status}){penaltyInfo}";
                                lstLobbyPlayers.Items.Add(display);
                                lstGamePlayers.Items.Add(display);
                            }
                        }
                        break;

                    case "SERVER_ROOM_LIST":
                        lstRooms.Items.Clear();
                        if (msg.Rooms != null)
                        {
                            foreach (var r in msg.Rooms)
                            {
                                string st = r.IsGameActive ? "Đang chơi" : "Đang chờ";
                                lstRooms.Items.Add($"{r.Id} - {r.Name} ({r.PlayerCount} players) [{st}]");
                            }
                        }
                        break;

                    case "SERVER_JOIN_ROOM_RESPONSE":
                        if (msg.Success == true)
                        {
                            MessageBox.Show(msg.Message, "Thông báo");
                            // Cập nhật lại danh sách phòng
                            _network.SendAsync(new NetworkMessage { Type = "CLIENT_GET_ROOMS" });
                        }
                        else
                        {
                            MessageBox.Show(msg.Message, "Lỗi");
                        }
                        break;

                    case "SERVER_LEAVE_ROOM_RESPONSE":
                        panelGame.Visible = false;
                        panelLobby.Visible = true;
                        _network.SendAsync(new NetworkMessage { Type = "CLIENT_GET_ROOMS" });
                        break;

                    case "SERVER_START_GAME":
                        panelLobby.Visible = false;
                        panelGame.Visible = true;
                        _penaltySeconds = 0;
                        _gameStartTime = DateTime.Now;
                        _gameTimer.Start();
                        lblGameStatus.Text = "⏱ 00:00 — Điền số bằng phím 1-9";
                        txtChatLog.Clear();

                        if (msg.Board != null)
                        {
                            for (int r = 0; r < 9; r++)
                            {
                                for (int c = 0; c < 9; c++)
                                {
                                    int val = msg.Board[r][c];
                                    _cells[r, c].Text = val == 0 ? "" : val.ToString();
                                    _cells[r, c].Enabled = val == 0;
                                    _cells[r, c].ForeColor = val == 0 ? Color.Blue : Color.Black;
                                    _cells[r, c].BackColor = Color.White;
                                }
                            }
                        }
                        break;

                    case "SERVER_MOVE_UPDATE":
                        if (msg.Row.HasValue && msg.Col.HasValue && msg.Value.HasValue)
                        {
                            int r = msg.Row.Value;
                            int c = msg.Col.Value;
                            if (msg.Correct == true)
                            {
                                _cells[r, c].Text = msg.Value.Value.ToString();
                                _cells[r, c].Enabled = false;
                                _cells[r, c].BackColor = Color.LightGreen;
                                txtChatLog.AppendText($"[✓] {msg.Username} điền đúng {msg.Value.Value} ở ({r},{c})\n");
                            }
                            else
                            {
                                int penaltySec = msg.Score ?? 0;
                                txtChatLog.AppendText($"[✗] {msg.Username} điền sai ở ({r},{c}) — +15s phạt (tổng: {penaltySec}s)\n");
                                var btn = _cells[r, c];
                                btn.BackColor = Color.LightCoral;
                                var flashTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                                flashTimer.Tick += (s, e) => { btn.BackColor = Color.White; flashTimer.Stop(); flashTimer.Dispose(); };
                                flashTimer.Start();
                            }
                        }
                        break;

                    case "SERVER_GAME_OVER":
                        _gameTimer.Stop();
                        lblGameStatus.Text = msg.Message;
                        MessageBox.Show(msg.Message, "Kết thúc Game");
                        panelGame.Visible = false;
                        panelLobby.Visible = true;
                        _isReady = false;
                        break;

                    case "SERVER_CHAT":
                        txtChatLog.AppendText($"{msg.Username}: {msg.ChatText}\n");
                        break;

                    case "SERVER_COUNTDOWN":
                        panelLobby.Visible = false;
                        panelGame.Visible = true;
                        lblGameStatus.Text = msg.Message ?? "Chuẩn bị...";
                        // Disable board during countdown
                        for (int r = 0; r < 9; r++)
                            for (int c = 0; c < 9; c++)
                                _cells[r, c].Enabled = false;
                        break;
                }
            }));
        }
        private async void btnDiscover_Click(object sender, EventArgs e)
        {
            btnDiscover.Enabled = false;
            btnDiscover.Text = "⏳ Đang tìm...";
            lblStatus.Text = "Đang tìm server trên mạng LAN...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;

            try
            {
                var result = await NetworkClient.DiscoverServerAsync(3000);
                if (result != null)
                {
                    txtIP.Text = result.ServerIP;
                    txtPort.Text = result.ServerPort.ToString();
                    lblStatus.Text = $"✅ Tìm thấy server: {result.ServerIP}:{result.ServerPort}";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblStatus.Text = "❌ Không tìm thấy server nào trên LAN!";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Lỗi: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                btnDiscover.Enabled = true;
                btnDiscover.Text = "🔍 Tìm Server LAN";
            }
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nhập tên đi bạn ơi!");
                return;
            }

            lblStatus.Text = "Đang kết nối...";
            btnConnect.Enabled = false;

            if (!int.TryParse(txtPort.Text, out int port)) port = 9999;
            bool ok = _network.Connect(txtIP.Text, port);

            if (!ok)
            {
                lblStatus.Text = "Không kết nối được!";
                btnConnect.Enabled = true;
            }
        }

        private async void btnReady_Click(object sender, EventArgs e)
        {
            _isReady = !_isReady;
            await _network.SendAsync(new NetworkMessage { Type = "CLIENT_READY", IsReady = _isReady });
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            await _network.SendAsync(new NetworkMessage { Type = "CLIENT_CREATE_ROOM", RoomName = txtName.Text + "'s Room" });
        }

        private async void btnJoinRoom_Click(object sender, EventArgs e)
        {
            if (lstRooms.SelectedItem != null)
            {
                string text = lstRooms.SelectedItem.ToString() ?? "";
                string roomId = text.Split('-')[0].Trim();
                await _network.SendAsync(new NetworkMessage { Type = "CLIENT_JOIN_ROOM", RoomId = roomId });
            }
        }

        private async void btnLeaveRoom_Click(object sender, EventArgs e)
        {
            await _network.SendAsync(new NetworkMessage { Type = "CLIENT_LEAVE_ROOM" });
        }

        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            int cellsToRemove = 42; // Vừa (Medium)
            int index = cmbDifficulty.SelectedIndex;
            if (index == 0) cellsToRemove = 30; // Easy
            else if (index == 1) cellsToRemove = 42; // Medium
            else if (index == 2) cellsToRemove = 52; // Hard
            else if (index == 3) cellsToRemove = 62; // Expert
            else if (index == 4) cellsToRemove = 72; // Evil
            else if (index == 5) cellsToRemove = 80; // Master

            await _network.SendAsync(new NetworkMessage { Type = "CLIENT_START_GAME", Value = cellsToRemove });
        }

        private void btnOffline_Click(object sender, EventArgs e)
        {
            _isOffline = true;
            _penaltySeconds = 0;
            panelLogin.Visible = false;
            panelLobby.Visible = false;
            panelGame.Visible = true;
            txtChatLog.Clear();
            txtChatLog.AppendText("Khởi động chế độ chơi đơn...\n");

            _offlineEngine = new SudokuClient.GameLogic.SudokuEngine();
            _offlineEngine.GenerateNewGame(42);

            _gameStartTime = DateTime.Now;
            _gameTimer.Start();
            lblGameStatus.Text = "⏱ 00:00 — Chế độ Offline";

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int val = _offlineEngine.PlayerBoard[r, c];
                    _cells[r, c].Text = val == 0 ? "" : val.ToString();
                    _cells[r, c].Enabled = val == 0;
                    _cells[r, c].ForeColor = val == 0 ? Color.Blue : Color.Black;
                    _cells[r, c].BackColor = Color.White;
                }
            }
        }

        private async void btnSendChat_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtChatInput.Text))
            {
                await _network.SendAsync(new NetworkMessage { Type = "CLIENT_CHAT", ChatText = txtChatInput.Text });
                txtChatInput.Clear();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _network.Disconnect();
            base.OnFormClosing(e);
        }

        private async void btnExitGame_Click(object sender, EventArgs e)
        {
            _gameTimer.Stop();
            txtChatLog.Clear();
            if (_isOffline)
            {
                _isOffline = false;
                panelGame.Visible = false;
                panelLogin.Visible = true;
            }
            else
            {
                await _network.SendAsync(new NetworkMessage { Type = "CLIENT_LEAVE_ROOM" });
                panelGame.Visible = false;
                panelLobby.Visible = true;
                await _network.SendAsync(new NetworkMessage { Type = "CLIENT_GET_ROOMS" });
            }
        }
    }
}