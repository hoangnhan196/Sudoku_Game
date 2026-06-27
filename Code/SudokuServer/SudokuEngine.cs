using System;
using System.Collections.Generic;

namespace SudokuServer.Game
{
    public class SudokuEngine
    {
        public int[,] SolutionBoard { get; private set; } = new int[9, 9];
        public int[,] PlayerBoard { get; private set; } = new int[9, 9];
        private static readonly Random _random = new Random();

        public void GenerateNewGame(int cellsToRemove)
        {
            SolutionBoard = new int[9, 9];
            PlayerBoard = new int[9, 9];

            // 1. Sinh bảng Sudoku đầy đủ nghiệm hợp lệ
            FillBoard(0, 0);

            // 2. Sao chép sang bảng của người chơi để chuẩn bị đục lỗ
            Array.Copy(SolutionBoard, PlayerBoard, SolutionBoard.Length);

            // 3. Tiến hành đục lỗ dựa trên số lượng ô trống yêu cầu
            RemoveCells(cellsToRemove);
        }

        private bool FillBoard(int row, int col)
        {
            if (col >= 9)
            {
                col = 0;
                row++;
                if (row >= 9) return true;
            }

            var numbers = GetRandomNumbers();
            foreach (var num in numbers)
            {
                if (IsValid(SolutionBoard, row, col, num))
                {
                    SolutionBoard[row, col] = num;
                    if (FillBoard(row, col + 1)) return true;
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

        public bool IsValid(int[,] board, int row, int col, int num)
        {
            for (int x = 0; x < 9; x++)
            {
                if (board[row, x] == num) return false;
                if (board[x, col] == num) return false;
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i + startRow, j + startCol] == num) return false;
                }
            }
            return true;
        }

        private void RemoveCells(int count)
        {
            if (count > 81) count = 81;

            // Trộn ngẫu nhiên 81 vị trí trên ma trận bằng Fisher-Yates để đảm bảo đề ra ngẫu nhiên dữ liệu
            List<int> positions = new List<int>(81);
            for (int i = 0; i < 81; i++) positions.Add(i);
            
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                int temp = positions[i];
                positions[i] = positions[j];
                positions[j] = temp;
            }

            int removed = 0;
            for (int i = 0; i < 81 && removed < count; i++)
            {
                int pos = positions[i];
                int r = pos / 9;
                int c = pos % 9;

                int backup = PlayerBoard[r, c];
                PlayerBoard[r, c] = 0;

                int solutions = 0;
                // Sử dụng bộ giải thông minh MRV để đếm số lượng nghiệm
                CountSolutionsMRV(PlayerBoard, ref solutions);

                if (solutions != 1)
                {
                    // Nếu làm mất tính duy nhất của nghiệm (vô nghiệm hoặc đa nghiệm) -> Phục hồi ô số
                    PlayerBoard[r, c] = backup;
                }
                else
                {
                    removed++;
                }
            }
        }

        private bool CountSolutionsMRV(int[,] board, ref int count)
        {
            if (count > 1) return true; // Dừng sớm nếu phát hiện ma trận đa nghiệm

            int minCandidates = 10;
            int bestRow = -1;
            int bestCol = -1;
            List<int> bestCandidates = null;

            // Tìm ô trống có ít số khả dĩ hợp lệ nhất để tối ưu nhánh đệ quy
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == 0)
                    {
                        var candidates = GetCandidates(board, r, c);
                        int cc = candidates.Count;

                        if (cc == 0) return false; // Nhánh cụt không có lời giải

                        if (cc < minCandidates)
                        {
                            minCandidates = cc;
                            bestRow = r;
                            bestCol = c;
                            bestCandidates = candidates;
                        }
                    }
                }
            }

            if (bestRow == -1)
            {
                count++;
                return count > 1;
            }

            foreach (int num in bestCandidates)
            {
                board[bestRow, bestCol] = num;
                if (CountSolutionsMRV(board, ref count))
                {
                    board[bestRow, bestCol] = 0;
                    return true;
                }
                board[bestRow, bestCol] = 0;
            }

            return false;
        }

        private List<int> GetCandidates(int[,] board, int row, int col)
        {
            List<int> candidates = new List<int>();
            for (int num = 1; num <= 9; num++)
            {
                if (IsValid(board, row, col, num)) candidates.Add(num);
            }
            return candidates;
        }

        public bool CheckMove(int row, int col, int value) => SolutionBoard[row, col] == value;

        public void ApplyMove(int row, int col, int value) => PlayerBoard[row, col] = value;

        public bool IsGameFinished()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (PlayerBoard[r, c] != SolutionBoard[r, c]) return false;
            return true;
        }
    }
}