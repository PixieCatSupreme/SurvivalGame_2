using System;

namespace Mentula.Server
{
    static class Program
    {
        static void Main(string[] args)
        {
            using(Server s = new Server())
            {
                s.Run();
            }
        }
    }
}