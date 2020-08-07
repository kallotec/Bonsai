using System;

namespace Bonsai.MapEditor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new EditorGame())
                game.Run();
        }
    }
}
