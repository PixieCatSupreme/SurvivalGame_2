using Mentula.Content;
using Microsoft.Xna.Framework.Content;
using System;
using System.ComponentModel.Design;

namespace Mentula.Server
{
    public partial class Resources : ContentManager
    {
        private Metal[] metals;
        private Biomass[] biomasses;
        private Item[] items; // TODO: Remove (to retrogen)

        public Resources()
            : base(new ServiceContainer(), "Content")
        {

        }

        public override T Load<T>(string assetName)
        {
            try { return base.Load<T>(assetName); }
            catch (ContentLoadException) { return default(T); }
        }
    }
}