//#define DRUNK

using Matrix3 = Mentula.Engine.Core.Matrix3;
using Vect2 = Mentula.Engine.Core.Vect2;
using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Client
{
    public class VertexGraphics : GraphicsDeviceManager
    {
        private float SCALE = 2;
        private float ROT = 0;
        private bool invertScale = false;

        public Camera Camera { get; private set; }
        private SpriteBatch batch;
        private FPS fpsCounter;

        private TextureCollection textures;
        private FontCollection fonts;

        private Chunk[] chunks;
        private Vector2[] vertexBuffer;

        public VertexGraphics(Game game)
            :base(game)
        {
            Camera = new Camera();
            fpsCounter = new FPS();

            chunks = new Chunk[0];
            vertexBuffer = new Vector2[0];
        }

        public void Load(ContentManager content)
        {
            batch = new SpriteBatch(GraphicsDevice);
            textures = new TextureCollection(content);
            fonts = new FontCollection(content, 3);

            textures.LoadFromConfig("R/Textures");
            fonts.Load("Fonts/", "ConsoleFont", "MenuFont", "NameFont");
        }

        public void Update(IntVector2 chunkPos, Vector2 tilePos, float delta)
        {
#if DRUNK
            ROT += 10 * delta;
            if (SCALE > 10 || SCALE < 2) invertScale = !invertScale;
            SCALE += (invertScale ? -1 : 1) * delta;
#endif

            Vector2 pos = Chunk.GetTotalPos(chunkPos, tilePos);
            Matrix3 view = Matrix3.Identity;
            view *= Matrix3.ApplyScale(SCALE);
            view *= Matrix3.ApplyRotation(ROT * Res.DEG2RAD);
            view *= Matrix3.ApplyTranslation(new Vect2(pos.X, pos.Y));
            Camera.Update(view);
        }

        public void Draw(float delta)
        {
            fpsCounter.Update(delta);
            Camera.Transform(ref chunks, ref vertexBuffer);

            GraphicsDevice.Clear(Color.LimeGreen);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            batch.DrawString(fonts["ConsoleFont"], fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red);

            int index = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                Chunk chunk = chunks[i];
                for (int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = vertexBuffer[index];
                    batch.Draw(textures[chunk.Tiles[j].Tex], pos, null, Color.White, ROT * Res.DEG2RAD, Vector2.Zero, SCALE, 0, 1);
                    index++;
                }
            }

            batch.End();
        }

        public void UpdateChunks(ref Chunk[] chunks)
        {
            this.chunks = chunks;
            vertexBuffer = new Vector2[chunks.Length * Res.ChunkTileLength];
        }
    }
}