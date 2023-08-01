using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Armors.Leather
{
    [AutoloadEquip(EquipType.Legs)]
    public class LeatherLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Leather Legs");
			// Tooltip.SetDefault("Increases movement speed by 10%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 3);
            recipe.AddIngredient(ItemID.Leather, 6);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 3);
            recipe2.AddIngredient(ItemID.Leather, 6);
            recipe2.AddIngredient(ItemID.LeadBar, 2);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }


    }
}
