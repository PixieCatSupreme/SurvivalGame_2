﻿using Mentula.Content.MM;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Client
{
    public class TextureCollection : Dictionary<int, Texture2D>
    {
        private ContentManager content;

        public TextureCollection(ContentManager content)
        {
            this.content = content;
        }

        public TextureCollection(ContentManager content, int size)
            : base(size)
        {
            this.content = content;
        }

        public void LoadFromConfig(string name)
        {
            R config = content.Load<R>(name);

            for (int i = 0; i < config.Values.Count; i++)
            {
                KeyValuePair<int, string> cur = config.ElementAt(i);
                Add(cur.Key, cur.Value);
            }
        }

        public void Add(int id, string name)
        {
            Add(id, content.Load<Texture2D>(name));
        }
    }

    public class FontCollection : Dictionary<string, SpriteFont>
    {
        private ContentManager content;

        public FontCollection(ContentManager content)
        {
            this.content = content;
        }

        public FontCollection(ContentManager content, int size)
            : base(size)
        {
            this.content = content;
        }

        public void Load(string mapName = "", params string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                Add(files[i], content.Load<SpriteFont>(mapName + files[i]));
            }
        }
    }
}