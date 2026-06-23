using System;

namespace SudokuClient.GameLogic
{
    public class Board
    {
        public const int BoardSize = 9;
        public const int BoxSize = 3;
        private Cell[,] _cells;

        public Board()
        {
            _cells = new Cell[BoardSize, BoardSize];
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    _cells[r, c] = new Cell();
                }
            }
        }

        public Cell GetCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                throw new ArgumentOutOfRangeException($"Invalid position: ({row}, {col})");
            return _cells[row, col];
        }

        public bool SetCell(int row, int col, int value)
        {
            if (!IsValidPosition(row, col))
                return false;
            return _cells[row, col].SetValue(value);
        }

        public bool ClearCell(int row, int col)
        {
            if (!IsValidPosition(row, col))
                return false;
            return _cells[row, col].Clear();
        }

        public void LoadFromArray(int[][] boardData)
        {
            if (boardData == null || boardData.Length != BoardSize)
                throw new ArgumentException("Invalid board data");

            for (int r = 0; r < BoardSize; r++)
            {
                if (boardData[r] == null || boardData[r].Length != BoardSize)
                    throw new ArgumentException("Invalid row data");

                for (int c = 0; c < BoardSize; c++)
                {
                    int value = boardData[r][c];
                    _cells[r, c] = new Cell(value);
                }
            }
        }

        public int[,] ToArray()
        {
            var result = new int[BoardSize, BoardSize];
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    result[r, c] = _cells[r, c].Value;
                }
            }
            return result;
        }

        public int[][] ToJaggedArray()
        {
            var result = new int[BoardSize][];
            for (int r = 0; r < BoardSize; r++)
            {
                result[r] = new int[BoardSize];
                for (int c = 0; c < BoardSize; c++)
                {
                    result[r][c] = _cells[r, c].Value;
                }
            }
            return result;
        }

        public Board Clone()
        {
            var clone = new Board();
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    clone._cells[r, c] = this._cells[r, c].Clone();
                }
            }
            return clone;
        }

        public bool HasConflict(int row, int col)
        {
            if (!IsValidPosition(row, col))
                return false;

            int val = _cells[row, col].Value;
            if (val == 0)
                return false;

            // Kiểm tra hàng
            for (int c = 0; c < BoardSize; c++)
            {
                if (c != col && _cells[row, c].Value == val)
                    return true;
            }

            // Kiểm tra cột
            for (int r = 0; r < BoardSize; r++)
            {
                if (r != row && _cells[r, col].Value == val)
                    return true;
            }

            // Kiểm tra khối 3×3
            int boxStartRow = (row / BoxSize) * BoxSize;
            int boxStartCol = (col / BoxSize) * BoxSize;
            for (int r = boxStartRow; r < boxStartRow + BoxSize; r++)
            {
                for (int c = boxStartCol; c < boxStartCol + BoxSize; c++)
                {
                    if ((r != row || c != col) && _cells[r, c].Value == val)
                        return true;
                }
            }

            return false;
        }

        public bool CanPlace(int row, int col, int value)
        {
            if (!IsValidPosition(row, col) || value < 1 || value > 9)
                return false;

            // Kiểm tra hàng
            for (int c = 0; c < BoardSize; c++)
            {
                if (_cells[row, c].Value == value)
                    return false;
            }

            // Kiểm tra cột
            for (int r = 0; r < BoardSize; r++)
            {
                if (_cells[r, col].Value == value)
                    return false;
            }

            // Kiểm tra khối 3×3
            int boxStartRow = (row / BoxSize) * BoxSize;
            int boxStartCol = (col / BoxSize) * BoxSize;
            for (int r = boxStartRow; r < boxStartRow + BoxSize; r++)
            {
                for (int c = boxStartCol; c < boxStartCol + BoxSize; c++)
                {
                    if (_cells[r, c].Value == value)
                        return false;
                }
            }

            return true;
        }

        public bool IsComplete()
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (_cells[r, c].Value == 0 || HasConflict(r, c))
                        return false;
                }
            }
            return true;
        }

        public int CountFilled()
        {
            int count = 0;
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (_cells[r, c].Value != 0)
                        count++;
                }
            }
            return count;
        }

        public void RemoveNotesForValue(int row, int col, int value)
        {
            // Xóa trong hàng
            for (int c = 0; c < BoardSize; c++)
            {
                _cells[row, c].RemoveNote(value);
            }

            // Xóa trong cột
            for (int r = 0; r < BoardSize; r++)
            {
                _cells[r, col].RemoveNote(value);
            }

            // Xóa trong khối 3×3
            int boxStartRow = (row / BoxSize) * BoxSize;
            int boxStartCol = (col / BoxSize) * BoxSize;
            for (int r = boxStartRow; r < boxStartRow + BoxSize; r++)
            {
                for (int c = boxStartCol; c < boxStartCol + BoxSize; c++)
                {
                    _cells[r, c].RemoveNote(value);
                }
            }
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }
    }
}
