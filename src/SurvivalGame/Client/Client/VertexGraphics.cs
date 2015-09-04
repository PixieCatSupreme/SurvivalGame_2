//#define DRUNK

using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Matrix3 = Mentula.Engine.Core.Matrix3;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class VertexGraphics : GraphicsDeviceManager
    {
        private float SCALE = 2f;
        private float ROT = 0;
        private bool invertScale = false;

        public KeyValuePair<string, Actor>[] Players;
        public Camera Camera { get; private set; }

        private SpriteBatch batch;
        private FPS fpsCounter;

        private TextureCollection textures;
        private FontCollection fonts;

        private Chunk[] chunks;
        private Vector2[] vertexBuffer;
        private float heroR;

        public VertexGraphics(Game game)
            :base(game)
        {
            Camera = new Camera();
            fpsCounter = new FPS();

            chunks = new Chunk[0];
            vertexBuffer = new Vector2[0];
            Players = new KeyValuePair<string, Actor>[0];
        }

        public void Load(ContentManager content)
        {
            batch = new SpriteBatch(GraphicsDevice);
            textures = new TextureCollection(content);
            fonts = new FontCollection(content, 3);
            Camera.SetOffset(new Vect2((PreferredBackBufferWidth >> 1) - (Res.TileSize >> 1), (PreferredBackBufferHeight >> 1) - (Res.TileSize >> 1)));

            textures.LoadFromConfig("R/Textures");
            fonts.Load("Fonts/", "ConsoleFont", "MenuFont", "NameFont");
        }

        public void Update(Actor hero, float delta)
        {
#if DRUNK
            ROT += 10 * delta;
            if (SCALE > 10 || SCALE < 2) invertScale = !invertScale;
            SCALE += (invertScale ? -1 : 1) * delta;
#endif

            Vector2 pos = Chunk.GetTotalPos(hero.ChunkPos, hero.Pos);
            Matrix3 view = Matrix3.Identity;
            view *= Matrix3.ApplyScale(SCALE);
            view *= Matrix3.ApplyRotation(ROT * Res.DEG2RAD);
            view *= Matrix3.ApplyTranslation(new Vect2(pos.X, pos.Y));
            Camera.Update(view);
            heroR = hero.Rotation;
        }

        public void Draw(float delta)
        {
            fpsCounter.Update(delta);
            Camera.Transform(ref chunks, ref vertexBuffer);

            GraphicsDevice.Clear(Color.LimeGreen);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            int index = 0;
            Vector2 midTexture = new Vector2(Res.TileSize >> 1, Res.TileSize >> 1);
            for (int i = 0; i < chunks.Length; i++)
            {
                Chunk chunk = chunks[i];
                for (int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = vertexBuffer[index];
                    batch.Draw(textures[chunk.Tiles[j].Tex], pos, null, Color.White, ROT * Res.DEG2RAD, midTexture, SCALE, 0, 0.5f);
                    index++;
                }
            }

            batch.Draw(textures[9997], new Vector2(Camera.Offset.X, Camera.Offset.Y), null, Color.White, heroR + (ROT * Res.DEG2RAD), midTexture, SCALE, 0, 0);

            batch.DrawString(fonts["ConsoleFont"], fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red);

            batch.End();
        }

        public void UpdateChunks(ref Chunk[] chunks)
        {
            this.chunks = chunks;
            vertexBuffer = new Vector2[chunks.Length * Res.ChunkTileLength];
        }
    }
}