namespace Mentula.Utilities.Resources
{
    using Mentula.Utilities.Res;
    using System;

    public static class Res
    {
        public const int Range_C = 1;
        public const float DEG2RAD = 0.017453f;

        public static readonly string AppName;
        public static readonly int ChunkSize;
        public static readonly int ChunkTileLength;
        public static readonly int TileSize;
        public static readonly int ClientDesync;
        public static readonly int MaxPlayers;
        public static readonly string Seed;

        static Res()
        {
            AppName = Resources.AppName;
            ChunkSize = int.Parse(Resources.ChunkSize);
            ChunkTileLength = ChunkSize * ChunkSize;
            TileSize = int.Parse(Resources.TileSize);
            ClientDesync = int.Parse(Resources.ClientTimeOut);
            MaxPlayers = int.Parse(Resources.MaxPlayers);
            Seed = DateTime.Now.ToString();
        }
    }
}