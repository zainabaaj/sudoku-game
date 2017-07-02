using System;
using System.Collections.Generic;
using System.IO;

namespace Sudoku
{
    class GameWindow
    {
        private const int WindowWidth = 60;
        private const int WindowHeight = 30;
        private const int FirstCellRowPosition = 8;
        private const int FirstCellColPosition = 18;
        private const int CellPositionRowIncrements = 2;
        private const int CellPositionColIncrements = 3;
        private const int MenuItemsCellRowPosition = 3;
        private readonly int[] MenuItemsCellColPositions = { 3, 12, 23, 36, 51 };


        public const ConsoleColor BackgroundColor = ConsoleColor.White;
        public const ConsoleColor BoxBorderColor = ConsoleColor.DarkCyan;
        public const ConsoleColor WrongEntryBackGroundColor = ConsoleColor.Red;
        public const ConsoleColor LockedEntryDigitColor = ConsoleColor.DarkGray;
        public const ConsoleColor EntryDigitColor = ConsoleColor.Black;
        public const ConsoleColor HighlightBackGroundColor = ConsoleColor.DarkCyan;
        public const ConsoleColor HighlightForeGroundColor = ConsoleColor.Black;
        public const ConsoleColor MenuForeGroundColor = ConsoleColor.Black;



        private const string WindowBoxFilePath = "GameBox.txt";
        private readonly string[] MenuItemLabels = { "easy", "medium", "hard", "Solve", "Exit" };

        private static bool gameIsRunning = true;

        public SudokuCell[,] cells;
        private MenuCell[] menuCells;
        public static SudokuCell CurrentCell;

        private SudokuGrid grid;
        public static int CurrentCellRowIndex;
        public static int CurrentCellColIndex;
        private MenuItem currentMenuItem;
        private int currentMenuItemIndex;

        public GameWindow()
        {
            grid = SudokuGenerator.Generate(SudokuGrid.Difficulty.Easy);
            InitWindow();
            InitCells(grid);
            currentMenuItem = MenuItem.None;
            currentMenuItemIndex = 0;
            gameIsRunning = true;
        }

