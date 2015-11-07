﻿//#define DRUNK
//#define VSCYNC

#pragma warning disable 67

using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Matrix3 = Mentula.Engine.Core.Matrix3;
using Vect2 = Mentula.Engine.Core.Vect2;

namespace Mentula.Client
{
    public class MediumGraphics : GraphicsDeviceManager, IGameComponent, IUpdateable, IDrawable
    {
        internal float SCALE = 2f;
        private float ROT = 0;
        private bool invertScale = false;

        public Camera Camera { get; private set; }
        public bool Enabled { get { return true; } }
        public int UpdateOrder { get { return 0; } }
        public int DrawOrder { get { return 0; } }
        public bool Visible { get; set; }

        private SpriteBatch batch;
        private FPS fpsCounter;
        private MainGame game;

        public TextureCollection textures;
        public FontCollection fonts;

        private readonly Vector2 midTexture;
        private readonly Vector2 nameOffset;

        private Vector2[] tileBuffer;
        private Vector2[] destrBuffer;
        private Vector2[] actorBuffer;
        private float heroR;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public MediumGraphics(MainGame game)
            : base(game)
        {
            Camera = new Camera();
            fpsCounter = new FPS();
            this.game = game;

            midTexture = new Vector2(Res.TileSize >> 1, Res.TileSize >> 1);
            nameOffset = new Vector2(0, -32);

            tileBuffer = new Vector2[0];
            destrBuffer = new Vector2[0];
            actorBuffer = new Vector2[0];

            PreferredBackBufferHeight = 600;
            PreferredBackBufferWidth = 800;
#if !VSCYNC
            SynchronizeWithVerticalRetrace = false;
            game.IsFixedTimeStep = false;
#endif
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
            Camera.Transform(ref game.chunks, ref tileBuffer, ref destrBuffer);
            Camera.Transform(ref game.npcs, ref actorBuffer);

            int tileIndex = 0, destrIndex = 0;
            SpriteFont nameFont = fonts["NameFont"];

            GraphicsDevice.Clear(Color.LimeGreen);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            for (int i = 0; i < game.chunks.Length; i++)
            {
                Chunk chunk = game.chunks[i];

                for (int j = 0; j < chunk.Tiles.Length; j++)
                {
                    Vector2 pos = tileBuffer[tileIndex++];
                    DrawBatch(textures[chunk.Tiles[j].Tex], pos, ROT * Res.DEG2RAD);
                }
            }

            for (int i = 0; i < game.npcs.Length; i++)
            {
                Vector2 pos = actorBuffer[i];
                NPC actor = game.npcs[i];

                DrawBatch(textures[actor.TextureId], pos, Rot(actor.Rotation));
                DrawString(nameFont, actor.HealthPrec + " | " + actor.Name, pos + nameOffset, Color.Red);
            }

            Vector2 heroPos = new Vector2(Camera.Offset.X, Camera.Offset.Y);
            DrawBatch(textures[9997], heroPos, Rot(heroR));

            for (int i = 0; i < game.chunks.Length; i++)
            {
                Chunk chunk = game.chunks[i];

                for (int j = 0; j < chunk.Destrucables.Length; j++)
                {
                    Vector2 pos = destrBuffer[destrIndex++];
                    Texture2D t = textures[chunk.Destrucables[j].Tex];
                    DrawBatch(t, pos, ROT * Res.DEG2RAD, t.Bounds.Width >> 1, t.Bounds.Height >> 1);
                }
            }

            MouseState mState = Mouse.GetState();
            float adder = 8 * SCALE;
            Vector2 mousePos = new Vector2(mState.X + adder, mState.Y + adder);
            DrawBatch(textures[9998], mousePos, 0);

#if DEBUG
            batch.DrawString(fonts["ConsoleFont"], fpsCounter.Avarage.ToString(), Vector2.Zero, Color.Red);
#endif
            batch.End();
        }

        public void UpdateChunks(ref Chunk[] chunks, ref NPC[] npcs)
        {
            int tileLength = chunks.Length * Res.ChunkTileLength;
            int destrLength = 0;
            for (int i = 0; i < chunks.Length; i++) destrLength += chunks[i].Destrucables.Length;

            if (tileLength != tileBuffer.Length) tileBuffer = new Vector2[tileLength];
            if (destrLength != destrBuffer.Length) destrBuffer = new Vector2[destrLength];
            if (npcs.Length != actorBuffer.Length) actorBuffer = new Vector2[npcs.Length];
        }

        public static void ChangeWindowBorder(IntPtr handle, byte newType)
        {
            Control window = Control.FromHandle(handle);
            window.FindForm().FormBorderStyle = (FormBorderStyle)newType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Rot(float rot) 
        { return ROT * Res.DEG2RAD + rot + 1.5707963f; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawBatch(Texture2D tex, Vector2 pos, float rot) 
        { batch.Draw(tex, pos, null, Color.White, rot, midTexture, SCALE, 0, 0); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawBatch(Texture2D tex, Vector2 pos, float rot, float xDiff, float yDiff)
        { batch.Draw(tex, pos, null, Color.White, rot, new Vector2(xDiff, yDiff), SCALE, 0, 0); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawString(SpriteFont font, string text, Vector2 pos, Color color)
        { batch.DrawString(font, text, pos, color, ROT * Res.DEG2RAD, Vector2.Zero, SCALE * 0.5f, 0, 0); }
    }
}