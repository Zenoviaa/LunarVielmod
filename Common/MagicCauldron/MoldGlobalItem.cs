using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
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

    public class MoldExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            if (item.GetGlobalItem<MoldGlobalItem>().isMold)
            {
                TooltipLine moldLine = new TooltipLine(Mod, "MoldHelpingText", LangText.Common("CauldronMoldHelp"));
                lines.Add(moldLine);
            }
        }
    }

}
