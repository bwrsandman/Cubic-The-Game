using System;

namespace Cubic_The_Game
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CubicGame game = new CubicGame())
            {
                game.Run();
            }
        }
    }
#endif
}

