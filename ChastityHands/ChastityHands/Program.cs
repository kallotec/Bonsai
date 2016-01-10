using ChastityHands.Game;
using System;

namespace ChastityHands
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new GameManager())
            {
                game.Run();
            }
        }
    }
#endif
}

