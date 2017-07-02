using System;

namespace Sudoku
{
    class MenuCell:SudokuCell
    {
        private string label;
        public string Label
        {
            get { return label; }
            set { label = value; }
        }
        
        public MenuCell(int windowRowPosition, int windowColPosition, string displayName)
            : base(windowRowPosition, windowColPosition)
        {
            label = displayName;
        }

        public override void Redraw()
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

            Console.ForegroundColor = GameWindow.MenuForeGroundColor;

            Console.Write(label);
        }


    }
}
