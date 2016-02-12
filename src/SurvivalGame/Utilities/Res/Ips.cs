using System.Net;

namespace Mentula.Utilities.Resources
{
    public static class Ips
    {
        public const int GAMEPORT = 7777;
        public const int CHATPORT = 7778;

        public static IPAddress Nico;
        public static IPAddress Joëll;
        public static IPAddress Frank;

        static Ips()
        {
            Nico = new IPAddress(new byte[4] { 83, 161, 148, 175 });
            Joëll = new IPAddress(new byte[4] { 83, 82, 128, 64 });
            Frank = new IPAddress(new byte[4] { 83, 82, 180, 172 });
        }
    }
}