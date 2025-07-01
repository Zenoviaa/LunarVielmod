using Stellamod.Common.MagicSystem;
using Stellamod.Items.Elements;

namespace Stellamod.Items.Staffs
{
    internal class VoidRiddenStaff : Staff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //For testing purposes
            Element = new Radiant();
        }
    }
}
