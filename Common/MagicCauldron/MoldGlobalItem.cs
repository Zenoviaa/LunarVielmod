using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicCauldron
{
    public class MoldGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isMold;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
        }
    }
}
