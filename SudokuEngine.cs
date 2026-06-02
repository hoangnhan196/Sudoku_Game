using System;

namespace SudokuServer.Game
{
    public class SudokuEngine
    {
        public int[,] SolutionBoard { get; private set; } = new int[9, 9];
        public int[,] PlayerBoard { get; private set; } = new int[9, 9];
        private static readonly Random _random = new Random();

        public SudokuEngine()
        {
            GenerateNewGame(40);
        }

        public void GenerateNewGame(int cellsToRemove)
        {
            // Reset boards
            SolutionBoard = new int[9, 9];
            PlayerBoard = new int[9, 9];

            // Generate full solved board
            FillBoard(0, 0);

            // Copy to player board and remove cells
            Array.Copy(SolutionBoard, PlayerBoard, SolutionBoard.Length);
            RemoveCells(cellsToRemove);
        }

        private bool FillBoard(int row, int col)
        {
            if (col >= 9)
            {
                col = 0;
                row++;
                if (row >= 9)
                    return true;
            }

            // Shuffle numbers 1-9 to make generation random
            var numbers = GetRandomNumbers();
            foreach (var num in numbers)
            {
                if (IsValid(SolutionBoard, row, col, num))
                {
                    SolutionBoard[row, col] = num;
                    if (FillBoard(row, col + 1))
                        return true;
                    SolutionBoard[row, col] = 0;
                }
            }

            return false;
        }

        private int[] GetRandomNumbers()
        {
            int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = nums.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                int temp = nums[i];
                nums[i] = nums[j];
                nums[j] = temp;
            }
            return nums;
        }

        private bool IsValid(int[,] board, int row, int col, int num)
        {
            // Check row
            for (int x = 0; x < 9; x++)
                if (board[row, x] == num)
                    return false;

            // Check col
            for (int x = 0; x < 9; x++)
                if (board[x, col] == num)
                    return false;

            // Check 3x3 box
            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i + startRow, j + startCol] == num)
                        return false;

            return true;
        }

        private void RemoveCells(int count)
        {
            int removed = 0;
            while (removed < count)
            {
                int r = _random.Next(9);
                int c = _random.Next(9);
                if (PlayerBoard[r, c] != 0)
                {
                    PlayerBoard[r, c] = 0;
                    removed++;
                }
            }
        }

        public bool CheckMove(int row, int col, int value)
        {
            if (row < 0 || row >= 9 || col < 0 || col >= 9) return false;
            return SolutionBoard[row, col] == value;
        }

        public void ApplyMove(int row, int col, int value)
        {
            if (row >= 0 && row < 9 && col >= 0 && col < 9)
            {
                PlayerBoard[row, col] = value;
            }
        }

        public bool IsGameFinished()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (PlayerBoard[r, c] != SolutionBoard[r, c])
                        return false;
                }
            }
            return true;
        }
    }
}
