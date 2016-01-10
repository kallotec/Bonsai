using System;

namespace BonsaiSandbox1
{
#if WINDOWS || XBOX
    static class Program
    {
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

