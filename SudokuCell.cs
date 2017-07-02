using System;

namespace Sudoku
{
    class SudokuCell
    {
        const char EmptyCellDisplayChar = ' ';
        
        public int value;
        private bool isEditable;
        public bool isCorrect;
        protected int windowRowPosition;
        protected int windowColPosition;

        public int RowPosition
        {
            get { return windowRowPosition; }
            set { windowRowPosition = value; }
        }

        public int ColumnPosition
        {
            get { return windowColPosition; }
            set { windowColPosition = value; }
        }

        public SudokuCell(int value, bool isEditable, int windowRowPosition, int windowColPosition)
        {
            this.value = value;
            this.isEditable = isEditable;
            this.windowRowPosition = windowRowPosition;
            this.windowColPosition = windowColPosition;
            isCorrect = true;
        }

        public SudokuCell(int windowRowPosition, int windowColPosition)
        {
            this.windowRowPosition = windowRowPosition;
            this.windowColPosition = windowColPosition;
        }

        public void Write(int newValue)
        {
            value = newValue;
            Redraw();
        }

        public virtual void Redraw()
        {
            Console.SetCursorPosition(windowColPosition, windowRowPosition);
            if (GameWindow.CurrentCell.HasSamePositionAs(this))
            {
                Console.BackgroundColor = GameWindow.HighlightBackGroundColor;
            }
            else
            {
                Console.BackgroundColor = GameWindow.BackgroundColor;
            }

            if (isEditable)
            {
                Console.ForegroundColor = GameWindow.EntryDigitColor;
            }
            else
            {
                Console.ForegroundColor = GameWindow.LockedEntryDigitColor;
            }

            if (!isCorrect && isEditable)
            {
                Console.ForegroundColor = GameWindow.WrongEntryBackGroundColor;
            }

            if (value != 0)
            {
                Console.Write(value);
            }
            else
                Console.Write(EmptyCellDisplayChar);
        }

        public void RefreshValues(int value, bool isEditable)
        {
            this.value = value;
            this.isEditable = isEditable;
        }

        public bool HasSamePositionAs(SudokuCell other)
        {
            if (this.windowRowPosition == other.windowRowPosition && this.windowColPosition == other.windowColPosition)
            {
                return true;
            }
            return false;
        }
    }
}
