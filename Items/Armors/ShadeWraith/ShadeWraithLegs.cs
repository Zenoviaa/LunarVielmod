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

namespace Stellamod.Items.Armors.ShadeWraith
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShadeWraithLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shade Wraith Legs");
			// Tooltip.SetDefault("Increases melee critical strike chance by 8% and movement speed by 10%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 5);
            recipe.AddIngredient(ItemID.DemoniteBar, 6);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemType<DarkEssence>(), 5);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 6);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
    }
}
