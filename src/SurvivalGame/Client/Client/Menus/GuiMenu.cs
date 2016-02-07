using Mentula.GuiItems.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Client.Menus
{
    public sealed class GuiMenu : Menu
    {
        private MainGame MGame { get { return (MainGame)Game; } }

        public GuiMenu(MainGame game)
            : base(game)
        { }
    }
}
