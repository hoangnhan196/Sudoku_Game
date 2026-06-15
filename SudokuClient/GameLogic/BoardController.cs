using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SudokuClient.GameLogic
{
    internal class BoardController
    {
        // Mảng 2 chiều quản lý 81 ô TextBox trên giao diện
        private TextBox[,] cells = new TextBox[9, 9];

        // Hàm này sẽ quét cái bảng lưới (TableLayoutPanel) để lấy toàn bộ TextBox cho vào mảng
        public void InitializeBoard(TableLayoutPanel grid)
        {
            // Bước 1: Gom tất cả 81 ô TextBox trên giao diện lại vào 1 danh sách
            var textBoxes = new System.Collections.Generic.List<TextBox>();
            foreach (Control ctrl in grid.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    textBoxes.Add(txt);
                    txt.TextAlign = HorizontalAlignment.Center; // Căn giữa chữ cho đẹp
                }
            }

            // Bước 2: Sắp xếp lại danh sách chuẩn xác theo tọa độ thực tế trên màn hình
            // (Khắc phục hoàn toàn lỗi TableLayoutPanel bị ảo vị trí)
            textBoxes.Sort((t1, t2) =>
            {
                if (Math.Abs(t1.Location.Y - t2.Location.Y) > 10)
                    return t1.Location.Y.CompareTo(t2.Location.Y);
                return t1.Location.X.CompareTo(t2.Location.X);
            });

            // Bước 3: Đưa vào mảng 2 chiều và "Phát bảng tên" (Tag) cho từng ô
            int index = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (index < textBoxes.Count)
                    {
                        cells[row, col] = textBoxes[index];

                        // Gắn tọa độ vào Tag để ClientForm biết chính xác đang gõ ở hàng/cột nào
                        cells[row, col].Tag = new int[] { row, col };

                        index++;
                    }
                }
            }
        }

        // Hàm nhận dữ liệu từ Server và in lên giao diện
        public void LoadBoard(int[][] boardData)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int val = boardData[r][c];
                    if (val != 0) // Nếu có số (Đề bài)
                    {
                        cells[r, c].Text = val.ToString();
                        cells[r, c].ReadOnly = true; // Khóa lại, không cho người chơi xóa đề
                        cells[r, c].ForeColor = Color.DarkBlue; // Đổi màu xanh đậm để phân biệt với số người chơi gõ
                    }
                    else // Nếu là ô trống (Số 0)
                    {
                        cells[r, c].Text = "";
                        cells[r, c].ReadOnly = false; // Mở khóa cho người chơi gõ
                        cells[r, c].ForeColor = Color.Black;
                    }
                }
            }
        }
        // Hàm cập nhật từng ô trên bàn cờ khi có người chơi điền số
        public void UpdateCell(int row, int col, int value, bool isCorrect)
        {
            // Cập nhật số vừa điền vào đúng ô trên giao diện
            cells[row, col].Text = value.ToString();

            // Tô màu để báo hiệu đúng hay sai
            if (isCorrect)
            {
                cells[row, col].ForeColor = Color.Green; // Điền ĐÚNG: Chữ màu xanh lá
            }
            else
            {
                cells[row, col].ForeColor = Color.Red;   // Điền SAI: Chữ màu đỏ
            }
        }
    }
}

