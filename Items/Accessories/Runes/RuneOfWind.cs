using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Stellamod.Items.Ores;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfWind : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Corsage");
            // Tooltip.SetDefault("Has a chance to gift super healing, if you were hit while under the effects of super healing everything on screen will be poisoned.");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = 1;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<GintzlMetal>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<RunicTableT>());
            recipe.Register();


        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Item.defense = 2;
            player.GetModPlayer<MyPlayer>().WindRune = true;

        }
    }
}