namespace Mentula.Utilities.Resources
{
    using Mentula.Utilities.Res;

    public static class Res
    {
        public static string AppName;

        public static int ChunkSize;
        public static int TileSize;
        public static int MaxPlayers;
        public static string Seed;

        static Res()
        {
            AppName = Resources.AppName;
            ChunkSize = int.Parse(Resources.ChunkSize);
            TileSize = int.Parse(Resources.TileSize);
            MaxPlayers = int.Parse(Resources.MaxPlayers);
            Seed = "s";
        }
    }
}