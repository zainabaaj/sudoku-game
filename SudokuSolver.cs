using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuSolver
    {
        public const byte EmptyCell = 0;
        
        private SudokuGrid grid;
        bool solutionFound;
        public SudokuSolver(SudokuGrid grid)
        {
            this.grid = grid;
            solutionFound = false;
        }

        public void Solve()
        {
            while (OptimizeBeforeTheRecursion()) { }
            int emptyAfter = GetEmptyCellsCount();

            int topLeftEmptyX = 0;
            int topLeftEmptyY = 0;
            bool foundEmpty = false;

            for (int row = 0; row < SudokuGrid.SudokuDimension; row++)
            {
                for (int col = 0; col < SudokuGrid.SudokuDimension; col++)
                {
                    if (grid[row, col] == EmptyCell)
                    {
                        topLeftEmptyX = row;
                        topLeftEmptyY = col;
                        foundEmpty = true;
                        break;
                    }
                }
                if (foundEmpty) break;
            }

            if (!foundEmpty) return;

            this.SolveWithRecursion(topLeftEmptyX, topLeftEmptyY, emptyAfter);
        }

        private bool OptimizeBeforeTheRecursion()
        {
            bool optimized = false;
            for (int row = 0; row < SudokuGrid.SudokuDimension; row++)
            {
                for (int col = 0; col < SudokuGrid.SudokuDimension; col++)
                {
                    bool[] possibleDigits = GetPossibleDigits(row, col);
                    if (possibleDigits.Count(x => x) == 1)
                    {
                        for (int position = 0; position < 9; position++)
                        {
                            if (possibleDigits[position])
                            {
                                grid[row, col] = position + 1;
                                optimized = true;
                            }
                        }
                    }
                }
            }
            return optimized;
        }

        private bool[] GetPossibleDigits(int row, int col)
        {
            if (grid[row, col] != EmptyCell)
            {
                return new bool[9] 
                { false, false, false, false, false, false, false, false, false };
            }
             bool[] isPossibleNumber = new bool[9] { true, true, true, true, true, true, true, true, true };

            for (int colIndex = 0; colIndex < SudokuGrid.SudokuDimension; colIndex++)
            {
                if (grid[row, colIndex] != EmptyCell)
                {
                    isPossibleNumber[grid[row, colIndex] - 1] = false;
                }
            }

            for (byte rowIndex = 0; rowIndex < SudokuGrid.SudokuDimension; rowIndex++)
            {
                if (grid[rowIndex, col] != EmptyCell)
                {
                    isPossibleNumber[grid[rowIndex, col] - 1] = false;
                }
            }

            int topLeftRow = row - row % 3; 
            int topLeftCol = col - col % 3; 
            for (int rowIndex = topLeftRow; rowIndex < topLeftRow + 3; rowIndex++)
            {
                for (int colIndex = topLeftCol; colIndex < topLeftCol + 3; colIndex++)
                {
                    if (grid[rowIndex, colIndex] != EmptyCell)
                    {
                        isPossibleNumber[grid[rowIndex, colIndex] - 1] = false;
                    }
                }
            }
            return isPossibleNumber;
        }

        private int GetEmptyCellsCount()
        {
            int count = 0;
            for (int row = 0; row < SudokuGrid.SudokuDimension; row++)
            {
                for (int col = 0; col < SudokuGrid.SudokuDimension; col++)
                {
                    if (grid[row, col] == 0)
                    { 
                        count++; 
                    }
                }
            }
            return count;
        }

        private void SolveWithRecursion(int row, int col, int empty)
        {
            if (empty == 0)
            {
                solutionFound = true;
                return;
            }

            bool[] possibleDigits = GetPossibleDigits(row, col);

            for (int position = 0; position < 9; position++)
            {
                if (!possibleDigits[position])
                {
                    continue;
                }

                grid[row, col] = position + 1;

                
                int topLeftEmptyX = 0;
                int topLeftEmptyY = 0;
                bool foundEmpty = false;

                for (int rowIndex = row; rowIndex < SudokuGrid.SudokuDimension; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < SudokuGrid.SudokuDimension; colIndex++)
                    {
                        if (grid[rowIndex, colIndex] == EmptyCell)
                        {
                            topLeftEmptyX = rowIndex;
                            topLeftEmptyY = colIndex;
                            foundEmpty = true;
                            break;
                        }
                    }
                    if (foundEmpty) break;
                }

                SolveWithRecursion(topLeftEmptyX, topLeftEmptyY, empty - 1);
                if (solutionFound) return;
            }
            grid[row, col] = 0;
        }

    }
}

