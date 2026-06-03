using System;

namespace SudokuServer.Game
{
    /// <summary>
    /// Quản lý bảng Sudoku 9×9, đóng gói mảng SudokuCell.
    /// Cung cấp các thao tác đọc/ghi ô, chuyển đổi dữ liệu.
    /// </summary>
    public class SudokuBoard
    {
        public const int Size = 9;

        private readonly SudokuCell[,] _cells;

        /// <summary>
        /// Khởi tạo bảng Sudoku trống 9×9.
        /// </summary>
        public SudokuBoard()
        {
            _cells = new SudokuCell[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    _cells[r, c] = new SudokuCell(r, c);
                }
            }
        }

        /// <summary>
        /// Lấy ô tại vị trí (row, col).
        /// </summary>
        public SudokuCell GetCell(int row, int col)
        {
            ValidatePosition(row, col);
            return _cells[row, col];
        }

        /// <summary>
        /// Đặt giá trị vào ô (chỉ cho ô editable).
        /// </summary>
        /// <returns>True nếu đặt thành công.</returns>
        public bool SetCell(int row, int col, int value)
        {
            ValidatePosition(row, col);
            return _cells[row, col].FillValue(value);
        }

        /// <summary>
        /// Xóa giá trị ô (chỉ cho ô editable).
        /// </summary>
        public bool ClearCell(int row, int col)
        {
            ValidatePosition(row, col);
            return _cells[row, col].Clear();
        }

        /// <summary>
        /// Kiểm tra bảng đã được điền đầy đủ chưa (không còn ô trống).
        /// </summary>
        public bool IsComplete()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (_cells[r, c].IsEmpty)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Đếm số ô trống còn lại.
        /// </summary>
        public int CountEmptyCells()
        {
            int count = 0;
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (_cells[r, c].IsEmpty)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Khởi tạo bảng từ mảng đáp án và mảng puzzle.
        /// Ô có giá trị trong puzzle → Given, ô trống → Empty.
        /// </summary>
        /// <param name="solution">Mảng đáp án đầy đủ 9×9.</param>
        /// <param name="puzzle">Mảng puzzle (0 = ô trống).</param>
        public void InitializeFromArrays(int[,] solution, int[,] puzzle)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (puzzle[r, c] != 0)
                    {
                        _cells[r, c].SetAsGiven(puzzle[r, c], solution[r, c]);
                    }
                    else
                    {
                        _cells[r, c].SetAsEmpty(solution[r, c]);
                    }
                }
            }
        }

        /// <summary>
        /// Chuyển bảng hiện tại thành mảng int[,] (dùng để gửi qua network).
        /// </summary>
        public int[,] ToIntArray()
        {
            int[,] result = new int[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    result[r, c] = _cells[r, c].Value;
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy mảng đáp án int[,].
        /// </summary>
        public int[,] GetSolutionArray()
        {
            int[,] result = new int[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    result[r, c] = _cells[r, c].SolutionValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra vị trí hợp lệ.
        /// </summary>
        private void ValidatePosition(int row, int col)
        {
            if (row < 0 || row >= Size)
                throw new ArgumentOutOfRangeException(nameof(row), $"Hàng phải từ 0 đến {Size - 1}.");
            if (col < 0 || col >= Size)
                throw new ArgumentOutOfRangeException(nameof(col), $"Cột phải từ 0 đến {Size - 1}.");
        }
    }
}
