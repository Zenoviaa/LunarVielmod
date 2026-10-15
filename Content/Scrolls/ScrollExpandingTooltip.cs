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
    public override string GetShiftHelpTooltip(Item item)
    {
        if (item.ModItem == null)
            return null;
        if (item.TryGetGlobalItem<ScrollGlobalItem>(out var scroll) is false)
            return null;
        if (!ScrollAbilities.scrollsToContentTemplates.ContainsKey(scroll.scroll))
            return null;

        var expandingString = $"{UIUtils.GetStaminaIconString(scroll.StaminaCost)}{LangText.Common("SpecialAttack")}";
        return expandingString;
    }

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
        line.OverrideColor = Color.Lerp(Main.DiscoColor, Color.White, 0.4f);
        lines.Add(line);


        var staminaAttack = LangText.Item(ScrollAbilities.scrollsToContentTemplates[scroll.scroll].ModItem, "Tooltip");

        var staminaCost = ScrollAbilities.GetStaminaCost(scroll.scroll);
        var staminaEffect = $"{UIUtils.GetStaminaIconString(staminaCost)}{staminaAttack}";
        line = new TooltipLine(Mod, "ScrollStaminaSlash", staminaEffect);
        line.OverrideColor = new Color(239, 163, 80);
        lines.Add(line);
    }
}
