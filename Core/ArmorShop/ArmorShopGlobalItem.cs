using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ArmorShop
{
    internal class ArmorShopGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            ArmorShopPlayer armorShopPlayer = Main.LocalPlayer.GetModPlayer<ArmorShopPlayer>();
            ArmorShopSet set = groups.FindSet(item);
            if (set == null)
                return;
            if (set.HasPurchased())
                return;

            var line = new TooltipLine(Mod, "ArmorType", LangText.ArmorShopClass(set.heads[0].ModItem));
            line.OverrideColor = Color.Yellow;
            tooltips.Add(line);
        }
    }
}
