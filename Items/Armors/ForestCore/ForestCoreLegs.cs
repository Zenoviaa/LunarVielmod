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

namespace Stellamod.Items.Armors.ForestCore
{
    [AutoloadEquip(EquipType.Legs)]
    public class ForestCoreLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Forest Core Boots");
			// Tooltip.SetDefault("Increases movement speed by 10%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 2;

            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 6);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 5);
            recipe.AddIngredient(ItemID.WoodGreaves, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }


    }
}