        public void StartMainLoop()
        {
            while (gameIsRunning)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                    switch (pressedKey.Key)
                    {
                        
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.DownArrow:
                            MoveCursor(pressedKey.Key);
                            break;

                        
                        case ConsoleKey.Enter:
                            if (currentMenuItem != MenuItem.None)
                            {
                                OnMenuItemSelected(currentMenuItem);
                            }
                            break;

                        
                        case ConsoleKey.NumPad1:
                        case ConsoleKey.NumPad2:
                        case ConsoleKey.NumPad3:
                        case ConsoleKey.NumPad4:
                        case ConsoleKey.NumPad5:
                        case ConsoleKey.NumPad6:
                        case ConsoleKey.NumPad7:
                        case ConsoleKey.NumPad8:
                        case ConsoleKey.NumPad9:
                            if (CurrentCellRowIndex != -1)
                            {
                                cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect = true;
                                var value = pressedKey.KeyChar - '0';
                                grid.WriteNumber(CurrentCellRowIndex, CurrentCellColIndex, (pressedKey.KeyChar - '0'));


                                for (var i = 0; i < 9; i++)
                                {
                                    if (i != CurrentCellRowIndex)
                                    {
                                        if (value == cells[i, CurrentCellColIndex].value)
                                        {
                                            cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect = false;
                                            break;
                                        }
                                    }
                                    if (i != CurrentCellColIndex)
                                    {
                                        if (value == cells[CurrentCellRowIndex, i].value)
                                        {
                                            cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect = false;
                                            break;
                                        }
                                    }
                                }
                                if (cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect)
                                {
                                    List<int> subBox = new List<int>();
                                    int correntRow = CurrentCell.RowPosition;
                                    int correntCol = CurrentCell.ColumnPosition;
                                    if ((correntRow + 1) % 3 != 0)
                                    { correntRow--; }
                                    else if ((correntCol + 1) % 3 != 0)
                                    { correntCol--; }
                                    else
                                    {
                                        bool moveOn = cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect;
                                        for (int row = 0; row < 3 && moveOn; row++)
                                        {
                                            for (int col = 0; col < 3 && moveOn; col++)
                                            {
                                                //if ((row + 1) % 3 == 0 && (col + 1) % 3 == 0)
                                                //{
                                                if (row != CurrentCellRowIndex && col != CurrentCellColIndex)
                                                {
                                                    subBox.Add(cells[row, col].value);
                                                    subBox.Add(cells[row, col - 1].value);
                                                    subBox.Add(cells[row, col - 2].value);

                                                    subBox.Add(cells[row - 1, col].value);
                                                    subBox.Add(cells[row - 1, col - 1].value);
                                                    subBox.Add(cells[row - 1, col - 2].value);

                                                    subBox.Add(cells[row - 2, col].value);
                                                    subBox.Add(cells[row - 2, col - 1].value);

                                                }
                                                if (!IsBoxCorrect(subBox))
                                                {
                                                    cells[CurrentCellRowIndex, CurrentCellColIndex].isCorrect =
                                                    !subBox.Contains(value);
                                                    moveOn = false;

                                                    break;
                                                }





                                            }


                                        }

                                    }

                                }

                                RefreshCellValues();

                                //test
                                if (grid.EmptyCells == 0)
                                    Console.ReadKey(true);
                            }
                            break;

                        case ConsoleKey.Backspace:
                            if (CurrentCellRowIndex != -1)
                            {
                                grid.WriteNumber(CurrentCellRowIndex, CurrentCellColIndex, SudokuGrid.EmptyCellValue);
                                RefreshCellValues();
                            }
                            break;

                    }



                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                }
            }
        }

        public void StopMainLoop()
        {
            gameIsRunning = false;
        }

        private void InitCells(SudokuGrid grid)
        {
            CurrentCell = new SudokuCell(FirstCellRowPosition, FirstCellColPosition);
            CurrentCellRowIndex = 0;
            CurrentCellColIndex = 0;

            const int SudokuDim = SudokuGrid.SudokuDimension;
            cells = new SudokuCell[SudokuDim, SudokuDim];
            int currWindowRowPos = GameWindow.FirstCellRowPosition;
            int currWindowColPos = GameWindow.FirstCellColPosition;
            const int RowIncrement = GameWindow.CellPositionRowIncrements;
            const int ColIncrement = GameWindow.CellPositionColIncrements;

            for (int row = 0; row < SudokuDim; row++)
            {
                for (int col = 0; col < SudokuDim; col++)
                {
                    cells[row, col] = new SudokuCell(grid[row, col], grid.IsEditable(row, col), currWindowRowPos, currWindowColPos);
                    currWindowColPos += ColIncrement;
                }
                currWindowColPos = GameWindow.FirstCellColPosition;
                currWindowRowPos += RowIncrement;
            }

            menuCells = new MenuCell[MenuItemLabels.Length];
            for (int item = 0; item < menuCells.Length; item++)
            {
                menuCells[item] = new MenuCell(MenuItemsCellRowPosition, MenuItemsCellColPositions[item], MenuItemLabels[item]);
            }
            RedrawCells();
        }

        private void RedrawCells()
        {
            for (int row = 0; row < SudokuGrid.SudokuDimension; row++)
            {
                for (int col = 0; col < SudokuGrid.SudokuDimension; col++)
                {
                    cells[row, col].Redraw();
                }
            }

            for (int item = 0; item < menuCells.Length; item++)
            {
                menuCells[item].Redraw();
            }
        }

        private void RefreshCellValues()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col].Write(grid[row, col]);
                }
            }
        }

        private void InitWindow()
        {
            Console.SetWindowSize(WindowWidth, WindowHeight);
            Console.SetBufferSize(WindowWidth, WindowHeight);

            Console.CursorVisible = false;

            DrawWindow();
        }

        private void DrawWindow()
        {
            string windowBuffer = ReadWindowBoxFromFile(WindowBoxFilePath);

            Console.BackgroundColor = BackgroundColor;

            Console.ForegroundColor = BoxBorderColor;
            Console.SetCursorPosition(0, 0);
            Console.Write(windowBuffer);

            //Redraw the cells
            //RedrawCells();

        }

        private void MoveCursor(ConsoleKey direction)
        {
            switch (direction)
            {
                #region Left
                case ConsoleKey.LeftArrow:
                    if (CurrentCellRowIndex > -1)
                    {
                        if (CurrentCellColIndex > 0)
                        {
                            CurrentCell.ColumnPosition -= CellPositionColIncrements;
                            CurrentCellColIndex--;
                        }
                    }
                    else if (currentMenuItemIndex > 0)
                    {
                        currentMenuItemIndex--;
                        CurrentCell.ColumnPosition = MenuItemsCellColPositions[currentMenuItemIndex];
                        SettMenuItemFromMenuIndex();
                    }
                    break;
                #endregion
                #region Right
                case ConsoleKey.RightArrow:
                    if (CurrentCellRowIndex > -1)
                    {
                        if (CurrentCellColIndex < SudokuGrid.SudokuDimension - 1)
                        {
                            CurrentCell.ColumnPosition += CellPositionColIncrements;
                            CurrentCellColIndex++;
                        }
                    }
                    else if (currentMenuItemIndex < MenuItemsCellColPositions.Length - 1)
                    {
                        currentMenuItemIndex++;
                        CurrentCell.ColumnPosition = MenuItemsCellColPositions[currentMenuItemIndex];
                        SettMenuItemFromMenuIndex();
                    }
                    break;
                #endregion
                #region Up
                case ConsoleKey.UpArrow:
                    if (CurrentCellRowIndex > 0)
                    {
                        CurrentCell.RowPosition -= CellPositionRowIncrements;
                        CurrentCellRowIndex--;
                    }
                    else if (CurrentCellRowIndex == 0)
                    {
                        CurrentCellRowIndex = -1;
                        CurrentCell.RowPosition = MenuItemsCellRowPosition;

                        if (CurrentCellColIndex < 2)
                        {
                            currentMenuItemIndex = 1;
                        }
                        else if (CurrentCellColIndex < 4)
                        {
                            currentMenuItemIndex = 2;
                        }
                        else if (CurrentCellColIndex < 7)
                        {
                            currentMenuItemIndex = 3;
                        }
                        else
                        {
                            currentMenuItemIndex = 4;
                        }
                        CurrentCell.ColumnPosition = MenuItemsCellColPositions[currentMenuItemIndex];
                        SettMenuItemFromMenuIndex();
                    }
                    break;
                #endregion
                #region Down
                case ConsoleKey.DownArrow:
                    if (CurrentCellRowIndex != -1 && CurrentCellRowIndex < SudokuGrid.SudokuDimension - 1)
                    {
                        CurrentCell.RowPosition += CellPositionRowIncrements;
                        CurrentCellRowIndex++;
                    }
           
                    else if (CurrentCellRowIndex == -1)
                    {
                        CurrentCellRowIndex = 0;
                        CurrentCell.RowPosition = FirstCellRowPosition;
                        currentMenuItem = MenuItem.None;

                        if (CurrentCell.ColumnPosition == MenuItemsCellColPositions[0]
                            || CurrentCell.ColumnPosition == MenuItemsCellColPositions[1])
                        {
                            CurrentCell.ColumnPosition = cells[0, 0].ColumnPosition;
                            CurrentCellColIndex = 0;
                        }
                        else if (CurrentCell.ColumnPosition == MenuItemsCellColPositions[2])
                        {
                            CurrentCell.ColumnPosition = cells[0, 2].ColumnPosition;
                            CurrentCellColIndex = 2;
                        }
                        else if (CurrentCell.ColumnPosition == MenuItemsCellColPositions[3])
                        {
                            CurrentCell.ColumnPosition = cells[0, 5].ColumnPosition;
                            CurrentCellColIndex = 4;
                        }
                        else
                        {
                            CurrentCell.ColumnPosition = cells[0, 8].ColumnPosition;
                            CurrentCellColIndex = 8;
                        }
                    }
                    break;
                #endregion
            }
            RedrawCells();
        }

        private void SettMenuItemFromMenuIndex()
        {
            switch (currentMenuItemIndex)
            {
                case 0:
                    currentMenuItem = MenuItem.easy;
                    break;
                case 1:
                    currentMenuItem = MenuItem.medium;
                    break;
                case 2:
                    currentMenuItem = MenuItem.hard;
                    break;
                case 3:
                    currentMenuItem = MenuItem.Solve;
                    break;
                case 4:
                    currentMenuItem = MenuItem.Exit;
                    break;
            }
        }


        private void OnMenuItemSelected(MenuItem item)
        {
            switch (item)
            {
                case MenuItem.easy:
                    NewGame(SudokuGrid.Difficulty.Easy);
                    break;
                case MenuItem.medium:
                    NewGame(SudokuGrid.Difficulty.Medium);
                    break;
                case MenuItem.hard:
                    NewGame(SudokuGrid.Difficulty.Hard);
                    break;

                case MenuItem.Solve:
                    SolveSudoku();
                    break;
                case MenuItem.Exit:
                    Close();
                    break;
            }
        }

        private static string ReadWindowBoxFromFile(string filepath)
        {
            string output = null;
            using (StreamReader reader = new StreamReader(filepath))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }

        private void NewGame(SudokuGrid.Difficulty difficulty)
        {
            grid = SudokuGenerator.Generate(difficulty);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col].RefreshValues(grid[row, col], grid.IsEditable(row, col));
                }
            }
            RefreshCellValues();

        }

        private void SolveSudoku()
        {
            grid.ClearUserCells();

            SudokuSolver solver = new SudokuSolver(grid);
            solver.Solve();
            RefreshCellValues();
        }

        public void Close()
        {
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.SetWindowSize(80, 25);
            Console.SetBufferSize(80, 300);

            Console.CursorVisible = true;

            StopMainLoop();

        }
        private static bool IsBoxCorrect(IList<int> subBox)
        {
            int boxNumberPresent = 0;

            for (int number = 0; number < subBox.Count; number++)
            {
                for (int numberInRow = 0; numberInRow < subBox.Count; numberInRow++)
                {
                    if (subBox[number] == subBox[numberInRow])
                    {
                        boxNumberPresent++;
                        if (boxNumberPresent > 1)
                        {
                            return false;
                        }
                    }
                }
                boxNumberPresent = 0;
            }
            return true;
        }

        enum MenuItem
        {
            easy,
            medium,
            hard,
            Solve,
            Exit,
            None
        }
    }
}
