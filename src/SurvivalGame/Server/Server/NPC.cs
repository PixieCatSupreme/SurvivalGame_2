//#define ZILTOID
using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Resc = Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;
using Mentula.Content;
using static Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        private IntVector2[] path;
        private int moveI;

        public NPC(Creature c)
            : base(c)
        {
            moveI = 0;
#if ZILTOID
            path = new IntVector2[4];
            path[0] = new IntVector2(-10, -10);
            path[1] = new IntVector2(0, -10);
            path[2] = new IntVector2(-10, 0);
            path[3] = new IntVector2(0, 0);
#endif
        }

        public void WalkPath(float deltaTime)
        {
            if (path != null)
            {
                float speed = deltaTime*10;
                while (speed > 0 && moveI < path.Length)
                {
                    Vector2 pos = new Vector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
                    float dist = Vector2.Distance(pos, path[moveI]);
                    Vector2 rot = path[moveI] - pos;
                    Rotation = MathEX.VectorToRadians(rot);
                    if (dist < speed)
                    {
                        Pos += rot;
                        speed -= dist;
                        moveI++;
                    }
                    else
                    {
                        Pos += (rot / dist * speed);
                        speed = 0;
                    }
                }
                FormatPos();
            }
        }

        
    }
}
