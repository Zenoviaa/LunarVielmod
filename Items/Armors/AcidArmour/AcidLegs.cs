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

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Legs)]
    public class AcidLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Legs");
			// Tooltip.SetDefault("Increases Acceleration by 5% and movement speed by 4%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed += 0.04f;
            player.runAcceleration += 0.12f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<ArmorDrive>(), 1);
            recipe.AddIngredient(ItemType<VirulentPlating>(), 4);
            recipe.AddIngredient(ItemType<IrradiatedBar>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
