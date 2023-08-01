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
using Stellamod.Items.Materials.HardMode;

namespace Stellamod.Items.Armors.Dreamer
{
    [AutoloadEquip(EquipType.Legs)]
    public class DreamerLegs : ModItem
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

            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DreamerBar>(), 8);
            recipe.AddIngredient(ItemID.SoulofSight, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }


    }
}
