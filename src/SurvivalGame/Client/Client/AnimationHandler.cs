using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Client
{
    public class AnimationHandler
    {
        private Animation[] _animationArray;

        public int GetFrameTexture(int animationName, int animationFrame)
        {
            return _animationArray[animationName].GetSpriteNumber(animationFrame);
        }
    }

    public class Animation
    {
        private int[] _textureNumbers;

        public string animationName;
        public int frames;

        public Animation(string animationName)
        {
            this.animationName = animationName;
            this.frames = _textureNumbers.Length;
        }

        public int GetSpriteNumber(int animationFrame)
        {
            return _textureNumbers[animationFrame];
        }
    }
}
