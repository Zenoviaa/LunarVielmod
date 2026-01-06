using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicCauldron
{
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

    public class BrewingMaterialLabelTooltip : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            if (cauldron.IsMaterial(item.type))
            {
                TooltipLine materialLine = new TooltipLine(Mod, "BrewingMaterialLabel", LangText.Common("CauldronMaterialLabel"));
                tooltips.Add(materialLine);
            }
        }
    }

    public class BrewingMaterialExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            if (cauldron.IsMaterial(item.type))
            {
                TooltipLine materialLine = new TooltipLine(Mod, "BrewingMaterialHelpingText", LangText.Common("CauldronMaterialHelp"));
                lines.Add(materialLine);
            }
        }
    }
}
