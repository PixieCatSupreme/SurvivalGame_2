using Mentula.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mentula.Utilities.Resources.Res;
using Microsoft.Xna.Framework;

namespace Mentula.Server
{
    public class MegaChunk
    {
        public List<City> Cities;
        public List<Structure> Structures;
        public IntVector2 Pos;
        private const int citySize = 1024;
        private const int maxCities = 6;
        private const int minStreetSize = 64;
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
            int citynums = (int)(maxCities * r.NextDouble());
            for (int i = 0; i < citynums; i++)
            {               
                Rectangle rect = new Rectangle();
                while (true)
                {
                    int size = (int)(citySize * r.NextDouble());
                    rect.Height = size;
                    rect.Width = size;
                    rect.X = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble());
                    rect.Y = (int)((ChunkSize * MegaChunkSize - size) * r.NextDouble());
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
                        List<Rectangle> streets = BinarySplitGenerator.GenerateBinarySplitMap2(new IntVector2(rect.Width, rect.Height), new IntVector2(minStreetSize), r.NextDouble().ToString());
                        Vector2 citymiddle = new Vector2(size / 2, size / 2);
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
                            Cities[i].Streets[j].Buildings = new List<Building>();
                            List<Rectangle> buildings = BinarySplitGenerator.GenerateBinarySplitMap1(new IntVector2(streets[j].Width, streets[j].Height), new IntVector2(minBuildingSize), r.NextDouble().ToString());
                            for (int k = 0; k < buildings.Count; k++)
                            {
                                Cities[i].Streets[j].Buildings.Add(new Building() { Space = buildings[k] });
                                List<Rectangle> rooms = BinarySplitGenerator.GenerateBinarySplitMap1(new IntVector2(buildings[k].Width, buildings[k].Height), new IntVector2(minRoomSize), r.NextDouble().ToString());
                                Cities[i].Streets[j].Buildings[k].Rooms = new List<Room>();
                                for (int l = 0; l < rooms.Count; l++)
                                {
                                    Cities[i].Streets[j].Buildings[k].Rooms.Add(new Room() { Space = rooms[l] });
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

    }
}
