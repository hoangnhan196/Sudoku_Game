using System;

namespace SudokuServer.Game
{
    /// <summary>
    /// Dịch vụ kiểm tra tính hợp lệ theo luật Sudoku.
    /// Luật Sudoku: mỗi số 1-9 chỉ xuất hiện duy nhất trong mỗi hàng, cột, và khối 3×3.
    /// </summary>
    public class SudokuValidator
    {
        /// <summary>
        /// Kích thước bảng Sudoku (9×9).
        /// </summary>
        public const int BoardSize = 9;

        /// <summary>
        /// Kích thước khối con (3×3).
        /// </summary>
        public const int BoxSize = 3;

        /// <summary>
        /// Kiểm tra việc đặt một số vào vị trí (row, col) có hợp lệ không.
        /// Hợp lệ = không vi phạm luật Sudoku (hàng, cột, khối 3×3).
        /// </summary>
        /// <param name="board">Bảng Sudoku hiện tại (9×9).</param>
        /// <param name="row">Hàng (0-8).</param>
        /// <param name="col">Cột (0-8).</param>
        /// <param name="value">Giá trị cần kiểm tra (1-9).</param>
        /// <returns>True nếu đặt số hợp lệ.</returns>
        public bool IsValidPlacement(int[,] board, int row, int col, int value)
        {
            if (value < 1 || value > 9)
                return false;

            if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
                return false;

            return IsRowValid(board, row, value, col)
                && IsColumnValid(board, col, value, row)
                && IsBoxValid(board, row, col, value);
        }

        /// <summary>
        /// Kiểm tra số có bị trùng trong hàng không.
        /// </summary>
        /// <param name="board">Bảng Sudoku.</param>
        /// <param name="row">Hàng cần kiểm tra.</param>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <param name="excludeCol">Cột cần bỏ qua (vị trí đang đặt số).</param>
        /// <returns>True nếu không bị trùng.</returns>
        public bool IsRowValid(int[,] board, int row, int value, int excludeCol)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (c == excludeCol) continue;
                if (board[row, c] == value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra số có bị trùng trong cột không.
        /// </summary>
        /// <param name="board">Bảng Sudoku.</param>
        /// <param name="col">Cột cần kiểm tra.</param>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <param name="excludeRow">Hàng cần bỏ qua (vị trí đang đặt số).</param>
        /// <returns>True nếu không bị trùng.</returns>
        public bool IsColumnValid(int[,] board, int col, int value, int excludeRow)
        {
            for (int r = 0; r < BoardSize; r++)
            {
                if (r == excludeRow) continue;
                if (board[r, col] == value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra số có bị trùng trong khối 3×3 không.
        /// </summary>
        /// <param name="board">Bảng Sudoku.</param>
        /// <param name="row">Hàng của ô cần kiểm tra.</param>
        /// <param name="col">Cột của ô cần kiểm tra.</param>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <returns>True nếu không bị trùng.</returns>
        public bool IsBoxValid(int[,] board, int row, int col, int value)
        {
            // Tìm góc trên-trái của khối 3×3 chứa ô (row, col)
            int boxStartRow = (row / BoxSize) * BoxSize;
            int boxStartCol = (col / BoxSize) * BoxSize;

            for (int r = boxStartRow; r < boxStartRow + BoxSize; r++)
            {
                for (int c = boxStartCol; c < boxStartCol + BoxSize; c++)
                {
                    // Bỏ qua chính ô đang xét
                    if (r == row && c == col) continue;
                    if (board[r, c] == value)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra toàn bộ bảng Sudoku đã hoàn thành và hợp lệ chưa.
        /// Bảng hoàn thành = tất cả ô đều có giá trị 1-9 và không vi phạm luật.
        /// </summary>
        /// <param name="board">Bảng Sudoku cần kiểm tra.</param>
        /// <returns>True nếu bảng hoàn thành hợp lệ.</returns>
        public bool IsBoardComplete(int[,] board)
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    int val = board[r, c];
                    if (val < 1 || val > 9)
                        return false;

                    if (!IsValidPlacement(board, r, c, val))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra bảng Sudoku có còn ô trống nào không.
        /// </summary>
        /// <param name="board">Bảng Sudoku.</param>
        /// <returns>True nếu không còn ô trống (value == 0).</returns>
        public bool IsBoardFilled(int[,] board)
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (board[r, c] == 0)
                        return false;
                }
            }
            return true;
        }
    }
}
