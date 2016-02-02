﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mentula.Utilities;

namespace Mentula.Server
{
    public class Structure
    {
        public Rectangle Space;
        public List<Tile> Tiles;
        public List<Destructible> Destructibles;

        public Structure()
        {
            Space = new Rectangle();
            Tiles = new List<Tile>();
            Destructibles = new List<Destructible>();
        }

        public void RescaleSpace()
        {
            Space.X = 0;
            Space.Y = 0;
            for (int i = 0; i < Tiles.Count; i++)
            {
                IntVector2 p = Tiles[i].Pos;
                ExtendTo(p);
            }
            for (int i = 0; i < Destructibles.Count; i++)
            {
                IntVector2 p = new IntVector2(Destructibles[i].Pos);
                ExtendTo(p);
            }
        }

        public void ExtendTo(IntVector2 p)
        {
            if (p.X > Space.Width)
            {
                Space.Width = p.X;
            }
            if (p.Y > Space.Height)
            {
                Space.Height = p.Y;
            }
            if (p.X < Space.X)
            {
                Space.X = p.X;
            }
            if (p.Y < Space.Y)
            {
                Space.Y = p.Y;
            }
        }
    }
}
