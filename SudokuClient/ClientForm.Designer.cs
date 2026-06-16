namespace SudokuClient
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;

        // Login Controls
        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnDiscover;

        // Lobby Controls
        private System.Windows.Forms.Panel panelLobby;
        private System.Windows.Forms.ListBox lstLobbyPlayers;
        private System.Windows.Forms.ListBox lstRooms;
        private System.Windows.Forms.Button btnReady;
        private System.Windows.Forms.Button btnCreateRoom;
        private System.Windows.Forms.Button btnJoinRoom;
        private System.Windows.Forms.Button btnLeaveRoom;
        private System.Windows.Forms.Label lblLobbyTitle;
        private System.Windows.Forms.Label lblRoomTitle;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.ComboBox cmbDifficulty;
        private System.Windows.Forms.Button btnOffline;

        // Game Controls
        private System.Windows.Forms.Panel panelGame;
        private System.Windows.Forms.Panel pnlBoard;
        private System.Windows.Forms.ListBox lstGamePlayers;
        private System.Windows.Forms.TextBox txtChatLog;
        private System.Windows.Forms.TextBox txtChatInput;
        private System.Windows.Forms.Button btnSendChat;
        private System.Windows.Forms.Label lblGameStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelLogin = new System.Windows.Forms.Panel();
            this.panelLobby = new System.Windows.Forms.Panel();
            this.panelGame = new System.Windows.Forms.Panel();
            this.pnlBoard = new System.Windows.Forms.Panel();
            
            // Instantiate Login
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();

            // Instantiate Lobby
            this.lstLobbyPlayers = new System.Windows.Forms.ListBox();
            this.lstRooms = new System.Windows.Forms.ListBox();
            this.btnReady = new System.Windows.Forms.Button();
            this.btnCreateRoom = new System.Windows.Forms.Button();
            this.btnJoinRoom = new System.Windows.Forms.Button();
            this.btnLeaveRoom = new System.Windows.Forms.Button();
            this.lblLobbyTitle = new System.Windows.Forms.Label();
            this.lblRoomTitle = new System.Windows.Forms.Label();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.cmbDifficulty = new System.Windows.Forms.ComboBox();
            this.btnOffline = new System.Windows.Forms.Button();
            this.btnDiscover = new System.Windows.Forms.Button();

            // Instantiate Game
            this.lstGamePlayers = new System.Windows.Forms.ListBox();
            this.txtChatLog = new System.Windows.Forms.TextBox();
            this.txtChatInput = new System.Windows.Forms.TextBox();
            this.btnSendChat = new System.Windows.Forms.Button();
            this.lblGameStatus = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // Form
            this.Text = "Sudoku Client";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.WhiteSmoke;

            // ================== LOGIN PANEL ==================
            this.panelLogin.Size = new System.Drawing.Size(400, 400);
            this.panelLogin.Location = new System.Drawing.Point(200, 80);
            this.panelLogin.BackColor = System.Drawing.Color.White;
            this.panelLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblTitle.Text = "🎮 Sudoku Online";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20, System.Drawing.FontStyle.Bold);
            this.lblTitle.Size = new System.Drawing.Size(400, 50);
            this.lblTitle.Location = new System.Drawing.Point(0, 20);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(33, 97, 140);

            this.lblName.Text = "Tên người chơi:";
            this.lblName.Location = new System.Drawing.Point(50, 100);
            this.lblName.Size = new System.Drawing.Size(100, 20);
            this.txtName.Location = new System.Drawing.Point(160, 97);
            this.txtName.Size = new System.Drawing.Size(180, 25);
            this.txtName.Text = "Player" + new System.Random().Next(1000, 9999);

            this.lblIP.Text = "Server IP:";
            this.lblIP.Location = new System.Drawing.Point(50, 140);
            this.lblIP.Size = new System.Drawing.Size(100, 20);
            this.txtIP.Location = new System.Drawing.Point(160, 137);
            this.txtIP.Size = new System.Drawing.Size(180, 25);
            this.txtIP.Text = "127.0.0.1";

            this.lblPort.Text = "Port:";
            this.lblPort.Location = new System.Drawing.Point(50, 180);
            this.lblPort.Size = new System.Drawing.Size(100, 20);
            this.txtPort.Location = new System.Drawing.Point(160, 177);
            this.txtPort.Size = new System.Drawing.Size(180, 25);
            this.txtPort.Text = "9999";

            this.btnConnect.Text = "Kết nối";
            this.btnConnect.Location = new System.Drawing.Point(160, 220);
            this.btnConnect.Size = new System.Drawing.Size(180, 40);
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(33, 97, 140);
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            this.lblStatus.Text = "";
            this.lblStatus.Location = new System.Drawing.Point(0, 280);
            this.lblStatus.Size = new System.Drawing.Size(400, 20);
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnOffline.Text = "Chơi Offline";
            this.btnOffline.Location = new System.Drawing.Point(160, 310);
            this.btnOffline.Size = new System.Drawing.Size(180, 30);
            this.btnOffline.BackColor = System.Drawing.Color.Gray;
            this.btnOffline.ForeColor = System.Drawing.Color.White;
            this.btnOffline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOffline.Click += new System.EventHandler(this.btnOffline_Click);

            this.panelLogin.Controls.Add(this.lblTitle);
            this.panelLogin.Controls.Add(this.lblName);
            this.panelLogin.Controls.Add(this.txtName);
            this.panelLogin.Controls.Add(this.lblIP);
            this.panelLogin.Controls.Add(this.txtIP);
            this.panelLogin.Controls.Add(this.lblPort);
            this.panelLogin.Controls.Add(this.txtPort);
            this.panelLogin.Controls.Add(this.btnConnect);
            this.panelLogin.Controls.Add(this.lblStatus);
            this.panelLogin.Controls.Add(this.btnOffline);

            this.btnDiscover.Text = "🔍 Tìm Server LAN";
            this.btnDiscover.Location = new System.Drawing.Point(50, 220);
            this.btnDiscover.Size = new System.Drawing.Size(130, 40);
            this.btnDiscover.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnDiscover.ForeColor = System.Drawing.Color.White;
            this.btnDiscover.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDiscover.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            this.btnDiscover.Click += new System.EventHandler(this.btnDiscover_Click);

            this.btnConnect.Location = new System.Drawing.Point(200, 220);
            this.btnConnect.Size = new System.Drawing.Size(140, 40);

            this.lblStatus.Location = new System.Drawing.Point(0, 280);

            this.btnOffline.Location = new System.Drawing.Point(110, 310);

            this.panelLogin.Controls.Add(this.btnDiscover);

            // ================== LOBBY PANEL ==================
            this.panelLobby.Size = new System.Drawing.Size(760, 560);
            this.panelLobby.Location = new System.Drawing.Point(20, 20);
            this.panelLobby.Visible = false;

            this.lblLobbyTitle.Text = "Sảnh chờ (Lobby)";
            this.lblLobbyTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            this.lblLobbyTitle.Location = new System.Drawing.Point(10, 10);
            this.lblLobbyTitle.Size = new System.Drawing.Size(300, 30);

            this.lstLobbyPlayers.Location = new System.Drawing.Point(10, 50);
            this.lstLobbyPlayers.Size = new System.Drawing.Size(350, 400);

            this.lblRoomTitle.Text = "Danh sách phòng";
            this.lblRoomTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            this.lblRoomTitle.Location = new System.Drawing.Point(400, 10);
            this.lblRoomTitle.Size = new System.Drawing.Size(300, 30);

            this.lstRooms.Location = new System.Drawing.Point(400, 50);
            this.lstRooms.Size = new System.Drawing.Size(350, 400);

            this.btnReady.Text = "Sẵn sàng (Lobby/Room)";
            this.btnReady.Location = new System.Drawing.Point(10, 470);
            this.btnReady.Size = new System.Drawing.Size(150, 40);
            this.btnReady.Click += new System.EventHandler(this.btnReady_Click);

            this.btnCreateRoom.Text = "Tạo phòng";
            this.btnCreateRoom.Location = new System.Drawing.Point(400, 470);
            this.btnCreateRoom.Size = new System.Drawing.Size(100, 40);
            this.btnCreateRoom.Click += new System.EventHandler(this.btnCreateRoom_Click);

            this.btnJoinRoom.Text = "Vào phòng";
            this.btnJoinRoom.Location = new System.Drawing.Point(520, 470);
            this.btnJoinRoom.Size = new System.Drawing.Size(100, 40);
            this.btnJoinRoom.Click += new System.EventHandler(this.btnJoinRoom_Click);

            this.btnLeaveRoom.Text = "Rời phòng";
            this.btnLeaveRoom.Location = new System.Drawing.Point(640, 470);
            this.btnLeaveRoom.Size = new System.Drawing.Size(100, 40);
            this.btnLeaveRoom.Click += new System.EventHandler(this.btnLeaveRoom_Click);

            this.cmbDifficulty.Items.AddRange(new object[] {
            "Dễ (Easy)",
            "Vừa (Medium)",
            "Khó (Hard)",
            "Siêu khó (Expert)",
            "Ác mộng (Evil)"});
            this.cmbDifficulty.Location = new System.Drawing.Point(400, 520);
            this.cmbDifficulty.Size = new System.Drawing.Size(150, 30);
            this.cmbDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDifficulty.SelectedIndex = 1; // Default to Medium

            this.btnStartGame.Text = "Bắt đầu Game";
            this.btnStartGame.Location = new System.Drawing.Point(560, 520);
            this.btnStartGame.Size = new System.Drawing.Size(180, 30);
            this.btnStartGame.BackColor = System.Drawing.Color.ForestGreen;
            this.btnStartGame.ForeColor = System.Drawing.Color.White;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);

            this.panelLobby.Controls.Add(this.lblLobbyTitle);
            this.panelLobby.Controls.Add(this.lstLobbyPlayers);
            this.panelLobby.Controls.Add(this.lblRoomTitle);
            this.panelLobby.Controls.Add(this.lstRooms);
            this.panelLobby.Controls.Add(this.btnReady);
            this.panelLobby.Controls.Add(this.btnCreateRoom);
            this.panelLobby.Controls.Add(this.btnJoinRoom);
            this.panelLobby.Controls.Add(this.btnLeaveRoom);
            this.panelLobby.Controls.Add(this.cmbDifficulty);
            this.panelLobby.Controls.Add(this.btnStartGame);

            // ================== GAME PANEL ==================
            this.panelGame.Size = new System.Drawing.Size(760, 560);
            this.panelGame.Location = new System.Drawing.Point(20, 20);
            this.panelGame.Visible = false;

            this.pnlBoard.Size = new System.Drawing.Size(450, 450);
            this.pnlBoard.Location = new System.Drawing.Point(10, 10);
            this.pnlBoard.BackColor = System.Drawing.Color.Black;
            // The grid buttons will be added programmatically

            this.lblGameStatus.Text = "Đang chơi...";
            this.lblGameStatus.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            this.lblGameStatus.Location = new System.Drawing.Point(10, 470);
            this.lblGameStatus.Size = new System.Drawing.Size(450, 30);
            this.lblGameStatus.ForeColor = System.Drawing.Color.DarkGreen;

            this.lstGamePlayers.Location = new System.Drawing.Point(480, 10);
            this.lstGamePlayers.Size = new System.Drawing.Size(260, 200);

            this.txtChatLog.Location = new System.Drawing.Point(480, 220);
            this.txtChatLog.Size = new System.Drawing.Size(260, 200);
            this.txtChatLog.Multiline = true;
            this.txtChatLog.ReadOnly = true;
            this.txtChatLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            this.txtChatInput.Location = new System.Drawing.Point(480, 430);
            this.txtChatInput.Size = new System.Drawing.Size(200, 25);

            this.btnSendChat.Text = "Gửi";
            this.btnSendChat.Location = new System.Drawing.Point(690, 428);
            this.btnSendChat.Size = new System.Drawing.Size(50, 30);
            this.btnSendChat.Click += new System.EventHandler(this.btnSendChat_Click);

            this.panelGame.Controls.Add(this.pnlBoard);
            this.panelGame.Controls.Add(this.lblGameStatus);
            this.panelGame.Controls.Add(this.lstGamePlayers);
            this.panelGame.Controls.Add(this.txtChatLog);
            this.panelGame.Controls.Add(this.txtChatInput);
            this.panelGame.Controls.Add(this.btnSendChat);

            this.Controls.Add(this.panelLogin);
            this.Controls.Add(this.panelLobby);
            this.Controls.Add(this.panelGame);

            this.ResumeLayout(false);
        }
    }
}