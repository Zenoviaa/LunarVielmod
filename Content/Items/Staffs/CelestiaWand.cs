using Stellamod.Core.MagicSystem;
using Stellamod.Content.Items.Elements;

namespace Stellamod.Content.Items.Staffs
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
