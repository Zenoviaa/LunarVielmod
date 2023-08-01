using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Materials.HardMode
{
    class PixieSilk : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.value = 100;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }
  
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(2);
            recipe.AddIngredient(ItemID.UnicornHorn, 1);
            recipe.AddIngredient(ItemID.FallenStar, 2);
            recipe.AddIngredient(ItemType<StarSilk>(), 10);
            recipe.AddIngredient(ItemID.PixieDust, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}