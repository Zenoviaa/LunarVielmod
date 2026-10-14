using Stellamod.Core.SwingSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class ScrollExpandingTooltip : AbstractExpandingTooltip
{
    public override ExpandableTooltipPriority Priority => ExpandableTooltipPriority.VeryLate;
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


        var staminaAttack = LangText.Item(ScrollAbilities.scrollsToContentTemplates[scroll.scroll].ModItem, "Tooltip");

        var staminaCost = ScrollAbilities.GetStaminaCost(scroll.scroll);
        var staminaEffect = $"{UIUtils.GetStaminaIconString(staminaCost)}{staminaAttack}";
        line = new TooltipLine(Mod, "ScrollStaminaSlash", staminaEffect);
        lines.Add(line);
    }
}
