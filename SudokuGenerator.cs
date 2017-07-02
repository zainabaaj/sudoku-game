using System;

namespace Sudoku
{
    static class SudokuGenerator
    {
        #region Ready Sudokus' to transform and generate new puzzles.
        private static readonly int[][,] ReadySudokus = 
            {
                 new int[,]
                 {
                    {9, 3, 6,  5, 1, 7,  4, 2, 8},
                    {8, 5, 2,  9, 4, 6,  3, 7, 1},
                    {1, 7, 4,  3, 8, 2,  6, 5, 9},

                    {4, 6, 7,  8, 3, 9,  5, 1, 2},
                    {3, 9, 5,  2, 7, 1,  8, 4, 6},
                    {2, 8, 1,  6, 5, 4,  7, 9, 3},

                    {7, 2, 9,  4, 6, 3,  1, 8, 5},
                    {5, 1, 3,  7, 2, 8,  9, 6, 4},
                    {6, 4, 8,  1, 9, 5,  2, 3, 7}
                 },

                 new int[,]
                 {
                    {8, 2, 7,  1, 5, 4,  3, 9, 6},
                    {9, 6, 5,  3, 2, 7,  1, 4, 8},
                    {3, 4, 1,  6, 8, 9,  7, 5, 2},

                    {5, 9, 3,  4, 6, 8,  2, 7, 1},
                    {4, 7, 2,  5, 1, 3,  6, 8, 9},
                    {6, 1, 8,  9, 7, 2,  4, 3, 5},

                    {7, 8, 6,  2, 3, 5,  9, 1, 4},
                    {1, 5, 4,  7, 2, 6,  8, 2, 3},
                    {2, 3, 9,  8, 4, 1,  5, 6, 7}
                 },

                 

                 new int[,]
                 {
                    {6, 8, 5,  3, 2, 4,  1, 7, 9},
                    {7, 2, 1,  5, 6, 9,  4, 8, 3},
                    {9, 4, 3,  7, 8, 1,  2, 5, 6},

                    {2, 6, 9,  1, 7, 3,  8, 4, 5},
                    {5, 1, 4,  6, 9, 8,  3, 2, 7},
                    {3, 7, 8,  4, 5, 2,  6, 9, 1},

                    {5, 8, 2,  9, 1, 6,  7, 3, 4},
                    {1, 3, 7,  8, 4, 5,  9, 6, 2},
                    {4, 9, 6,  2, 3, 7,  5, 1, 8}
                 }

                 
            };
        #endregion

        private const int RowSwapsLimit = 10;
        private const int GroupSwapsLimit = 10;
        private const int TransposeLimit = 5;

        public const int EasyDifficultyInitEmptyCells = 45;
        public const int MediumDifficultyInitEmptyCells = 55;
        public const int HardDifficultyInitEmptyCells = 60;
        public const int EasyDifficultyMaxEmptyRows = 0;
        public const int MediumDifficultyMaxEmptyRows = 0;
        public const int HardDifficultyMaxEmptyRows = 1;

        private static readonly Random randGenerator = new Random();
       
        public static SudokuGrid Generate(SudokuGrid.Difficulty difficulty)
        {
            SudokuGrid generatedGrid = new SudokuGrid(ReadySudokus[randGenerator.Next(0, ReadySudokus.Length - 1)], difficulty, false);
            
            for (int transposeCount = 0; transposeCount < TransposeLimit; transposeCount++)
            {
                for (int groupSwapCount = 0; groupSwapCount < GroupSwapsLimit; groupSwapCount++)
                {
                    for (int rowSwapCount = 0; rowSwapCount < RowSwapsLimit; rowSwapCount++)
                    {
                        generatedGrid.Transform(SudokuGrid.TransformType.SwapRows);
                    }
                    generatedGrid.Transform(SudokuGrid.TransformType.SwapGroups);
                }
                generatedGrid.Transform(SudokuGrid.TransformType.Transpose);
            }
            
            #region Remove some numbers.
            
            int cellsToRemove = 0, emptyRowsLimit = 0;
            switch (difficulty)
            { 
                case SudokuGrid.Difficulty.Easy:
                    cellsToRemove = EasyDifficultyInitEmptyCells;
                    emptyRowsLimit = EasyDifficultyMaxEmptyRows;
                    break;
                case SudokuGrid.Difficulty.Medium:
                    cellsToRemove = MediumDifficultyInitEmptyCells;
                    emptyRowsLimit = MediumDifficultyMaxEmptyRows;
                    break;
                case SudokuGrid.Difficulty.Hard:
                    cellsToRemove = HardDifficultyInitEmptyCells;
                    emptyRowsLimit = HardDifficultyMaxEmptyRows;
                    break;
            }
            generatedGrid.EmptyCells = cellsToRemove;

           int cellsRemoved = 0;
            int rowsRemoved = 0;
            int row, col;
            const int IndexLimit = SudokuGrid.SudokuDimension - 1;
            while (cellsRemoved <= cellsToRemove)
            {
                row = randGenerator.Next(0, IndexLimit);
                col = randGenerator.Next(0, IndexLimit);
                
                if (!(generatedGrid.IsSingleInRow(row, col) && rowsRemoved > emptyRowsLimit))
                {
                    generatedGrid.ClearCell(row, col);
                    cellsRemoved++;
                }
            }

            #endregion
            
            return generatedGrid;
        }
            
    }
}
