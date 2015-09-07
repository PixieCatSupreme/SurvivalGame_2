//#define DRUNK

#pragma warning disable 67

using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Matrix3 = Mentula.Engine.Core.Matrix3;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class VertexGraphics : GraphicsDeviceManager, IGameComponent, IUpdateable, IDrawable
    {
        private float SCALE = 2f;
        private float ROT = 0;
        private bool invertScale = false;

        public Camera Camera { get; private set; }

        private SpriteBatch batch;
        private FPS fpsCounter;
        private MainGame game;

        private TextureCollection textures;
        private FontCollection fonts;

        private readonly Vector2 midTexture;
        private readonly Vector2 nameOffset;

        private Chunk[] chunks;
        private Player[] players;
        private Vector2[] vertexBuffer;
        private Vector2[] actorBuffer;
        private float heroR;

        public bool Enabled { get { return true; } }
        public int UpdateOrder { get { return 0; } }
        public int DrawOrder { get { return 0; } }
        public bool Visible { get { return true; } }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public VertexGraphics(MainGame game)
            : base(game)
        {
            Camera = new Camera();
            fpsCounter = new FPS();
            this.game = game;

            chunks = new Chunk[0];
            vertexBuffer = new Vector2[0];
            actorBuffer = new Vector2[0];
            midTexture = new Vector2(Res.TileSize >> 1, Res.TileSize >> 1);
            nameOffset = new Vector2(0, -32);
            players = new Player[0];
        }

        public void Initialize()
        {
            batch = new SpriteBatch(GraphicsDevice);
            textures = new TextureCollection(game.Content);
            fonts = new FontCollection(game.Content, 3);
            Camera.SetOffset(new Vect2((PreferredBackBufferWidth >> 1) - (Res.TileSize >> 1), (PreferredBackBufferHeight >> 1) - (Res.TileSize >> 1)));

            textures.LoadFromConfig("R/Textures");
            fonts.Load("Fonts/", "ConsoleFont", "MenuFont", "NameFont");
        }

        public void Update(GameTime gametTime)
        {
#if DRUNK
            ROT += 10 * delta;
            if (SCALE > 10 || SCALE < 2) invertScale = !invertScale;
            SCALE += (invertScale ? -1 : 1) * delta;
#endif

            Vector2 pos = Chunk.GetTotalPos(game.hero.ChunkPos, game.hero.Pos);
            Matrix3 view = Matrix3.Identity;
            view *= Matrix3.ApplyScale(SCALE);
            view *= Matrix3.ApplyRotation(ROT * Res.DEG2RAD);
            view *= Matrix3.ApplyTranslation(new Vect2(pos.X, pos.Y));
            Camera.Update(view);
            heroR = game.hero.Rotation;
        }

        public void Draw(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fpsCounter.Update(delta);
            Camera.Transform(ref chunks, ref vertexBuffer);
            Camera.Transform(ref players, ref actorBuffer);

            int index = 0;

            GraphicsDevice.Clear(Color.LimeGreen);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

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

            for (int i = 0; i < players.Length; i++)
            {
                string name = players[i].Name;
                string text = string.Format("{0} | {1}", players[i].HealthPrec, name);

                if (name == Environment.UserName)
                {
                    Vector2 pos = new Vector2(Camera.Offset.X, Camera.Offset.Y);
                    batch.Draw(textures[9997], pos, null, Color.White, heroR + 1.5707963f + (ROT * Res.DEG2RAD), midTexture, SCALE, 0, 0);
                    batch.DrawString(fonts["NameFont"], text, pos + nameOffset, Color.Red);
                }
                else
                {
                    batch.Draw(textures[9997], actorBuffer[i], null, Color.White, players[i].Rotation + 1.5707963f + (ROT * Res.DEG2RAD), midTexture, SCALE, 0, 0);
                    batch.DrawString(fonts["NameFont"], text, actorBuffer[i] + nameOffset, Color.Red);
                }
            }

            batch.DrawString(fonts["ConsoleFont"], fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red);

            batch.End();
        }

        public void UpdateChunks(ref Chunk[] chunks)
        {
            this.chunks = chunks;
            if (chunks.Length != vertexBuffer.Length) vertexBuffer = new Vector2[chunks.Length * Res.ChunkTileLength];
        }

        public void UpdatePlayers(Player[] players)
        {
            this.players = players;
            if (players.Length != actorBuffer.Length) actorBuffer = new Vector2[players.Length];
        }
    }
}