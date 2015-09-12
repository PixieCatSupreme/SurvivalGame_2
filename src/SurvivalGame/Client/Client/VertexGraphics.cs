//#define DRUNK

#pragma warning disable 67

using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public bool Enabled { get { return true; } }
        public int UpdateOrder { get { return 0; } }
        public int DrawOrder { get { return 0; } }
        public bool Visible { get { return true; } }

        private SpriteBatch batch;
        private FPS fpsCounter;
        private MainGame game;

        private TextureCollection textures;
        private FontCollection fonts;

        private readonly Vector2 midTexture;
        private readonly Vector2 nameOffset;

        private Vector2[] tileBuffer;
        private Vector2[] creatureBuffer;
        private Vector2[] actorBuffer;
        private float heroR;

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

            midTexture = new Vector2(Res.TileSize >> 1, Res.TileSize >> 1);
            nameOffset = new Vector2(0, -32);

            tileBuffer = new Vector2[0];
            creatureBuffer = new Vector2[0];
            actorBuffer = new Vector2[0];
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
            Camera.Transform(ref game.chunks, ref tileBuffer, ref creatureBuffer);
            Camera.Transform(ref game.players, ref actorBuffer);

            int tileIndex = 0, creatureIndex = 0;
            SpriteFont nameFont = fonts["NameFont"];

            GraphicsDevice.Clear(Color.LimeGreen);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            for (int i = 0; i < game.chunks.Length; i++)
            {
                Chunk chunk = game.chunks[i];
                for (int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = tileBuffer[tileIndex];
                    DrawBatch(textures[chunk.Tiles[j].Tex], pos, ROT * Res.DEG2RAD);
                    tileIndex++;
                }
            }

            for (int i = 0; i < game.chunks.Length; i++)
            {
                Chunk chunk = game.chunks[i];
                for (int j = 0; j < chunk.Creatures.Length; j++)
                {
                    Vector2 pos = creatureBuffer[creatureIndex];
                    NPC npc = chunk.Creatures[j];

                    DrawBatch(textures[npc.TextureId], pos, Rot(npc.Rotation));
                    batch.DrawString(nameFont, npc.HealthPrec + " | " + npc.Name, pos + nameOffset, Color.Red);
                    creatureIndex++;
                }
            }

            for (int i = 0; i < game.players.Length; i++)
            {
                Vector2 pos = actorBuffer[i];
                NPC actor = game.players[i];

                DrawBatch(textures[9997], pos, Rot(actor.Rotation));
                batch.DrawString(nameFont, actor.HealthPrec + " | " + actor.Name, pos + nameOffset, Color.Red);
            }

            Vector2 heroPos = new Vector2(Camera.Offset.X, Camera.Offset.Y);
            DrawBatch(textures[9997], heroPos, Rot(heroR));

            batch.DrawString(fonts["ConsoleFont"], fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red);
            batch.End();
        }

        public void UpdateChunks(ref Chunk[] chunks)
        {
            int tileLength = chunks.Length * Res.ChunkTileLength, creatureLength = 0;

            for (int i = 0; i < chunks.Length; i++) creatureLength += chunks[i].Creatures.Length;

            if (tileLength != tileBuffer.Length) tileBuffer = new Vector2[tileLength];
            if (creatureLength != creatureBuffer.Length) creatureBuffer = new Vector2[creatureLength];
        }

        public void UpdatePlayers(int newLength)
        {
            if (newLength != actorBuffer.Length) actorBuffer = new Vector2[newLength];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Rot(float rot) { return ROT * Res.DEG2RAD + rot + 1.5707963f; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawBatch(Texture2D tex, Vector2 pos, float rot) { batch.Draw(tex, pos, null, Color.White, rot, midTexture, SCALE, 0, 0); }
    }
}