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
using Stellamod.Items.Materials.Tech;


namespace Stellamod.Items.Armors.HeavyMetal
{
    [AutoloadEquip(EquipType.Legs)]
    public class HeavyMetalLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("HeavyMetal Legs");
			// Tooltip.SetDefault("Increases movement speed by 10%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.05f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<MetallicOmniSource>(), 1);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 4);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemType<MetallicOmniSource>(), 1);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 4);
            recipe2.AddIngredient(ItemID.LeadBar, 2);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }


    }
}
