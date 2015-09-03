namespace Mentula.Utilities.Resources
{
    using Mentula.Utilities.Res;

    public static class Res
    {
        public static string AppName;
        public const int Range_C = 1;
        public const float DEG2RAD = 0.017453f;

        public static int ChunkSize;
        public static int ChunkTileLength;
        public static int TileSize;
        public static int MaxPlayers;
        public static string Seed;

        static Res()
        {
            AppName = Resources.AppName;
            ChunkSize = int.Parse(Resources.ChunkSize);
            ChunkTileLength = ChunkSize * ChunkSize;
            TileSize = int.Parse(Resources.TileSize);
            MaxPlayers = int.Parse(Resources.MaxPlayers);
            Seed = "DesertPlz";
        }
    }
}