#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Cubic_The_Game
{
    static class Program
    {
        private static CubicGame game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            game = new CubicGame();
            game.Run();
        }
    }
}
