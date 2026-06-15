using System;
using System.Collections.Generic;

namespace SudokuServer.GameLogic
{
    public class SudokuCore
    {
        // Mảng lưu trạng thái hiện tại của bàn cờ (đề bài & nước đi)
        public int[,] PlayerBoard { get; set; } = new int[9, 9];

        // Mảng lưu đáp án gốc để hệ thống đối chiếu
        public int[,] SolutionBoard { get; set; } = new int[9, 9];

        private Random rand = new Random();

        // 1. Hàm tạo game mới
        public void GenerateNewGame(int cellsToRemove)
        {
            // Reset lại 2 mảng về số 0
            PlayerBoard = new int[9, 9];
            SolutionBoard = new int[9, 9];

            // Bước 1: Dùng Backtracking sinh ra 1 bảng Sudoku hoàn chỉnh (Đáp án)
            FillBoard(SolutionBoard);

            // Bước 2: Chép đáp án sang bảng người chơi
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    PlayerBoard[r, c] = SolutionBoard[r, c];
                }
            }

            // Bước 3: Khoét lỗ ngẫu nhiên để tạo thành đề bài
            RemoveCells(cellsToRemove);
        }

        // --- THUẬT TOÁN QUAY LUI (BACKTRACKING) ---
        private bool FillBoard(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    // Nếu tìm thấy ô trống
                    if (board[row, col] == 0)
                    {
                        // Xáo trộn ngẫu nhiên các số từ 1-9 để tạo map mới mẻ
                        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        Shuffle(numbers);

                        foreach (int num in numbers)
                        {
                            // Nếu điền số này không vi phạm luật Sudoku
                            if (IsSafe(board, row, col, num))
                            {
                                board[row, col] = num; // Thử điền

                                // Đệ quy: Gọi lại hàm này để điền ô tiếp theo
                                if (FillBoard(board))
                                    return true;

                                // QUAY LUI (BACKTRACK): Nếu đi tiếp vào ngõ cụt, xóa số này đi và thử số khác
                                board[row, col] = 0;
                            }
                        }
                        return false; // Hết số thử mà không được -> Báo lỗi cho đệ quy lùi lại
                    }
                }
            }
            return true; // Không còn ô trống nào -> Bảng đã điền thành công
        }

        // Hàm kiểm tra luật Sudoku (Trùng hàng, trùng cột, trùng ô 3x3)
        private bool IsSafe(int[,] board, int row, int col, int num)
        {
            // Check hàng ngang và cột dọc
            for (int i = 0; i < 9; i++)
            {
                if (board[row, i] == num || board[i, col] == num)
                    return false;
            }

            // Check khối 3x3
            int startRow = row - (row % 3);
            int startCol = col - (col % 3);
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (board[startRow + r, startCol + c] == num)
                        return false;
                }
            }

            return true;
        }

        // Hàm xáo trộn danh sách số ngẫu nhiên
        private void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Hàm xóa ô ngẫu nhiên để tạo đề bài
        private void RemoveCells(int cellsToRemove)
        {
            int count = 0;
            while (count < cellsToRemove)
            {
                int r = rand.Next(9);
                int c = rand.Next(9);

                // Nếu ô này chưa bị xóa thì mới xóa (set về 0)
                if (PlayerBoard[r, c] != 0)
                {
                    PlayerBoard[r, c] = 0;
                    count++;
                }
            }
        }

        // --- CÁC HÀM XỬ LÝ NƯỚC ĐI TỪ CLIENT GỬI LÊN ---
        public bool CheckMove(int row, int col, int value)
        {
            // So sánh với mảng đáp án gốc
            return SolutionBoard[row, col] == value;
        }

        public void ApplyMove(int row, int col, int value)
        {
            PlayerBoard[row, col] = value;
        }

        public bool IsGameFinished()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (PlayerBoard[r, c] == 0)
                        return false; // Vẫn còn ô trống (số 0)
                }
            }
            return true; // Đã điền kín bảng
        }
    }
}