using Stellamod.Common.ArmorRework;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public class ArtifactGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isMagicArtifact;
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(item, player, ref reduce, ref mult);
            if (isMagicArtifact)
            {
                ArmorStatsPlayer statsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
                reduce -= statsPlayer.artifactManaReduction;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (isMagicArtifact)
            {
                TooltipLine line = new TooltipLine(Mod, "MagicArtifactType", LangText.Common("MagicArtifact"));
                tooltips.Add(line);
            }
        }
    }

    public class ExpandingArtifactTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            ArtifactGlobalItem artifactGlobalItem = item.GetGlobalItem<ArtifactGlobalItem>();
            if (artifactGlobalItem.isMagicArtifact)
            {
                TooltipLine line = new TooltipLine(Mod, "MagicArtifact", LangText.Common("MagicArtifactHelp"));
                lines.Add(line);
            }
        }
    }
}
