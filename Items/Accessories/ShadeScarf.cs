
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories
{
    public class ShadeScarf : ModItem
    {
        public bool SScarf = false;
        public override void SetStaticDefaults()
        {

            // DisplayName.SetDefault("Shade Scarf");
            // Tooltip.SetDefault("Has a chance to gift regeneration on striking an enemy");
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(ItemID.DemoniteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemType<DarkEssence>(), 10);
            recipe2.AddIngredient(ItemID.Leather, 5);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 10);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
