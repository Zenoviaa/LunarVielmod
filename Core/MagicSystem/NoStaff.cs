using Stellamod.Core.Helpers;

namespace Stellamod.Core.MagicSystem
{
    internal class NoStaff : Staff
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
