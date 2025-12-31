using Terraria.ModLoader;

namespace Stellamod.Assets
{
    public class TextureItem : ModTexturedType
    {
        protected sealed override void Register()
        {
            ModTypeLookup<TextureItem>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
    }
}
