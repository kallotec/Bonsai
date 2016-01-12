using System;

namespace Bonsai.Samples.Platformer2D
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new PlatformerGame())
            {
                game.Run();
            }
        }
    }
#endif
}

