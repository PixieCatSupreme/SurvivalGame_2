using System;
using Mentula.Client;

namespace Client
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }
#endif
}