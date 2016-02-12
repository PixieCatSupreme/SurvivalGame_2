using Mentula.Utilities;
using Microsoft.Xna.Framework;
using Mentula.Utilities.MathExtensions;
using Mentula.Engine.Core;
using System;
using System.Collections.Generic;
using Mentula.Content;
using static Mentula.Utilities.Resources.Res;
using System.Threading;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        public Queue<KeyValuePair<NPCTasks, object[]>> Tasks;

        private const int ClockCap = 32;
        private int NPCClock;
        private IntVector2[] path;
        private int moveI;
        private Thread thread;
        private static List<Chunk> chunks = new List<Chunk>();
        private Creature target;
        private bool runThread;

        public static void SetChunkRef(ref List<Chunk> cl)
        {
            chunks = cl;
        }

        public NPC(Creature c)
            : base(c)
        {
            Tasks = new Queue<KeyValuePair<NPCTasks, object[]>>();
            NPCClock = RNG.Next(ClockCap);
            moveI = 0;
            InitThread();
            Load();
        }

        public void Load()
        {
            if (thread.ThreadState == ThreadState.Running) throw new InvalidOperationException();
            if (thread.ThreadState != ThreadState.Unstarted) InitThread();
            runThread = true;
            thread.Start();
        }

        public void UnLoad()
        {
            runThread = false;
        }

        public void Update(float Delta, Creature t)
        {
            if (NPCClock >= ClockCap)
            {
                NPCClock = 0;
                target = t;
                Tasks.Enqueue(new KeyValuePair<NPCTasks, object[]>(NPCTasks.CalcPath, new object[0]));
            }
            else
            {
                NPCClock++;
            }

            Tasks.Enqueue(new KeyValuePair<NPCTasks, object[]>(NPCTasks.Walk, new object[1] { Delta }));
        }

        private void InitThread()
        {
            thread = new Thread(RunThread);
        }

        private void RunThread()
        {
            while (runThread)
            {
                if (Tasks.Count > 0)
                {
                    TickThread();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void TickThread()
        {
            KeyValuePair<NPCTasks, object[]> a = Tasks.Dequeue();
            switch (a.Key)
            {
                case NPCTasks.Walk:
                    WalkPath(a.Value);
                    break;
                case NPCTasks.CalcPath:
                    GeneratePath();
                    break;
            }
        }

        private void WalkPath(object[] deltaTime)
        {
            if (path != null)
            {
                float speed = Convert.ToSingle(deltaTime[0]) * 10;
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

        private void GeneratePath()
        {
            IntVector2 pos = new IntVector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
            IntVector2 targetPos = new IntVector2(target.Pos.X + target.ChunkPos.X * ChunkSize, target.Pos.Y + target.ChunkPos.Y * ChunkSize);
            bool inRange = true;
            bool diff = true;
            if (Vector2.Distance(pos, targetPos) > 32)
            {
                inRange = false;
            }
            if (path != null)
            {
                if (path[path.Length - 1] == targetPos)
                {
                    diff = false;
                }
            }
            if (inRange && diff)
            {
                int minX = Math.Min(ChunkPos.X, target.ChunkPos.X) - 1;
                int maxX = Math.Max(ChunkPos.X, target.ChunkPos.X) + 1;
                int minY = Math.Min(ChunkPos.Y, target.ChunkPos.Y) - 1;
                int maxY = Math.Max(ChunkPos.Y, target.ChunkPos.Y) + 1;
                int minTX = Math.Min(pos.X, targetPos.X) - 5;
                int maxTX = Math.Max(pos.X, targetPos.X) + 5;
                int minTY = Math.Min(pos.Y, targetPos.Y) - 5;
                int maxTY = Math.Max(pos.Y, targetPos.Y) + 5;

                List<Chunk> c = new List<Chunk>();
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (chunks[i].ChunkPos.X >= minX && chunks[i].ChunkPos.X <= maxX && chunks[i].ChunkPos.Y >= minY && chunks[i].ChunkPos.Y <= maxY)
                    {
                        c.Add(chunks[i]);
                    }
                }
                AStar.Map pathing = new AStar.Map(pos, targetPos);
                for (int i = 0; i < c.Count; i++)
                {
                    int dx = c[i].ChunkPos.X * ChunkSize;
                    int dy = c[i].ChunkPos.Y * ChunkSize;
                    for (int j = 0; j < c[i].Destructibles.Count; j++)
                    {
                        int dex = (int)c[i].Destructibles[j].Pos.X + dx;
                        int dey = (int)c[i].Destructibles[j].Pos.Y + dy;
                        pathing.AddNode(new IntVector2(dex, dey), 800000, false);
                    }
                }
                int xdiff = Math.Abs(minTX - maxTX);
                int ydiff = Math.Abs(minTY - maxTY);
                for (int i = 0; i < xdiff; i++)
                {
                    for (int j = 0; j < ydiff; j++)
                    {
                        pathing.AddNode(new IntVector2(minTX + i, minTY + j));
                    }
                }
                IntVector2[] p = AStar.Route8(ref pathing);
                if (p.Length > 0)
                {
                    moveI = 0;
                    path = p;
                }
            }
        }

    }

    public enum NPCTasks
    {
        Walk,
        CalcPath
    }
}
