//#define VSCYNC

using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vect2 = Mentula.Engine.Core.Vect2;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mentula.Utilities;

namespace Mentula.Client
{
    public class FastGraphics : GraphicsDeviceManager, IGameComponent, IUpdateable, IDrawable
    {
        internal float SCALE = 2f;

        public bool Enabled { get { return true; } }
        public int UpdateOrder { get { return 0; } }
        public int DrawOrder { get { return 0; } }
        public bool Visible { get; set; }

        private MainGame game;
        public FontCollection fonts;

        private VertexBuffer tileBuffer;
        private VertexBuffer destrBuffer;
        private VertexBuffer actorBuffer;
        private Color[] textureBuffer;
        private float heroR;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public FastGraphics(MainGame game)
            : base(game)
        {
            this.game = game;

            textureBuffer = new Color[0];
            PreferredBackBufferHeight = 600;
            PreferredBackBufferWidth = 800;

#if !VSCYNC
            SynchronizeWithVerticalRetrace = false;
            game.IsFixedTimeStep = false;
#endif
        }

        public void Initialize()
        {
            tileBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), 1, BufferUsage.WriteOnly);
            destrBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), 1, BufferUsage.WriteOnly);
            fonts = new FontCollection(game.Content, 3);
            fonts.Load("Fonts/", "ConsoleFont", "MenuFont", "NameFont");

            TextureCollection textures = new TextureCollection(game.Content);
            textures.LoadFromConfig("R/Textures");
            int length = 0;

            for (int i = 0; i < textures.Count; i++)
            {
                int curKey = textures.Keys.ElementAt(i);
                if (curKey > length) length = curKey;
            }

            textureBuffer = new Color[length + 1];

            for (int i = 0; i < textures.Count; i++)
            {
                KeyValuePair<int, Texture2D> curTexture = textures.ElementAt(i);
                Dictionary<Color, int> colors = new Dictionary<Color, int>();

                Color[] textureData = new Color[curTexture.Value.Width * curTexture.Value.Height];
                curTexture.Value.GetData<Color>(textureData);

                for (int j = 0; j < textureData.Length; j++)
                {
                    Color curColor = textureData[j];
                    if (curColor == Color.Transparent) continue;
                    if (colors.ContainsKey(curColor)) colors[curColor] += 1;
                    else colors.Add(curColor, 1);
                }

                Color highColor = Color.White;
                int highLength = 0;

                for (int j = 0; j < colors.Count; j++)
                {
                    KeyValuePair<Color, int> curColor = colors.ElementAt(j);

                    if (curColor.Value > highLength)
                    {
                        highLength = curColor.Value;
                        highColor = curColor.Key;
                    }
                }

                textureBuffer[curTexture.Key] = highColor;
            }
        }


        public void Update(GameTime gameTime)
        {
            Vector2 pos = Chunk.GetTotalPos(game.hero.ChunkPos, game.hero.Pos);
            Matrix view = Matrix.CreateScale(SCALE + Res.TileSize);
            view *= Matrix.CreateTranslation(new Vector3(pos.X, pos.Y, 0));
            heroR = game.hero.Rotation;

            VertexPositionColor[] tileVertices = new VertexPositionColor[tileBuffer.VertexCount];
            VertexPositionColor[] destrVertices = new VertexPositionColor[destrBuffer.VertexCount];

            int tileIndex = 0, destrIndex = 0;
            for (int i = 0; i < game.chunks.Length; i++)
            {
                Chunk curChunk = game.chunks[i];

                for (int j = 0; j < curChunk.Tiles.Length; j++)
                {
                    Tile curTile = curChunk.Tiles[i];
                    Color curColor = textureBuffer[curTile.Tex];
                    Vector2 curr = Chunk.GetTotalPos(curChunk.ChunkPos, curTile.Pos);

                    Vector3 transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y, 10), view);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X + 1, curr.Y, 10), view);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X + 1, curr.Y + 1, 10), view);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y + 1, 10), view);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y, 10), view);
                    tileVertices[tileIndex++] = new VertexPositionColor(transformedPos, curColor);
                }

                for (int j = 0; j < curChunk.Destrucables.Length; j++)
                {
                    Tile curDesr = curChunk.Destrucables[i];
                    Color curColor = textureBuffer[curDesr.Tex];
                    Vector2 curr = Chunk.GetTotalPos(curChunk.ChunkPos, curDesr.Pos);

                    Vector3 transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y, 10), view);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X + 1, curr.Y, 10), view);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X + 1, curr.Y + 1, 10), view);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y + 1, 10), view);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);

                    transformedPos = Vector3.Transform(new Vector3(curr.X, curr.Y, 10), view);
                    destrVertices[destrIndex++] = new VertexPositionColor(transformedPos, curColor);
                }
            }

            tileBuffer.SetData(tileVertices);
            destrBuffer.SetData(destrVertices);
        }

        public void Draw(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            BasicEffect basicEffect = new BasicEffect(game.GraphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.Identity;
            basicEffect.Projection = Matrix.CreatePerspective(800, 600, 0.1f, 100f);
            basicEffect.VertexColorEnabled = true;

            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.SetVertexBuffer(tileBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, tileBuffer.VertexCount / 3);

                GraphicsDevice.SetVertexBuffer(destrBuffer);
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, destrBuffer.VertexCount / 3);
            }

            GraphicsDevice.SetVertexBuffer(null);
        }

        public void UpdateChunks(ref Chunk[] chunks, ref NPC[] npcs)
        {
            int tileLength = chunks.Length * Res.ChunkTileLength;
            int destrLength = 0;
            for (int i = 0; i < chunks.Length; i++) destrLength += chunks[i].Destrucables.Length;

            if (tileLength != tileBuffer.VertexCount * 6) tileBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), tileLength * 6, BufferUsage.WriteOnly);
            if (destrLength != destrBuffer.VertexCount * 6) destrBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), destrLength * 6, BufferUsage.WriteOnly);
            //if (npcs.Length != actorBuffer.VertexCount * 6) actorBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), npcs.Length * 6, BufferUsage.WriteOnly);
        }

        public static void ChangeWindowBorder(IntPtr handle, byte newType)
        {
            Control window = Control.FromHandle(handle);
            window.FindForm().FormBorderStyle = (FormBorderStyle)newType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Rot(float rot)
        { return rot + 1.5707963f; }
    }
}