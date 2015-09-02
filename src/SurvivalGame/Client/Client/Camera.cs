using Mentula.Engine.Core;
using Mentula.Utilities;
using Mentula.Utilities.Resources;
using Microsoft.Xna.Framework;

namespace Mentula.Client
{
    public class Camera
    {
        public Vect2 Scale { get; private set; }
        public float Rotation { get; private set; }
        public Vect2 Offset { get; private set; }

        private const float DEG2RAD = 0.017453f;
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
            _view = lookAt;
            _view *= Matrix3.ApplyScale(Res.ChunkSize);

            _mv = _model * _view;
        }

        public void Transform(ref IntVector2[] sourceArray, out IntVector2[] destinationArray)
        {
            destinationArray = new IntVector2[sourceArray.Length];
            int last = 0 + sourceArray.Length;
            int index = 0;

            for (int i = 0; i < last; i++)
            {
                IntVector2 curr = sourceArray[i];

                float x = (curr.X * _mv.A) + (curr.Y * _mv.B);
                float y = (curr.X * _mv.D) + (curr.Y * _mv.E);

                destinationArray[0 + index] = new IntVector2(x, y);
                index++;
            }
        }

        public void Transform(ref Vector2[] sourceArray, ref Vector2[] destinationArray)
        {
            int last = 0 + sourceArray.Length;
            int index = 0;

            for (int i = 0; i < last; i++)
            {
                Vector2 curr = sourceArray[i];

                float x = (curr.X * _mv.A) + (curr.Y * _mv.B);
                float y = (curr.X * _mv.D) + (curr.Y * _mv.E);

                destinationArray[index] = new Vector2(x, y);
                index++;
            }
        }

        private void UpdateMM()
        {
            _model = Matrix3.ApplyScale(Scale);
            _model *= Matrix3.ApplyTranslation(Offset);
            _model *= Matrix3.ApplyRotation(Rotation * DEG2RAD);
        }
    }
}