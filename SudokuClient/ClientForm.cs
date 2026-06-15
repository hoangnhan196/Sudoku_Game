using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Text;
using SudokuClient.GameLogic;
namespace SudokuClient
{
    public partial class ClientForm : Form
    {
        private TcpClient tcpClient;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread listenThread;
        private BoardController boardController;
        public ClientForm()
        {
            InitializeComponent();
            boardController = new BoardController();
            boardController.InitializeBoard(tableLayoutPanel1);
            foreach (Control ctrl in tableLayoutPanel1.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.KeyUp += SudokuCell_KeyUp; // Móc sự kiện
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox80_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox73_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClientForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox76_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập các số từ 1 đến 9 và phím điều khiển (như Backspace để xóa)
            // Chặn số 0 vì Sudoku không dùng số 0
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '1' || e.KeyChar > '9'))
            {
                e.Handled = true; // Từ chối nhận phím này (không in ra màn hình)
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox82_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra và lấy Port từ TextBox
                if (!int.TryParse(txtPort.Text, out int port))
                {
                    MessageBox.Show("Port không hợp lệ!");
                    return;
                }

                // 1. Tạo kết nối TCP tới Server
                tcpClient = new TcpClient();
                tcpClient.Connect(txtIP.Text, port);

                // 2. Mở đường ống đọc/ghi dữ liệu
                NetworkStream stream = tcpClient.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // 3. Đóng gói lời chào (CLIENT_CONNECT) gửi lên Server
                var connectMsg = new NetworkMessage
                {
                    Type = "CLIENT_CONNECT",
                    Username = "Player1" // Tạm thời hardcode tên, sau này có thể làm ô nhập tên
                };
                string jsonMsg = JsonSerializer.Serialize(connectMsg);
                writer.WriteLine(jsonMsg);

                // 4. Mở một luồng chạy ngầm để liên tục nghe ngóng Server trả lời
                listenThread = new Thread(ListenToServer);
                listenThread.IsBackground = true;
                listenThread.Start();

                MessageBox.Show("Đã kết nối thành công tới Server!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnConnect.Enabled = false; // Khóa nút lại tránh bấm kết nối nhiều lần
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối: " + ex.Message, "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ListenToServer()
        {
            try
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break; // Server ngắt kết nối

                    // Dịch gói tin từ JSON sang Object
                    var msg = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (msg != null)
                    {
                        ProcessServerMessage(msg);
                    }
                }
            }
            catch
            {
                // Bắt lỗi khi ngắt kết nối đột ngột
            }
        }
        private void ProcessServerMessage(NetworkMessage msg)
        {
            // Vì luồng mạng không được phép sửa Giao diện trực tiếp, phải dùng Invoke
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<NetworkMessage>(ProcessServerMessage), msg);
                return;
            }

            switch (msg.Type)
            {
                case "SERVER_CONNECT_RESPONSE":
                    // Server xác nhận đã kết nối thành công
                    // Bạn có thể update giao diện ở đây
                    break;

                case "SERVER_START_GAME":
                    if (msg.Board != null)
                    {
                        boardController.LoadBoard(msg.Board); // Giao mảng số cho Controller in lên màn hình
                        MessageBox.Show("Trận đấu bắt đầu!", "Thông báo");
                    }
                    break;
                case "SERVER_MOVE_UPDATE":
                    // Khi nhận được thông báo có người vừa điền số
                    if (msg.Row.HasValue && msg.Col.HasValue && msg.Value.HasValue)
                    {
                        int r = msg.Row.Value;
                        int c = msg.Col.Value;
                        int val = msg.Value.Value;

                        // Gọi BoardController để điền số đó vào màn hình của mình
                        boardController.UpdateCell(r, c, val, msg.Correct.Value);

                        // (Tùy chọn): Hiện thông báo lên góc màn hình "Player A vừa điền đúng +10 điểm"
                    }
                    break;
            }
        }

        // Nhớ đóng mạng an toàn khi tắt app
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            listenThread?.Abort();
            reader?.Dispose();
            writer?.Dispose();
            tcpClient?.Close();
        }
        // Giả sử bạn gắn sự kiện KeyUp cho 81 ô TextBox
        private void SudokuCell_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Chỉ gửi dữ liệu khi: Đây là ô người chơi được phép gõ (không khóa) và ô đó không trống
            if (txt != null && !txt.ReadOnly && txt.Text.Length > 0)
            {
                // Lấy "Bảng tên" mà chúng ta đã phát ở bước 1 ra đọc
                int[] pos = txt.Tag as int[];

                // Cố gắng chuyển chữ người chơi gõ thành số
                if (pos != null && int.TryParse(txt.Text, out int val))
                {
                    // Đóng gói nước đi với tọa độ CHUẨN XÁC 100%
                    var moveMsg = new NetworkMessage
                    {
                        Type = "CLIENT_MOVE",
                        Row = pos[0], // Hàng
                        Col = pos[1], // Cột
                        Value = val
                    };

                    // Gửi lên Server
                    string jsonMsg = JsonSerializer.Serialize(moveMsg);
                    writer.WriteLine(jsonMsg);
                }
            }
        }
    }
}

