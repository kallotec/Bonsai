using Skavenger.Game;
using System;

namespace Skavenger
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SkavengerGame())
                game.Run();
        }
    }
}
