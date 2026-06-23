using System;
using System.Collections.Generic;

namespace SudokuClient.GameLogic
{
    public static class GameValidator
    {
        public const int BoardSize = 9;
        public const int BoxSize = 3;

        public static bool IsValidMove(Board board, int row, int col, int value)
        {
            if (board == null)
                return false;

            try
            {
                var cell = board.GetCell(row, col);
                if (cell.IsGiven || value < 1 || value > 9)
                    return false;

                return board.CanPlace(row, col, value);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsGameWon(Board board)
        {
            if (board == null)
                return false;

            return board.IsComplete();
        }

        public static bool IsBoardValid(Board board)
        {
            if (board == null)
                return false;

            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (board.HasConflict(r, c))
                        return false;
                }
            }

            return true;
        }

        public static bool IsRowValid(Board board, int row)
        {
            if (board == null || row < 0 || row >= BoardSize)
                return false;

            var values = new bool[10];
            for (int c = 0; c < BoardSize; c++)
            {
                var cell = board.GetCell(row, c);
                if (cell.Value != 0)
                {
                    if (values[cell.Value])
                        return false;
                    values[cell.Value] = true;
                }
            }
            return true;
        }

        public static bool IsColumnValid(Board board, int col)
        {
            if (board == null || col < 0 || col >= BoardSize)
                return false;

            var values = new bool[10];
            for (int r = 0; r < BoardSize; r++)
            {
                var cell = board.GetCell(r, col);
                if (cell.Value != 0)
                {
                    if (values[cell.Value])
                        return false;
                    values[cell.Value] = true;
                }
            }
            return true;
        }

        public static bool IsBoxValid(Board board, int boxRow, int boxCol)
        {
            if (board == null || boxRow < 0 || boxRow >= 3 || boxCol < 0 || boxCol >= 3)
                return false;

            var values = new bool[10];
            int startRow = boxRow * BoxSize;
            int startCol = boxCol * BoxSize;

            for (int r = startRow; r < startRow + BoxSize; r++)
            {
                for (int c = startCol; c < startCol + BoxSize; c++)
                {
                    var cell = board.GetCell(r, c);
                    if (cell.Value != 0)
                    {
                        if (values[cell.Value])
                            return false;
                        values[cell.Value] = true;
                    }
                }
            }
            return true;
        }

        public static int[] GetPossibleValues(Board board, int row, int col)
        {
            var possible = new System.Collections.Generic.List<int>();

            for (int value = 1; value <= 9; value++)
            {
                if (IsValidMove(board, row, col, value))
                    possible.Add(value);
            }

            return possible.ToArray();
        }
    }
}
