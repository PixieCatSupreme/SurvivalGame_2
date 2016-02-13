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


        private const int MaxDist = 24;
        private const int ExtraTileRange = 5;
        private const int ClockCap = 16;

        private int NPCClock;
        private IntVector2[] path;
        private int moveI;
        private Thread thread;
        private static Map map;
        private bool runThread;
        private IntVector2 spawnLoc;
        private IntVector2 spawnLocT;
        private IntVector2 spawnLocC;
        private bool Evade;

        public static void SetChunkRef(ref Map m)
        {
            map = m;
        }

        public NPC(Creature c)
            : base(c)
        {
            Tasks = new Queue<KeyValuePair<NPCTasks, object[]>>();
            NPCClock = RNG.Next(ClockCap);
            moveI = 0;
            spawnLoc = new IntVector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
            spawnLocT = new IntVector2(Pos.X, Pos.Y);
            spawnLocC = new IntVector2(ChunkPos.X, ChunkPos.Y);

            Evade = false;
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
            if (!Evade)
            {
                Vector2 pos = new Vector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
                if (Vector2.Distance(pos, spawnLoc) > MaxDist + ExtraTileRange)
                {
                    Evade = true;
                    Tasks.Enqueue(new KeyValuePair<NPCTasks, object[]>(NPCTasks.CalcPath, new object[4] { spawnLocT.X, spawnLocC.X, spawnLocT.Y, spawnLocC.Y }));
                }
            }

            if (NPCClock >= ClockCap && !Evade)
            {
                NPCClock = 0;
                Tasks.Enqueue(new KeyValuePair<NPCTasks, object[]>(NPCTasks.CalcPath, new object[4] { t.Pos.X, t.ChunkPos.X, t.Pos.Y, t.ChunkPos.Y }));
            }
            else
            {
                NPCClock++;
            }
            WalkPath(Delta);
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
                case NPCTasks.CalcPath:
                    GeneratePath(a.Value);
                    break;
            }
        }

        private void WalkPath(float deltaTime)
        {
            if (path != null)
            {
                float speed = deltaTime * 8;
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
                if (moveI >= path.Length)
                {
                    Evade = false;
                }
            }
        }

        private void GeneratePath(object[] target)
        {
            IntVector2 pos = new IntVector2(Pos.X + ChunkPos.X * ChunkSize, Pos.Y + ChunkPos.Y * ChunkSize);
            IntVector2 targetPosc = new IntVector2(Convert.ToInt32(target[1]), Convert.ToInt32(target[3]));
            IntVector2 targetPos = new IntVector2(Convert.ToInt32(target[0]) + targetPosc.X * ChunkSize, Convert.ToInt32(target[2]) + targetPosc.Y * ChunkSize);
            bool inRange = true;
            bool diff = true;
            if (Vector2.Distance(pos, targetPos) > MaxDist)
            {
                if (!Evade)
                {
                    inRange = false;
                }
            }
            if (path != null)
            {
                if (path.Length > 0)
                {
                    if (path[path.Length - 1] == targetPos)
                    {
                        diff = false;
                    }
                }
            }
            if (inRange && diff)
            {
                int minX = Math.Min(ChunkPos.X, targetPosc.X) - 1;
                int maxX = Math.Max(ChunkPos.X, targetPosc.X) + 1;
                int minY = Math.Min(ChunkPos.Y, targetPosc.Y) - 1;
                int maxY = Math.Max(ChunkPos.Y, targetPosc.Y) + 1;
                int minTX = Math.Min(pos.X, targetPos.X) - ExtraTileRange;
                int maxTX = Math.Max(pos.X, targetPos.X) + ExtraTileRange;
                int minTY = Math.Min(pos.Y, targetPos.Y) - ExtraTileRange;
                int maxTY = Math.Max(pos.Y, targetPos.Y) + ExtraTileRange;

                if (Evade)
                {
                    minTX -= ExtraTileRange;
                    maxTX += ExtraTileRange;
                    minTY -= ExtraTileRange;
                    maxTY += ExtraTileRange;
                }

                List<Chunk> c = new List<Chunk>();
                for (int i = 0; i < map.LoadedChunks.Count; i++)
                {
                    if (
                        map.LoadedChunks[i].ChunkPos.X >= minX &&
                        map.LoadedChunks[i].ChunkPos.X <= maxX &&
                        map.LoadedChunks[i].ChunkPos.Y >= minY &&
                        map.LoadedChunks[i].ChunkPos.Y <= maxY
                        )
                    {
                        c.Add(map.LoadedChunks[i]);
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
                moveI = 0;
                path = p;
            }
        }

    }

    public enum NPCTasks
    {
        CalcPath
    }
}
