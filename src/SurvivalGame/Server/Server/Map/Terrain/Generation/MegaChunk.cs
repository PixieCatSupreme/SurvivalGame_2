using Mentula.Utilities;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Mentula.Utilities.Resources.Res;

namespace Mentula.Server
{
    public class MegaChunk
    {
        public List<City> Cities;
        public List<Structure> Structures;
        public IntVector2 Pos;
        private const int citySize = 1024;
        private const int maxCities = 6;
        private const int minStreetSize =32;
        private const int minBuildingSize = 16;
        private const int minRoomSize = 2;
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
                    int size = (int)(citySize * r.NextDouble());
                    rect.Height = size;
                    rect.Width = size;
                    rect.X = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble()) + Pos.X * MegaChunkSize;
                    rect.Y = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble()) + Pos.Y * MegaChunkSize;
                    bool canplace = true;

                    for (int j = 0; j < Cities.Count; j++)
                    {
                        if (Rectangle.Intersect(rect, Cities[j].Space) == Rectangle.Empty)
                        {
                            canplace = false;
                        }
                    }

                    if (canplace)
                    {
                        Cities.Add(new City() { Space = rect });
                        Cities[i].Streets = new List<Street>();
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
                                GenerateHouse(Cities[i].Streets[j].Buildings[k], r);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void GenerateHouse(Building b, Random r)
        {
            //List<Rectangle> rooms = BinarySplitGenerator.GenerateBinarySplitMap1(new IntVector2(b.Space.Width, b.Space.Height), new IntVector2(minRoomSize), r.NextDouble().ToString());
            Structure s = new Structure();
            s.Space = b.Space;
            for (int i = 0; i < b.Space.Width; i++)
            {
                for (int j = 0; j < b.Space.Height; j++)
                {
                    s.Tiles.Add(new Tile(5, new IntVector2(i, j)));
                }
            }
            Structures.Add(s);
        }
    }
}
