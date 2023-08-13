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
    [AutoloadEquip(EquipType.Body)]
    public class AcidBody : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("AcidBody");
			// Tooltip.SetDefault("Increases ranged damage by 13% and ranged speed by 10%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 80000;
            Item.rare = 8;

            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
