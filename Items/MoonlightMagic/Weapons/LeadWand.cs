using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class LeadWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
        }

        public override int GetNormalSlotCount()
        {
            return 4;
        }

        public override int GetTimedSlotCount()
        {
            return 4;
        }
    }
}
