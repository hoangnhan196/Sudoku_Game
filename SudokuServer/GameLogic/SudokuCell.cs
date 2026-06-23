using System;

namespace SudokuServer.Game
{
    /// <summary>
    /// Trạng thái của một ô Sudoku.
    /// </summary>
    public enum CellState
    {
        /// <summary>Ô trống, chưa được điền.</summary>
        Empty,

        /// <summary>Ô đề bài (không thể chỉnh sửa).</summary>
        Given,

        /// <summary>Ô đã được người chơi điền vào.</summary>
        PlayerFilled
    }

    /// <summary>
    /// Đại diện cho một ô trong bảng Sudoku 9×9.
    /// Mỗi ô lưu giá trị hiện tại, đáp án đúng, và trạng thái.
    /// </summary>
    public class SudokuCell
    {
        private int _value;
        private int _solutionValue;

        /// <summary>
        /// Giá trị hiện tại của ô (0 = trống, 1-9 = giá trị).
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                if (value < 0 || value > 9)
                    throw new ArgumentOutOfRangeException(nameof(value), "Giá trị phải từ 0 đến 9.");
                _value = value;
            }
        }

        /// <summary>
        /// Đáp án đúng của ô (1-9).
        /// </summary>
        public int SolutionValue
        {
            get => _solutionValue;
            set
            {
                if (value < 0 || value > 9)
                    throw new ArgumentOutOfRangeException(nameof(value), "Giá trị đáp án phải từ 0 đến 9.");
                _solutionValue = value;
            }
        }

        /// <summary>
        /// Trạng thái hiện tại của ô.
        /// </summary>
        public CellState State { get; set; }

        /// <summary>
        /// Vị trí hàng của ô (0-8).
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Vị trí cột của ô (0-8).
        /// </summary>
        public int Col { get; }

        /// <summary>
        /// Kiểm tra giá trị hiện tại có đúng với đáp án không.
        /// </summary>
        public bool IsCorrect => Value != 0 && Value == SolutionValue;

        /// <summary>
        /// Kiểm tra ô có thể chỉnh sửa được không (chỉ ô không phải đề bài).
        /// </summary>
        public bool IsEditable => State != CellState.Given;

        /// <summary>
        /// Kiểm tra ô có đang trống không.
        /// </summary>
        public bool IsEmpty => Value == 0;

        /// <summary>
        /// Khởi tạo một ô Sudoku tại vị trí (row, col).
        /// </summary>
        /// <param name="row">Hàng (0-8).</param>
        /// <param name="col">Cột (0-8).</param>
        public SudokuCell(int row, int col)
        {
            if (row < 0 || row > 8)
                throw new ArgumentOutOfRangeException(nameof(row), "Hàng phải từ 0 đến 8.");
            if (col < 0 || col > 8)
                throw new ArgumentOutOfRangeException(nameof(col), "Cột phải từ 0 đến 8.");

            Row = row;
            Col = col;
            Value = 0;
            SolutionValue = 0;
            State = CellState.Empty;
        }

        /// <summary>
        /// Thiết lập ô là ô đề bài với giá trị cho trước.
        /// </summary>
        /// <param name="value">Giá trị đề bài (1-9).</param>
        /// <param name="solutionValue">Đáp án đúng (1-9).</param>
        public void SetAsGiven(int value, int solutionValue)
        {
            Value = value;
            SolutionValue = solutionValue;
            State = CellState.Given;
        }

        /// <summary>
        /// Thiết lập ô là ô trống (cần người chơi điền).
        /// </summary>
        /// <param name="solutionValue">Đáp án đúng (1-9).</param>
        public void SetAsEmpty(int solutionValue)
        {
            Value = 0;
            SolutionValue = solutionValue;
            State = CellState.Empty;
        }

        /// <summary>
        /// Người chơi điền giá trị vào ô.
        /// Chỉ thành công nếu ô có thể chỉnh sửa.
        /// </summary>
        /// <param name="value">Giá trị người chơi điền (1-9).</param>
        /// <returns>True nếu điền thành công.</returns>
        public bool FillValue(int value)
        {
            if (!IsEditable)
                return false;

            Value = value;
            State = value == 0 ? CellState.Empty : CellState.PlayerFilled;
            return true;
        }

        /// <summary>
        /// Xóa giá trị ô (chỉ cho ô có thể chỉnh sửa).
        /// </summary>
        /// <returns>True nếu xóa thành công.</returns>
        public bool Clear()
        {
            if (!IsEditable)
                return false;

            Value = 0;
            State = CellState.Empty;
            return true;
        }

        public override string ToString()
        {
            return $"Cell({Row},{Col}) Value={Value} Solution={SolutionValue} State={State}";
        }
    }
}
