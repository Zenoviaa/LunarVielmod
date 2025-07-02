using Stellamod.Core.MagicSystem;
using Stellamod.Content.Items.Elements;

namespace Stellamod.Content.Items.Staffs
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
