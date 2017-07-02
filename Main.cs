using System;

namespace Sudoku
{
    class MainEntry
    {
        static void Main()
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.StartMainLoop();
        }
    }
}
