using Stellamod.Common.MagicSystem;
using Stellamod.Items.Elements;

namespace Stellamod.Items.Staffs
{
    internal class CelestiaWand : Staff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //For testing purposes
            Element = new Radiant();
        }

        public override int GetNormalSlotCount()
        {
            return 4;
        }
        public override int GetTimedSlotCount()
        {
            return 0;
        }
    }
}
