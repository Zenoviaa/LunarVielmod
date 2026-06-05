using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class ScrollExpandingTooltip : AbstractExpandingTooltip
{
    public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
    {
        if (item.TryGetGlobalItem<ScrollGlobalItem>(out var scroll) is false)
            return;
        if (scroll.scroll == ScrollAbility._None)
            return;
        if (!ScrollAbilities.scrollsToContentTemplates.ContainsKey(scroll.scroll))
            return;
        //Bruh

        TooltipLine line;
        line = new TooltipLine(Mod, "ScrollEnchantment", LangText.Common("ScrollEnchantment"));
        line.OverrideColor = Color.GreenYellow;
        lines.Add(line);

        line = new TooltipLine(Mod, "ScrollStaminaSlash", LangText.Item(ScrollAbilities.scrollsToContentTemplates[scroll.scroll].ModItem, "Tooltip"));
        lines.Add(line);

        line = new TooltipLine(Mod, "ScrollStaminaCost", LangText.Common("StaminaCost",
            ScrollAbilities.GetStaminaCost(scroll.scroll)));
        line.OverrideColor = Color.Goldenrod;
        lines.Add(line);
    }
}
