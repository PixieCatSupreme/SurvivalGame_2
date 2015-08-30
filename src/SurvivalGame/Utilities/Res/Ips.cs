using System.Net;

namespace Mentula.Utilities.Resources
{
    public static class Ips
    {
        public const int PORT = 7777;

        public static IPEndPoint EndNico;
        public static IPEndPoint EndJoëll;
        public static IPEndPoint EndFrank;

        static Ips()
        {
            EndNico  = new IPEndPoint(new IPAddress(new byte[4] { 83, 161, 148, 175}), PORT);
            EndJoëll = new IPEndPoint(new IPAddress(new byte[4] { 83, 82, 128, 64 }), PORT);
            EndFrank = new IPEndPoint(new IPAddress(new byte[4] { 83, 82, 180, 172 }), PORT);
        }
    }
}