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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbDifficulty = new System.Windows.Forms.ComboBox();
            this.lblDifficulty = new System.Windows.Forms.Label();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lstClients = new System.Windows.Forms.ListBox();
            this.lblClients = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlBoard = new System.Windows.Forms.Panel();
            this.lblBoard = new System.Windows.Forms.Label();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.lblLog = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.pnlTop.Controls.Add(this.lblStatus);
            this.pnlTop.Controls.Add(this.cmbDifficulty);
            this.pnlTop.Controls.Add(this.lblDifficulty);
            this.pnlTop.Controls.Add(this.btnStartGame);
            this.pnlTop.Controls.Add(this.btnStop);
            this.pnlTop.Controls.Add(this.btnStart);
            this.pnlTop.Controls.Add(this.txtPort);
            this.pnlTop.Controls.Add(this.lblPort);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1008, 70);
            this.pnlTop.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblStatus.Location = new System.Drawing.Point(780, 25);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(126, 20);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Status: Stopped";
            // 
            // cmbDifficulty
            // 
            this.cmbDifficulty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.cmbDifficulty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDifficulty.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbDifficulty.ForeColor = System.Drawing.Color.White;
            this.cmbDifficulty.FormattingEnabled = true;
            this.cmbDifficulty.Items.AddRange(new object[] {
            "Easy (30 blank cells)",
            "Medium (40 blank cells)",
            "Hard (50 blank cells)"});
            this.cmbDifficulty.Location = new System.Drawing.Point(460, 22);
            this.cmbDifficulty.Name = "cmbDifficulty";
            this.cmbDifficulty.Size = new System.Drawing.Size(160, 25);
            this.cmbDifficulty.TabIndex = 6;
            // 
            // lblDifficulty
            // 
            this.lblDifficulty.AutoSize = true;
            this.lblDifficulty.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDifficulty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.lblDifficulty.Location = new System.Drawing.Point(388, 25);
            this.lblDifficulty.Name = "lblDifficulty";
            this.lblDifficulty.Size = new System.Drawing.Size(66, 19);
            this.lblDifficulty.TabIndex = 5;
            this.lblDifficulty.Text = "Difficulty:";
            // 
            // btnStartGame
            // 
            this.btnStartGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(92)))), ((int)(((byte)(231)))));
            this.btnStartGame.Enabled = false;
            this.btnStartGame.FlatAppearance.BorderSize = 0;
            this.btnStartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartGame.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStartGame.ForeColor = System.Drawing.Color.White;
            this.btnStartGame.Location = new System.Drawing.Point(630, 20);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(120, 30);
            this.btnStartGame.TabIndex = 4;
            this.btnStartGame.Text = "Start Game";
            this.btnStartGame.UseVisualStyleBackColor = false;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(260, 20);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(160, 20);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(90, 30);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start Server";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtPort
            // 
            this.txtPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(56)))));
            this.txtPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPort.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtPort.ForeColor = System.Drawing.Color.White;
            this.txtPort.Location = new System.Drawing.Point(60, 22);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(80, 25);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "8888";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.lblPort.Location = new System.Drawing.Point(20, 25);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(37, 19);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port:";
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(28)))));
            this.pnlLeft.Controls.Add(this.lstClients);
            this.pnlLeft.Controls.Add(this.lblClients);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 70);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(15);
            this.pnlLeft.Size = new System.Drawing.Size(250, 491);
            this.pnlLeft.TabIndex = 1;
            // 
            // lstClients
            // 
            this.lstClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.lstClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClients.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstClients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
            this.lstClients.FormattingEnabled = true;
            this.lstClients.ItemHeight = 17;
            this.lstClients.Location = new System.Drawing.Point(15, 45);
            this.lstClients.Name = "lstClients";
            this.lstClients.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstClients.Size = new System.Drawing.Size(220, 431);
            this.lstClients.TabIndex = 1;
            // 
            // lblClients
            // 
            this.lblClients.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblClients.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblClients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(160)))));
            this.lblClients.Location = new System.Drawing.Point(15, 15);
            this.lblClients.Name = "lblClients";
            this.lblClients.Size = new System.Drawing.Size(220, 30);
            this.lblClients.TabIndex = 0;
            this.lblClients.Text = "Connected Players (0)";
            this.lblClients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(28)))));
            this.pnlRight.Controls.Add(this.pnlBoard);
            this.pnlRight.Controls.Add(this.lblBoard);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(628, 70);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(15);
            this.pnlRight.Size = new System.Drawing.Size(380, 491);
            this.pnlRight.TabIndex = 2;
            // 
            // pnlBoard
            // 
            this.pnlBoard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.pnlBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBoard.Location = new System.Drawing.Point(15, 45);
            this.pnlBoard.Name = "pnlBoard";
            this.pnlBoard.Size = new System.Drawing.Size(350, 350);
            this.pnlBoard.TabIndex = 1;
            this.pnlBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBoard_Paint);
            // 
            // lblBoard
            // 
            this.lblBoard.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBoard.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBoard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(160)))));
            this.lblBoard.Location = new System.Drawing.Point(15, 15);
            this.lblBoard.Name = "lblBoard";
            this.lblBoard.Size = new System.Drawing.Size(350, 30);
            this.lblBoard.TabIndex = 0;
            this.lblBoard.Text = "Sudoku Board Status Preview";
            this.lblBoard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(24)))));
            this.pnlCenter.Controls.Add(this.rtbLog);
            this.pnlCenter.Controls.Add(this.lblLog);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(250, 70);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(15);
            this.pnlCenter.Size = new System.Drawing.Size(378, 491);
            this.pnlCenter.TabIndex = 3;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
            this.rtbLog.Location = new System.Drawing.Point(15, 45);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(348, 431);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // lblLog
            // 
            this.lblLog.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(160)))));
            this.lblLog.Location = new System.Drawing.Point(15, 15);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(348, 30);
            this.lblLog.TabIndex = 0;
            this.lblLog.Text = "Server Activity Log";
            this.lblLog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SudokuServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlTop);
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "SudokuServer";
            this.Text = "Multiplayer Sudoku Server Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SudokuServer_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            this.ResumeLayout(false);

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
