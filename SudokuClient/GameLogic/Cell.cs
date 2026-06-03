using System;
using System.Collections.Generic;

namespace SudokuClient.GameLogic
{
    public class Cell
    {
        public int Value { get; set; }
        public int InitialValue { get; set; }
        public bool IsGiven => InitialValue != 0;
        public HashSet<int> Notes { get; set; }

        public Cell(int value = 0)
        {
            Value = value;
            InitialValue = value;
            Notes = new HashSet<int>();
        }

        public bool SetValue(int value)
        {
            if (IsGiven)
                return false;

            if (value < 0 || value > 9)
                return false;

            Value = value;
            if (value != 0)
                Notes.Clear();

            return true;
        }

        public bool Clear()
        {
            if (IsGiven)
                return false;

            Value = 0;
            return true;
        }

        public void ToggleNote(int value)
        {
            if (Value != 0 || IsGiven || value < 1 || value > 9)
                return;

            if (Notes.Contains(value))
                Notes.Remove(value);
            else
                Notes.Add(value);
        }

        public void RemoveNote(int value)
        {
            Notes.Remove(value);
        }

        public Cell Clone()
        {
            var clone = new Cell(InitialValue)
            {
                Value = this.Value
            };
            foreach (var note in this.Notes)
                clone.Notes.Add(note);
            return clone;
        }
    }
}
