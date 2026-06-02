using System;
using System.Drawing;
using System.Windows.Forms;
using SudokuServer.Network;

namespace SudokuServer
{
    public partial class SudokuServer : Form
    {
        private SocketManager? _server;

        public SudokuServer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _server = new SocketManager();
            _server.OnLog += LogToConsole;
            _server.OnClientListChanged += UpdateClientList;
            _server.OnGameStateChanged += UpdateGameState;

            cmbDifficulty.SelectedIndex = 1; // Medium by default
            UpdateUIState(false);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_server == null) return;

            if (int.TryParse(txtPort.Text, out int port))
            {
                try
                {
                    _server.Start(port);
                    UpdateUIState(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid port number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_server == null) return;

            _server.Stop();
            UpdateUIState(false);
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            if (_server == null || !_server.IsRunning) return;

            int cellsToRemove = 40; // Default Medium
            if (cmbDifficulty.SelectedIndex == 0) cellsToRemove = 30; // Easy
            else if (cmbDifficulty.SelectedIndex == 2) cellsToRemove = 50; // Hard

            _server.StartGame(cellsToRemove);
        }

        private void SudokuServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_server != null && _server.IsRunning)
            {
                _server.Stop();
            }
        }

        private void LogToConsole(string message)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(LogToConsole), message);
                return;
            }

            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void UpdateClientList()
        {
            if (lstClients.InvokeRequired || lblClients.InvokeRequired)
            {
                Invoke(new Action(UpdateClientList));
                return;
            }

            if (_server == null) return;

            lstClients.Items.Clear();
            var clients = _server.GetClientsList();
            foreach (var c in clients)
            {
                string status = c.IsReady ? "READY" : "WAITING";
                lstClients.Items.Add($"{c.Username} ({c.Score} pts) - {status}");
            }

            lblClients.Text = $"Connected Players ({clients.Count})";
        }

        private void UpdateGameState()
        {
            if (pnlBoard.InvokeRequired)
            {
                pnlBoard.Invoke(new Action(UpdateGameState));
                return;
            }

            pnlBoard.Invalidate(); // Redraw the board
        }

        private void UpdateUIState(bool isRunning)
        {
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
            btnStartGame.Enabled = isRunning;
            txtPort.Enabled = !isRunning;
            cmbDifficulty.Enabled = !isRunning;

            if (isRunning)
            {
                lblStatus.Text = "Status: Running";
                lblStatus.ForeColor = Color.FromArgb(40, 167, 69); // Green
            }
            else
            {
                lblStatus.Text = "Status: Stopped";
                lblStatus.ForeColor = Color.FromArgb(220, 53, 69); // Red
            }
        }

        private void pnlBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int width = pnlBoard.Width;
            int height = pnlBoard.Height;
            float cellWidth = (float)width / 9;
            float cellHeight = (float)height / 9;

            // Draw grid background
            g.Clear(Color.FromArgb(30, 30, 40));

            // Draw thin cell gridlines
            using (Pen thinPen = new Pen(Color.FromArgb(50, 50, 65), 1))
            {
                for (int i = 1; i < 9; i++)
                {
                    // vertical line
                    g.DrawLine(thinPen, i * cellWidth, 0, i * cellWidth, height);
                    // horizontal line
                    g.DrawLine(thinPen, 0, i * cellHeight, width, i * cellHeight);
                }
            }

            // Draw thick 3x3 block borders and outer border
            using (Pen thickPen = new Pen(Color.FromArgb(120, 120, 140), 3))
            {
                // Vertical block lines
                g.DrawLine(thickPen, 3 * cellWidth, 0, 3 * cellWidth, height);
                g.DrawLine(thickPen, 6 * cellWidth, 0, 6 * cellWidth, height);

                // Horizontal block lines
                g.DrawLine(thickPen, 0, 3 * cellHeight, width, 3 * cellHeight);
                g.DrawLine(thickPen, 0, 6 * cellHeight, width, 6 * cellHeight);

                // Outer border
                g.DrawRectangle(thickPen, 0, 0, width - 1, height - 1);
            }

            if (_server == null || !_server.IsGameActive)
            {
                // Draw "No Active Game" text
                string emptyMsg = "No Active Game";
                using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.FromArgb(100, 100, 120)))
                {
                    SizeF size = g.MeasureString(emptyMsg, font);
                    g.DrawString(emptyMsg, font, brush, (width - size.Width) / 2, (height - size.Height) / 2);
                }
                return;
            }

            // Draw numbers
            int[,] board = _server.GetPlayerBoard();
            using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(240, 240, 250)))
            {
                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        int val = board[r, c];
                        if (val != 0)
                        {
                            string valStr = val.ToString();
                            SizeF size = g.MeasureString(valStr, font);
                            float posX = c * cellWidth + (cellWidth - size.Width) / 2;
                            float posY = r * cellHeight + (cellHeight - size.Height) / 2;
                            g.DrawString(valStr, font, textBrush, posX, posY);
                        }
                    }
                }
            }
        }
    }
}
