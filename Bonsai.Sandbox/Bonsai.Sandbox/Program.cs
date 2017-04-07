using Bonsai.Framework;
using Bonsai.Sandbox.Game;
using System;

namespace Bonsai.Sandbox
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new SandboxGame())
            {
                game.Run();
            }
        }
    }
#endif
}

