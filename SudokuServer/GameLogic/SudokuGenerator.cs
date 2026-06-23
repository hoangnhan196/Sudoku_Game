using System;
using System.Collections.Generic;

namespace SudokuServer.Game
{
    /// <summary>
    /// Thuật toán tạo bảng Sudoku.
    /// Sử dụng backtracking với random shuffle để tạo bảng đầy đủ,
    /// sau đó xóa ô ngẫu nhiên để tạo puzzle.
    /// </summary>
    public class SudokuGenerator
    {
        private const int Size = 9;
        private readonly Random _random;
        private readonly SudokuValidator _validator;

        public SudokuGenerator()
        {
            _random = new Random();
            _validator = new SudokuValidator();
        }

        /// <summary>
        /// Tạo bảng Sudoku đầy đủ (đã giải xong) bằng backtracking.
        /// </summary>
        /// <returns>Mảng 9×9 chứa đáp án hoàn chỉnh.</returns>
        public int[,] GenerateFullBoard()
        {
            int[,] board = new int[Size, Size];

            if (!FillBoard(board, 0, 0))
            {
                throw new InvalidOperationException("Không thể tạo bảng Sudoku hợp lệ.");
            }

            return board;
        }

        /// <summary>
        /// Tạo puzzle từ bảng đầy đủ bằng cách xóa một số ô ngẫu nhiên.
        /// </summary>
        /// <param name="solution">Bảng đáp án đầy đủ.</param>
        /// <param name="cellsToRemove">Số ô cần xóa (độ khó).</param>
        /// <returns>Mảng 9×9 puzzle (0 = ô trống).</returns>
        public int[,] CreatePuzzle(int[,] solution, int cellsToRemove)
        {
            if (cellsToRemove < 0 || cellsToRemove > 81)
                throw new ArgumentOutOfRangeException(nameof(cellsToRemove), "Số ô xóa phải từ 0 đến 81.");

            // Clone bảng đáp án
            int[,] puzzle = (int[,])solution.Clone();

            // Tạo danh sách tất cả vị trí và xáo trộn
            List<(int row, int col)> positions = new List<(int, int)>();
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    positions.Add((r, c));
                }
            }
            ShuffleList(positions);

            // Xóa ô theo thứ tự đã xáo trộn
            int removed = 0;
            foreach (var (row, col) in positions)
            {
                if (removed >= cellsToRemove)
                    break;

                puzzle[row, col] = 0;
                removed++;
            }

            return puzzle;
        }

        /// <summary>
        /// Thuật toán backtracking đệ quy để điền bảng Sudoku.
        /// Duyệt từng ô từ trái sang phải, trên xuống dưới.
        /// Tại mỗi ô, thử các số 1-9 (đã xáo trộn ngẫu nhiên).
        /// </summary>
        /// <param name="board">Bảng Sudoku đang điền.</param>
        /// <param name="row">Hàng hiện tại.</param>
        /// <param name="col">Cột hiện tại.</param>
        /// <returns>True nếu điền thành công toàn bộ bảng.</returns>
        private bool FillBoard(int[,] board, int row, int col)
        {
            // Đã điền hết tất cả các hàng → thành công
            if (row == Size)
                return true;

            // Tính vị trí ô tiếp theo
            int nextRow = col == Size - 1 ? row + 1 : row;
            int nextCol = col == Size - 1 ? 0 : col + 1;

            // Tạo mảng số 1-9 và xáo trộn ngẫu nhiên
            int[] numbers = GenerateShuffledNumbers();

            foreach (int num in numbers)
            {
                // Kiểm tra có thể đặt số này tại (row, col) không
                if (_validator.IsValidPlacement(board, row, col, num))
                {
                    board[row, col] = num;

                    // Đệ quy điền ô tiếp theo
                    if (FillBoard(board, nextRow, nextCol))
                        return true;

                    // Backtrack: không tìm được lời giải → xóa và thử số khác
                    board[row, col] = 0;
                }
            }

            // Không có số nào hợp lệ → backtrack
            return false;
        }

        /// <summary>
        /// Tạo mảng [1..9] đã xáo trộn ngẫu nhiên (Fisher-Yates shuffle).
        /// </summary>
        private int[] GenerateShuffledNumbers()
        {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Fisher-Yates shuffle
            for (int i = numbers.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                // Swap
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }

            return numbers;
        }

        /// <summary>
        /// Xáo trộn ngẫu nhiên một danh sách (Fisher-Yates shuffle).
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
