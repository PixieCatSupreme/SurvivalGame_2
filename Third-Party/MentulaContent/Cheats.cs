using Mentula.Content.MM;
using Microsoft.Xna.Framework.Content;

namespace Mentula.Content
{
    public static class Cheats
    {
        public static readonly Metal Unobtanium = new Metal();

        public static void Test(ContentManager content)
        {
            MItemProcessor.Manifest[] test = content.Load<MItemProcessor.Manifest[]>("BodyParts");
        }
    }
}