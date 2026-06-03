using System;

namespace SudokuServer.Game
{
    /// <summary>
    /// Facade class tổng hợp toàn bộ logic game Sudoku.
    /// Đây là API chính mà các tầng khác (Room, SocketManager) sẽ gọi.
    /// Kết hợp SudokuBoard, SudokuGenerator, SudokuValidator.
    /// </summary>
    public class SudokuEngine
    {
        private readonly SudokuBoard _board;
        private readonly SudokuGenerator _generator;
        private readonly SudokuValidator _validator;

        /// <summary>
        /// Bảng hiện tại của người chơi (bao gồm cả ô đã điền và ô trống).
        /// </summary>
        public int[,] PlayerBoard => _board.ToIntArray();

        /// <summary>
        /// Bảng đáp án hoàn chỉnh.
        /// </summary>
        public int[,] SolutionBoard => _board.GetSolutionArray();

        public SudokuEngine()
        {
            _board = new SudokuBoard();
            _generator = new SudokuGenerator();
            _validator = new SudokuValidator();
        }

        /// <summary>
        /// Tạo game mới với độ khó xác định bởi số ô bị xóa.
        /// 1. Tạo bảng đáp án đầy đủ bằng backtracking.
        /// 2. Xóa ngẫu nhiên cellsToRemove ô để tạo puzzle.
        /// 3. Khởi tạo SudokuBoard với đáp án và puzzle.
        /// </summary>
        /// <param name="cellsToRemove">Số ô cần xóa (30=Easy, 42=Medium, 52=Hard, 62=Expert, 72=Evil).</param>
        public void GenerateNewGame(int cellsToRemove)
        {
            // Bước 1: Tạo bảng đáp án đầy đủ
            int[,] solution = _generator.GenerateFullBoard();

            // Bước 2: Tạo puzzle bằng cách xóa ô
            int[,] puzzle = _generator.CreatePuzzle(solution, cellsToRemove);

            // Bước 3: Khởi tạo board với dữ liệu
            _board.InitializeFromArrays(solution, puzzle);
        }

        /// <summary>
        /// Kiểm tra nước đi có đúng với đáp án không.
        /// So sánh giá trị người chơi đặt với SolutionBoard.
        /// </summary>
        /// <param name="row">Hàng (0-8).</param>
        /// <param name="col">Cột (0-8).</param>
        /// <param name="value">Giá trị người chơi đặt (1-9).</param>
        /// <returns>True nếu giá trị đúng với đáp án.</returns>
        public bool CheckMove(int row, int col, int value)
        {
            var cell = _board.GetCell(row, col);

            // Không cho sửa ô đề bài
            if (!cell.IsEditable)
                return false;

            // So sánh với đáp án
            return value == cell.SolutionValue;
        }

        /// <summary>
        /// Áp dụng nước đi vào bảng (sau khi đã kiểm tra đúng).
        /// </summary>
        /// <param name="row">Hàng (0-8).</param>
        /// <param name="col">Cột (0-8).</param>
        /// <param name="value">Giá trị cần đặt (1-9).</param>
        public void ApplyMove(int row, int col, int value)
        {
            _board.SetCell(row, col, value);
        }

        /// <summary>
        /// Kiểm tra game đã hoàn thành chưa (tất cả ô đã được điền đúng).
        /// </summary>
        /// <returns>True nếu bảng đã hoàn thành.</returns>
        public bool IsGameFinished()
        {
            return _board.IsComplete();
        }

        /// <summary>
        /// Lấy số ô trống còn lại.
        /// </summary>
        public int GetRemainingCells()
        {
            return _board.CountEmptyCells();
        }

        /// <summary>
        /// Lấy thông tin một ô cụ thể.
        /// </summary>
        public SudokuCell GetCell(int row, int col)
        {
            return _board.GetCell(row, col);
        }
    }
}
