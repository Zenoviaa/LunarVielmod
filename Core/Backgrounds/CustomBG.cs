using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    public abstract class CustomBG : ModType
    {
        public int Type;
        public CustomBGLayer[] Layers = new CustomBGLayer[0];
        public int Priority;
        public float Alpha;
        public virtual bool IsActive()
        {
            return false;
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        protected sealed override void Register()
        {
            ModTypeLookup<CustomBG>.Register(this);
        }

    }
}
