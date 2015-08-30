﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mentula.Client
{
    [DebuggerDisplay("Avarage={Avarage}")]
    public class FPS
    {
        protected const int FRAME_BUFFER = 100;
        public float Current;
        public float Avarage;

        private Queue<float> buffer;

        public FPS()
        {
            Current = 0;
            Avarage = 0;
            buffer = new Queue<float>();
        }

        public void Update(float delta)
        {
            if (delta < 0) return;
            Current = 1 / delta;
            buffer.Enqueue(Current);

            if (buffer.Count > FRAME_BUFFER) buffer.Dequeue();

            Avarage = buffer.Average();
        }

        public override string ToString()
        {
            return ((int)Current).ToString();
        }
    }
}
