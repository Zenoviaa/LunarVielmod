using Stellamod.Helpers;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class NoStaff : AbstractMagicWand
    {
        //No element
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            normalSlotCount = 0;
            timedSlotCount = 0;
        }
    }
}
