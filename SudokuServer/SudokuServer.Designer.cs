namespace SudokuServer
{
    partial class SudokuServer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlTop = new Panel();
            lblStatus = new Label();
            cmbDifficulty = new ComboBox();
            lblDifficulty = new Label();
            btnStartGame = new Button();
            btnStop = new Button();
            btnStart = new Button();
            txtPort = new TextBox();
            lblPort = new Label();
            pnlLeft = new Panel();
            lstClients = new ListBox();
            lblClients = new Label();
            pnlRight = new Panel();
            pnlBoard = new Panel();
            lblBoard = new Label();
            pnlCenter = new Panel();
            rtbLog = new RichTextBox();
            lblLog = new Label();
            pnlTop.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            pnlCenter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(28, 28, 36);
            pnlTop.Controls.Add(lblStatus);
            pnlTop.Controls.Add(cmbDifficulty);
            pnlTop.Controls.Add(lblDifficulty);
            pnlTop.Controls.Add(btnStartGame);
            pnlTop.Controls.Add(btnStop);
            pnlTop.Controls.Add(btnStart);
            pnlTop.Controls.Add(txtPort);
            pnlTop.Controls.Add(lblPort);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1008, 70);
            pnlTop.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
            lblStatus.Location = new Point(780, 25);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(119, 20);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Status: Stopped";
            // 
            // cmbDifficulty
            // 
            cmbDifficulty.BackColor = Color.FromArgb(45, 55, 60);
            cmbDifficulty.FlatStyle = FlatStyle.Flat;
            cmbDifficulty.Font = new Font("Segoe UI", 10F);
            cmbDifficulty.ForeColor = Color.White;
            cmbDifficulty.FormattingEnabled = true;
            cmbDifficulty.Items.AddRange(new object[] { "Easy (30 blank cells)", "Medium (42 blank cells)", "Hard (52 blank cells)", "Expert (62 blank cells)", "Evil (72 blank cells)" });
            cmbDifficulty.Location = new Point(460, 22);
            cmbDifficulty.Name = "cmbDifficulty";
            cmbDifficulty.Size = new Size(160, 25);
            cmbDifficulty.TabIndex = 6;
            // 
            // lblDifficulty
            // 
            lblDifficulty.AutoSize = true;
            lblDifficulty.Font = new Font("Segoe UI", 10F);
            lblDifficulty.ForeColor = Color.FromArgb(200, 200, 210);
            lblDifficulty.Location = new Point(388, 25);
            lblDifficulty.Name = "lblDifficulty";
            lblDifficulty.Size = new Size(65, 19);
            lblDifficulty.TabIndex = 5;
            lblDifficulty.Text = "Difficulty:";
            // 
            // btnStartGame
            // 
            btnStartGame.BackColor = Color.FromArgb(108, 92, 231);
            btnStartGame.Enabled = false;
            btnStartGame.FlatAppearance.BorderSize = 0;
            btnStartGame.FlatStyle = FlatStyle.Flat;
            btnStartGame.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStartGame.ForeColor = Color.White;
            btnStartGame.Location = new Point(630, 20);
            btnStartGame.Name = "btnStartGame";
            btnStartGame.Size = new Size(120, 30);
            btnStartGame.TabIndex = 4;
            btnStartGame.Text = "Start Game";
            btnStartGame.UseVisualStyleBackColor = false;
            btnStartGame.Click += btnStartGame_Click;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.FromArgb(220, 53, 69);
            btnStop.Enabled = false;
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStop.ForeColor = Color.White;
            btnStop.Location = new Point(260, 20);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(90, 30);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.FromArgb(40, 167, 69);
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStart.ForeColor = Color.White;
            btnStart.Location = new Point(160, 20);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(90, 30);
            btnStart.TabIndex = 2;
            btnStart.Text = "Start Server";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // txtPort
            // 
            txtPort.BackColor = Color.FromArgb(45, 45, 56);
            txtPort.BorderStyle = BorderStyle.FixedSingle;
            txtPort.Font = new Font("Segoe UI", 10F);
            txtPort.ForeColor = Color.White;
            txtPort.Location = new Point(60, 22);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(80, 25);
            txtPort.TabIndex = 1;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new Font("Segoe UI", 10F);
            lblPort.ForeColor = Color.FromArgb(200, 200, 210);
            lblPort.Location = new Point(20, 25);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(37, 19);
            lblPort.TabIndex = 0;
            lblPort.Text = "Port:";
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(21, 21, 28);
            pnlLeft.Controls.Add(lstClients);
            pnlLeft.Controls.Add(lblClients);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 70);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(15);
            pnlLeft.Size = new Size(250, 491);
            pnlLeft.TabIndex = 1;
            // 
            // lstClients
            // 
            lstClients.BackColor = Color.FromArgb(30, 30, 40);
            lstClients.BorderStyle = BorderStyle.None;
            lstClients.Dock = DockStyle.Fill;
            lstClients.Font = new Font("Segoe UI", 10F);
            lstClients.ForeColor = Color.FromArgb(230, 230, 240);
            lstClients.FormattingEnabled = true;
            lstClients.ItemHeight = 17;
            lstClients.Location = new Point(15, 45);
            lstClients.Name = "lstClients";
            lstClients.SelectionMode = SelectionMode.None;
            lstClients.Size = new Size(220, 431);
            lstClients.TabIndex = 1;
            // 
            // lblClients
            // 
            lblClients.Dock = DockStyle.Top;
            lblClients.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblClients.ForeColor = Color.FromArgb(150, 150, 160);
            lblClients.Location = new Point(15, 15);
            lblClients.Name = "lblClients";
            lblClients.Size = new Size(220, 30);
            lblClients.TabIndex = 0;
            lblClients.Text = "Connected Players (0)";
            lblClients.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(21, 21, 28);
            pnlRight.Controls.Add(pnlBoard);
            pnlRight.Controls.Add(lblBoard);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Location = new Point(628, 70);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(15);
            pnlRight.Size = new Size(380, 491);
            pnlRight.TabIndex = 2;
            // 
            // pnlBoard
            // 
            pnlBoard.BackColor = Color.FromArgb(30, 30, 40);
            pnlBoard.BorderStyle = BorderStyle.FixedSingle;
            pnlBoard.Location = new Point(15, 45);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(350, 350);
            pnlBoard.TabIndex = 1;
            pnlBoard.Paint += pnlBoard_Paint;
            // 
            // lblBoard
            // 
            lblBoard.Dock = DockStyle.Top;
            lblBoard.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBoard.ForeColor = Color.FromArgb(150, 150, 160);
            lblBoard.Location = new Point(15, 15);
            lblBoard.Name = "lblBoard";
            lblBoard.Size = new Size(350, 30);
            lblBoard.TabIndex = 0;
            lblBoard.Text = "Sudoku Board Status Preview";
            lblBoard.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlCenter
            // 
            pnlCenter.BackColor = Color.FromArgb(18, 18, 24);
            pnlCenter.Controls.Add(rtbLog);
            pnlCenter.Controls.Add(lblLog);
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.Location = new Point(250, 70);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Padding = new Padding(15);
            pnlCenter.Size = new Size(378, 491);
            pnlCenter.TabIndex = 3;
            pnlCenter.Paint += pnlCenter_Paint;
            // 
            // rtbLog
            // 
            rtbLog.BackColor = Color.FromArgb(30, 30, 40);
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Font = new Font("Consolas", 9.75F);
            rtbLog.ForeColor = Color.FromArgb(220, 220, 230);
            rtbLog.Location = new Point(15, 45);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(348, 431);
            rtbLog.TabIndex = 1;
            rtbLog.Text = "";
            // 
            // lblLog
            // 
            lblLog.Dock = DockStyle.Top;
            lblLog.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblLog.ForeColor = Color.FromArgb(150, 150, 160);
            lblLog.Location = new Point(15, 15);
            lblLog.Name = "lblLog";
            lblLog.Size = new Size(348, 30);
            lblLog.TabIndex = 0;
            lblLog.Text = "Server Activity Log";
            lblLog.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SudokuServer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(18, 18, 24);
            ClientSize = new Size(1008, 561);
            Controls.Add(pnlCenter);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(pnlTop);
            MinimumSize = new Size(1024, 600);
            Name = "SudokuServer";
            Text = "Multiplayer Sudoku Server Dashboard";
            FormClosing += SudokuServer_FormClosing;
            Load += Form1_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlLeft.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlCenter.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.ComboBox cmbDifficulty;
        private System.Windows.Forms.Label lblDifficulty;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Label lblClients;
        private System.Windows.Forms.ListBox lstClients;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblBoard;
        private System.Windows.Forms.Panel pnlBoard;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Label lblLog;
    }
}
