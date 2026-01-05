using Microsoft.Xna.Framework;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public class BellMinionExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {

            BellMinionGlobalItem bellMinion = item.GetGlobalItem<BellMinionGlobalItem>();
            if (bellMinion.isBellMinion)
            {
                TooltipLine line = new TooltipLine(Mod, "MinionHelp", LangText.Common("BellMinionHelp"));
                lines.Add(line);
            }

        }
    }
    public class BellMinionGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isBellMinion;
        public float addedCastingTime;
        public override bool CanUseItem(Item item, Player player)
        {
            if (isBellMinion)
                return false;
            return base.CanUseItem(item, player);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (isBellMinion)
            {
                float seconds = addedCastingTime / 60f;
                string secondsString = seconds.ToString("#.#");
                TooltipLine line = new TooltipLine(Mod, "AmountOfCastingTime",
                    LangText.Common("CastingTime", secondsString));
                line.OverrideColor = Color.Lerp(new Color(80, 187, 180), Color.Black, 0.25f);
                tooltips.Add(line);
            }
        }
    }
}
