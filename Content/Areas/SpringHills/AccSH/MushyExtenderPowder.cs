using Microsoft.Xna.Framework;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.IgnitersNPowders;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class MushyExtenderPowder : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "ADBPaadu", Helpers.LangText.Common("NoStack"))
            {
                OverrideColor = new Color(110, 187, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            IgniterPlayer igniterPlayer = player.GetModPlayer<IgniterPlayer>();
            igniterPlayer.extenderBonus += 0.15f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
                material: ModContent.ItemType<Mushroom>());
        }
    }
}