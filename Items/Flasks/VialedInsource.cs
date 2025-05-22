using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class VialedInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Insource",  Helpers.LangText.Common("Insource"))
            {
                OverrideColor = new Color(100, 278, 203)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourcePotionSickness = 1200;
            InsourceCannotUseDuration = 1200;
        }

        public override void TriggerEffect(Player player)
        {
            player.AddBuff(ModContent.BuffType<VialedUp>(), 600);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 10);
            recipe.AddIngredient(ItemID.Daybloom, 5);
            recipe.AddIngredient(ItemID.Moonglow, 5);
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }
    }
}