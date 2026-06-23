using System;

namespace SudokuClient.GameLogic
{
    public class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public DateTime Timestamp { get; set; }

        public Move(int row, int col, int oldValue, int newValue)
        {
            Row = row;
            Col = col;
            OldValue = oldValue;
            NewValue = newValue;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"({Row}, {Col}): {OldValue} -> {NewValue} [{Timestamp:HH:mm:ss}]";
        }
    }
}
