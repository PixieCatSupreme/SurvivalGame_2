using Mentula.Utilities;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Mentula.Utilities.Resources.Res;
using Mentula.Utilities.MathExtensions;

namespace Mentula.Server
{
    public class MegaChunk
    {
        public List<City> Cities;
        public List<Structure> Structures;
        public IntVector2 Pos;
        private const int citySize = 1024;
        private const int maxCities = 6;
        private const int minStreetSize = 32;
        private const int minBuildingSize = 16;
        private const int minRoomSize = 3;
        public MegaChunk(IntVector2 pos)
        {
            Cities = new List<City>();
            Structures = new List<Structure>();
            Pos = pos;
            GenerateCities();
        }
        private void GenerateCities()
        {
            Random r = new Random(RNG.RIntFromString(Pos.X + Seed + Pos.Y));
            int citynums = (int)(maxCities * r.NextDouble()) + 1;
            for (int i = 0; i < citynums; i++)
            {
                Rectangle rect = new Rectangle();
                while (true)
                {
                    int size = (int)(citySize * r.NextDouble()) + citySize / 4;
                    rect.Height = size;
                    rect.Width = size;
                    rect.X = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble()) + Pos.X * MegaChunkSize;
                    rect.Y = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble()) + Pos.Y * MegaChunkSize;
                    bool canplace = true;

                    for (int j = 0; j < Cities.Count; j++)
                    {
                        if (!Rectangle.Intersect(rect, Cities[j].Space).IsEmpty)
                        {
                            canplace = false;
                        }
                    }

                    if (canplace)
                    {
                        Cities.Add(new City() { Space = rect });
                        Cities[i].Streets = new List<Street>();
                        List<Rectangle> roads = new List<Rectangle>();
                        List<Rectangle> streets = BinarySplitGenerator.GenerateBinarySplitMap2(Cities[i].Space, new IntVector2(minStreetSize), r.NextDouble().ToString());
                        Vector2 citymiddle = new Vector2(size / 2 + rect.X, size / 2 + rect.Y);
                        for (int j = 0; j < streets.Count; j++)
                        {
                            Vector2 streetmiddle = new Vector2(streets[i].X + streets[i].Width / 2, streets[i].Y + streets[i].Height / 2);
                            if (Vector2.Distance(streetmiddle, citymiddle) > size / 2)
                            {
                                streets.RemoveAt(j);
                            }

                        }

                        for (int j = 0; j < streets.Count; j++)
                        {

                            Cities[i].Streets.Add(new Street() { Space = streets[j] });
                            Cities[i].Streets[j].Space.X += 4;
                            Cities[i].Streets[j].Space.Y += 4;
                            Cities[i].Streets[j].Space.Width -= 9;
                            Cities[i].Streets[j].Space.Height -= 9;
                            roads.Add(new Rectangle(Cities[i].Streets[j].Space.X - 9, Cities[i].Streets[j].Space.Y - 9, 9, Cities[i].Streets[j].Space.Height + 18));
                            roads.Add(new Rectangle(Cities[i].Streets[j].Space.X - 9, Cities[i].Streets[j].Space.Y - 9, Cities[i].Streets[j].Space.Width + 18, 9));
                            roads.Add(new Rectangle(Cities[i].Streets[j].Space.X + Cities[i].Streets[j].Space.Width, Cities[i].Streets[j].Space.Y - 9, 9, Cities[i].Streets[j].Space.Height + 18));
                            roads.Add(new Rectangle(Cities[i].Streets[j].Space.X - 9, Cities[i].Streets[j].Space.Y + Cities[i].Streets[j].Space.Height, Cities[i].Streets[j].Space.Width + 18, 9));
                            Cities[i].Streets[j].Buildings = new List<Building>();
                            List<Rectangle> buildings = BinarySplitGenerator.GenerateBinarySplitMap1(Cities[i].Streets[j].Space, new IntVector2(minBuildingSize), r.NextDouble().ToString());
                            for (int k = 0; k < buildings.Count; k++)
                            {
                                Rectangle bs = buildings[k];
                                bs.X += 1;
                                bs.Y += 1;
                                bs.Width -= 2;
                                bs.Height -= 2;
                                Cities[i].Streets[j].Buildings.Add(new Building() { Space = bs });
                                GenerateHouse(Cities[i].Streets[j], Cities[i].Streets[j].Buildings[k], r);
                            }
                        }
                        for (int j = 0; j < roads.Count; j++)
                        {
                            GenerateRoads(roads[j]);
                        }
                        break;
                    }
                }
            }
        }

        private void GenerateHouse(Street str, Building b, Random r)
        {
            List<Rectangle> rooms = BinarySplitGenerator.GenerateBinarySplitMap1(new Rectangle(0, 0, b.Space.Width - 1, b.Space.Height - 1), new IntVector2(minRoomSize), r.NextDouble().ToString());
            rooms.Shuffle();
            Structure s = new Structure();
            s.Space = b.Space;
            for (int i = 0; i < b.Space.Width; i++)
            {
                for (int j = 0; j < b.Space.Height; j++)
                {
                    s.Tiles.Add(new Tile(5, new IntVector2(i, j)));
                }
            }
            for (int i = 0; i < s.Space.Width; i++)
            {
                s.Destructibles.Add(new Destructible(IntVector2.Zero, new Vector2(i, s.Space.Height - 1), 8));
            }

            for (int i = 0; i < s.Space.Height; i++)
            {
                s.Destructibles.Add(new Destructible(IntVector2.Zero, new Vector2(s.Space.Width - 1, i), 8));
            }
            for (int i = 0; i < rooms.Count; i++)
            {
                for (int j = 0; j < rooms[i].Width; j++)
                {
                    s.Destructibles.Add(new Destructible(IntVector2.Zero, new Vector2(rooms[i].X + j, rooms[i].Y), 8));
                }
                for (int j = 0; j < rooms[i].Height; j++)
                {
                    s.Destructibles.Add(new Destructible(IntVector2.Zero, new Vector2(rooms[i].X, rooms[i].Y + j), 8));
                }
            }
            IntVector2 startpos = IntVector2.Zero;
            int frontDoorIndex = 0;
            #region direction
            Vector2 dir = new Vector2(b.Space.Center.X, b.Space.Center.Y) - new Vector2(str.Space.Center.X, str.Space.Center.Y);
            int dist = int.MaxValue;
            if (str.Space.Width > str.Space.Height)
            {
                dir.X *= 1000;
            }
            else
            {
                dir.Y *= 1000;
            }
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                startpos.X = b.Space.Width / 2;
                int x = startpos.X;
                if (dir.Y < 0)
                {
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        if (rooms[i].Y == 0)
                        {
                            int dist2 = Math.Abs(rooms[i].Center.X - startpos.X);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                frontDoorIndex = i;
                                x = rooms[i].Center.X;
                            }
                        }
                    }
                }
                else
                {
                    startpos.Y = b.Space.Height - 1;
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        if (rooms[i].Y + rooms[i].Height == b.Space.Height - 1)
                        {
                            int dist2 = Math.Abs(rooms[i].Center.X - startpos.X);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                frontDoorIndex = i;
                                x = rooms[i].Center.X;
                            }
                        }
                    }
                }
                startpos.X = x;
            }
            else
            {
                startpos.Y = b.Space.Height / 2;
                int y = startpos.Y;
                if (dir.X < 0)
                {
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        if (rooms[i].X == 0)
                        {
                            int dist2 = Math.Abs(rooms[i].Center.Y - startpos.Y);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                frontDoorIndex = i;
                                y = rooms[i].Center.Y;
                            }
                        }
                    }
                }
                else
                {
                    startpos.X = b.Space.Width - 1;
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        if (rooms[i].X + rooms[i].Width == b.Space.Width - 1)
                        {
                            int dist2 = Math.Abs(rooms[i].Center.Y - startpos.Y);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                frontDoorIndex = i;
                                y = rooms[i].Center.Y;
                            }
                        }
                    }
                }
                startpos.Y = y;
            }
            for (int i = 0; i < s.Destructibles.Count; i++)
            {
                if (s.Destructibles[i].Pos == startpos)
                {
                    s.Destructibles.RemoveAt(i);

                }
            }
            #endregion

            bool[] connected = new bool[rooms.Count];
            connected[frontDoorIndex] = true;
            bool allConected = false;
            while (!allConected)
            {
                for (int i = 0; i < connected.Length; i++)
                {
                    for (int j = 0; j < connected.Length; j++)
                    {
                        if (!connected[i] && connected[j])
                        {
                            Rectangle room1 = rooms[i];
                            room1.Width += 1;
                            room1.Height += 1;
                            Rectangle room2 = rooms[j];
                            room2.Width += 1;
                            room2.Height += 1;
                            Rectangle overlap = Rectangle.Intersect(room1, room2);
                            if (overlap.Width > 2 || overlap.Height > 2)
                            {
                                if (overlap.Width > 2)
                                {
                                    overlap.Width -= 2;
                                    overlap.X += 1;
                                }
                                if (overlap.Height > 2)
                                {
                                    overlap.Height -= 2;
                                    overlap.Y += 1;
                                }
                                IntVector2 doorPos = new IntVector2(overlap.Center.X, overlap.Center.Y);
                                for (int k = 0; k < s.Destructibles.Count; k++)
                                {
                                    if (s.Destructibles[k].Pos == doorPos)
                                    {
                                        s.Destructibles.RemoveAt(k);
                                    }
                                }
                                connected[i] = true;
                            }
                        }
                    }
                }
                allConected = true;
                for (int i = 0; i < connected.Length; i++)
                {
                    if (!connected[i])
                    {
                        allConected = false;
                    }
                }
            }
            Structures.Add(s);
        }

        private void GenerateRoads(Rectangle road)
        {
            Structure s = new Structure();
            s.Space = road;
            if (road.Width == 9)
            {
                for (int i = 0; i < road.Height; i++)
                {
                    s.Tiles.Add(new Tile(9, new IntVector2(0, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(1, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(2, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(3, i)));
                    s.Tiles.Add(new Tile(11, new IntVector2(4, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(5, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(6, i)));
                    s.Tiles.Add(new Tile(10, new IntVector2(7, i)));
                    s.Tiles.Add(new Tile(9, new IntVector2(8, i)));
                }
            }
            else if (road.Height == 9)
            {
                for (int i = 0; i < road.Width; i++)
                {
                    s.Tiles.Add(new Tile(9, new IntVector2(i, 0)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 1)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 2)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 3)));
                    s.Tiles.Add(new Tile(12, new IntVector2(i, 4)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 5)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 6)));
                    s.Tiles.Add(new Tile(10, new IntVector2(i, 7)));
                    s.Tiles.Add(new Tile(9, new IntVector2(i, 8)));
                }
            }
            Structures.Add(s);
        }
    }
}
