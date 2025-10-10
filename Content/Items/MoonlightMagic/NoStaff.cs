using Stellamod.Helpers;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class NoStaff : BaseStaff
    {
        //No element
        public override string Texture => AssetHelper.EmptyTexture;
        public override int GetNormalSlotCount()
        {
            return 0;
        }
        public override int GetTimedSlotCount()
        {
            return 0;
        }
    }
}
