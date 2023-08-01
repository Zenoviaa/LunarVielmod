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
    [AutoloadEquip(EquipType.Body)]
    public class ShadeWraithBody : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shade Wraith Body");
			// Tooltip.SetDefault("Increases endurance by 13%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 80000;
            Item.rare = 14;

            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 9);
            recipe.AddIngredient(ItemID.DemoniteBar, 6);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemType<DarkEssence>(), 9);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 6);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
    }
}
