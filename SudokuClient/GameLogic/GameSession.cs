using System;
using System.Collections.Generic;

namespace SudokuClient.GameLogic
{
    public class GameSession
    {
        public Board Board { get; private set; }
        private List<Move> _moveHistory;
        private int _moveHistoryIndex;

        public DateTime StartTime { get; private set; }
        public GameState State { get; set; }

        public event Action? OnBoardChanged;
        public event Action? OnGameWon;
        public event Action<int, int, int>? OnInvalidMove;

        public GameSession()
        {
            Board = new Board();
            _moveHistory = new List<Move>();
            _moveHistoryIndex = 0;
            StartTime = DateTime.Now;
            State = GameState.Playing;
        }

        public void LoadBoard(int[][] boardData)
        {
            if (boardData == null)
                throw new ArgumentNullException(nameof(boardData));

            Board.LoadFromArray(boardData);
            _moveHistory.Clear();
            _moveHistoryIndex = 0;
            StartTime = DateTime.Now;
            State = GameState.Playing;
            OnBoardChanged?.Invoke();
        }

        public bool PlaceNumber(int row, int col, int value)
        {
            if (State != GameState.Playing)
                return false;

            if (!GameValidator.IsValidMove(Board, row, col, value))
            {
                OnInvalidMove?.Invoke(row, col, value);
                return false;
            }

            var oldValue = Board.GetCell(row, col).Value;
            var move = new Move(row, col, oldValue, value);
            
            if (_moveHistoryIndex < _moveHistory.Count)
                _moveHistory.RemoveRange(_moveHistoryIndex, _moveHistory.Count - _moveHistoryIndex);

            _moveHistory.Add(move);
            _moveHistoryIndex++;

            // Đặt giá trị
            Board.SetCell(row, col, value);
            Board.RemoveNotesForValue(row, col, value);

            OnBoardChanged?.Invoke();

            if (GameValidator.IsGameWon(Board))
            {
                State = GameState.Won;
                OnGameWon?.Invoke();
            }

            return true;
        }

        public bool ClearCell(int row, int col)
        {
            if (State != GameState.Playing)
                return false;

            var cell = Board.GetCell(row, col);
            if (cell.IsGiven)
                return false;

            var oldValue = cell.Value;
            if (oldValue == 0)
                return false;

            var move = new Move(row, col, oldValue, 0);
            
            if (_moveHistoryIndex < _moveHistory.Count)
                _moveHistory.RemoveRange(_moveHistoryIndex, _moveHistory.Count - _moveHistoryIndex);

            _moveHistory.Add(move);
            _moveHistoryIndex++;

            Board.ClearCell(row, col);
            OnBoardChanged?.Invoke();

            return true;
        }

        public void ToggleNote(int row, int col, int value)
        {
            Board.GetCell(row, col).ToggleNote(value);
            OnBoardChanged?.Invoke();
        }

        public bool Undo()
        {
            if (_moveHistoryIndex <= 0)
                return false;

            _moveHistoryIndex--;
            var move = _moveHistory[_moveHistoryIndex];
            Board.SetCell(move.Row, move.Col, move.OldValue);
            OnBoardChanged?.Invoke();
            return true;
        }

        public bool Redo()
        {
            if (_moveHistoryIndex >= _moveHistory.Count)
                return false;

            var move = _moveHistory[_moveHistoryIndex];
            Board.SetCell(move.Row, move.Col, move.NewValue);
            _moveHistoryIndex++;
            OnBoardChanged?.Invoke();
            return true;
        }

        public bool CanUndo => _moveHistoryIndex > 0;
        public bool CanRedo => _moveHistoryIndex < _moveHistory.Count;
        public TimeSpan ElapsedTime => DateTime.Now - StartTime;

        public void Reset()
        {
            _moveHistoryIndex = 0;
            
            for (int r = 0; r < Board.BoardSize; r++)
            {
                for (int c = 0; c < Board.BoardSize; c++)
                {
                    var cell = Board.GetCell(r, c);
                    if (!cell.IsGiven && cell.Value != 0)
                    {
                        cell.Clear();
                        cell.Notes.Clear();
                    }
                }
            }

            _moveHistory.Clear();
            StartTime = DateTime.Now;
            State = GameState.Playing;
            OnBoardChanged?.Invoke();
        }

        public int[,] GetBoardArray()
        {
            return Board.ToArray();
        }

        public int[][] GetBoardJaggedArray()
        {
            return Board.ToJaggedArray();
        }
    }

    public enum GameState
    {
        Playing,
        Won,
        Lost,
        Paused
    }
}
