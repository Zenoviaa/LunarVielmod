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

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Legs)]
    public class ElagentLegs : ModItem
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

            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<AzureFragments>(), 9);
            recipe.AddIngredient(ItemID.Feather, 6);
            recipe.AddIngredient(ItemID.Bone, 7);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();
        }
    }
}
