using Mentula.Content;
using Mentula.Engine.Core;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Client
{
    public class Camera
    {
        public Vect2 Scale { get; private set; }
        public float Rotation { get; private set; }
        public Vect2 Offset { get; private set; }

        private Matrix3 _model;
        private Matrix3 _view;
        private Matrix3 _mv;

        public Camera()
        {
            Scale = Vect2.One;
            Rotation = 0;
            Offset = new Vect2();

            _model = Matrix3.Identity;
            _view = Matrix3.Identity;
            _mv = Matrix3.Identity;
        }

        public void SetScale(Vect2 scale)
        {
            if (scale != Scale)
            {
                Scale = scale;
                UpdateMM();
            }
        }

        public void SetRotation(float rotation)
        {
            if (rotation != Rotation)
            {
                Rotation = rotation;
                UpdateMM();
            }
        }

        public void SetOffset(Vect2 offset)
        {
            if (offset != Offset)
            {
                Offset = offset;
                UpdateMM();
            }
        }

        public void Update(Matrix3 lookAt)
        {
            _view = Matrix3.ApplyScale(Res.TileSize);
            _view *= lookAt;

            _mv = _model * _view;
        }

        public void Transform(ref Chunk[] sourceArray, ref Vector2[] destinationArray_Tiles, ref Vector2[] destinationArray_Destr)
        {
            int index = 0;
            for (int i = 0; i < sourceArray.Length; i++)
            {
                Chunk cur = sourceArray[i];

                for (int j = 0; j < cur.Tiles.Length; j++)
                {
                    Vector2 curr = Chunk.GetTotalPos(cur.ChunkPos, cur.Tiles[j].Pos);
                    float x = (curr.X * _mv.A) + (curr.Y * _mv.B) + _mv.C;
                    float y = (curr.X * _mv.D) + (curr.Y * _mv.E) + _mv.F;

                    destinationArray_Tiles[(i * Res.ChunkTileLength) + j] = new Vector2(x, y);
                }

                for (int j = 0; j < cur.Destrucables.Length; j++)
                {
                    Vector2 curr = Chunk.GetTotalPos(cur.ChunkPos, cur.Destrucables[j].Pos);
                    float x = (curr.X * _mv.A) + (curr.Y * _mv.B) + _mv.C;
                    float y = (curr.X * _mv.D) + (curr.Y * _mv.E) + _mv.F;

                    destinationArray_Destr[index++] = new Vector2(x, y);
                }
            }
        }

        public void Transform(ref Creature[] sourceArray, ref Vector2[] destinationArray)
        {
            int index = 0;

            for (int i = 0; i < sourceArray.Length; i++)
            {
                IEntity actor = sourceArray[i];
                destinationArray[index++] = Transform(ref actor);
            }
        }

        public Vector2 Transform(ref IEntity creature)
        {
            Vector2 cur = Chunk.GetTotalPos(creature.ChunkPos, creature.Pos);
            float x = (cur.X * _mv.A) + (cur.Y * _mv.B) + _mv.C;
            float y = (cur.X * _mv.D) + (cur.Y * _mv.E) + _mv.F;
            return new Vector2(x, y);
        }

        private void UpdateMM()
        {
            _model = Matrix3.ApplyScale(Scale);
            _model *= Matrix3.ApplyTranslation(Offset);
            _model *= Matrix3.ApplyRotation(Rotation * Res.DEG2RAD);
        }
    }
}