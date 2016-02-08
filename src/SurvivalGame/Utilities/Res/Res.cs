namespace Mentula.Utilities.Resources
{
    using Mentula.Utilities.Res;
    using System;

    public static class Res
    {
        public const int Range_C = 2;
        public const float DEG2RAD = 0.017453f;
        public const float FPS30 = 0.033333f;
        public const float FPS60 = 0.016666f;
        public const float MOVE_SPEED = 10;
        public const float DIFF = 0.8f;

        public static readonly string AppName;
        public static readonly int ChunkSize;
        public static readonly int MegaChunkSize;
        public static readonly int ChunkTileLength;
        public static readonly int TileSize;
        public static readonly int ClientDesync;
        public static readonly int MaxPlayers;
        public static readonly string Seed;

        static Res()
        {
            AppName = Resources.AppName;
            ChunkSize = int.Parse(Resources.ChunkSize);
            MegaChunkSize = int.Parse(Resources.MegaChunkSize);
            ChunkTileLength = ChunkSize * ChunkSize;
            TileSize = int.Parse(Resources.TileSize);
            ClientDesync = int.Parse(Resources.ClientTimeOut);
            MaxPlayers = int.Parse(Resources.MaxPlayers);
            Seed = DateTime.Now.ToString();
            Seed = "1";
        }
    }
}