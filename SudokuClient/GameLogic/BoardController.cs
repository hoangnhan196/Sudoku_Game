using System;

namespace SudokuClient.GameLogic
{
    public class BoardController
    {
        private GameSession _gameSession;

        public int SelectedRow { get; set; } = -1;
        public int SelectedCol { get; set; } = -1;
        public bool HasSelection => SelectedRow >= 0 && SelectedCol >= 0;

        public event Action? OnBoardChanged;
        public event Action? OnGameWon;
        public event Action<int, int, int>? OnInvalidMove;

        public BoardController()
        {
            _gameSession = new GameSession();
            _gameSession.OnBoardChanged += () => OnBoardChanged?.Invoke();
            _gameSession.OnGameWon += () => OnGameWon?.Invoke();
            _gameSession.OnInvalidMove += (r, c, v) => OnInvalidMove?.Invoke(r, c, v);
        }

        public void LoadBoard(int[][] boardFromServer)
        {
            try
            {
                _gameSession.LoadBoard(boardFromServer);
                SelectedRow = -1;
                SelectedCol = -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading board: {ex.Message}");
            }
        }

        public void SelectCell(int row, int col)
        {
            if (row < 0 || row >= Board.BoardSize || col < 0 || col >= Board.BoardSize)
                return;

            SelectedRow = row;
            SelectedCol = col;
            OnBoardChanged?.Invoke();
        }

        public void ClearSelection()
        {
            SelectedRow = -1;
            SelectedCol = -1;
            OnBoardChanged?.Invoke();
        }

        public bool PlaceNumber(int row, int col, int value)
        {
            return _gameSession.PlaceNumber(row, col, value);
        }

        public bool ClearCell(int row, int col)
        {
            return _gameSession.ClearCell(row, col);
        }

        public void ToggleNote(int row, int col, int value)
        {
            _gameSession.ToggleNote(row, col, value);
        }

        public int GetCellValue(int row, int col)
        {
            try
            {
                return _gameSession.Board.GetCell(row, col).Value;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsCellGiven(int row, int col)
        {
            try
            {
                return _gameSession.Board.GetCell(row, col).IsGiven;
            }
            catch
            {
                return false;
            }
        }

        public System.Collections.Generic.HashSet<int> GetNotes(int row, int col)
        {
            try
            {
                return _gameSession.Board.GetCell(row, col).Notes;
            }
            catch
            {
                return new System.Collections.Generic.HashSet<int>();
            }
        }

        public bool HasConflict(int row, int col)
        {
            try
            {
                return _gameSession.Board.HasConflict(row, col);
            }
            catch
            {
                return false;
            }
        }

        public int[,] GetBoard()
        {
            return _gameSession.GetBoardArray();
        }

        public bool Undo()
        {
            return _gameSession.Undo();
        }

        public bool Redo()
        {
            return _gameSession.Redo();
        }

        public bool CanUndo => _gameSession.CanUndo;
        public bool CanRedo => _gameSession.CanRedo;

        public void Reset()
        {
            _gameSession.Reset();
            SelectedRow = -1;
            SelectedCol = -1;
        }

        public TimeSpan ElapsedTime => _gameSession.ElapsedTime;
        public GameState GameState => _gameSession.State;

        public void ApplyServerUpdate(int row, int col, int value)
        {
            try
            {
                _gameSession.Board.SetCell(row, col, value);
                _gameSession.Board.RemoveNotesForValue(row, col, value);
                OnBoardChanged?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying server update: {ex.Message}");
            }
        }

        public bool IsGameComplete()
        {
            return GameValidator.IsGameWon(_gameSession.Board);
        }

        public int[] GetPossibleValues(int row, int col)
        {
            try
            {
                return GameValidator.GetPossibleValues(_gameSession.Board, row, col);
            }
            catch
            {
                return new int[0];
            }
        }
    }
}
