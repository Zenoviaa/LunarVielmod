using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories
{
    public class WoodShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Shield");
        }
        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}