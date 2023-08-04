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
    [AutoloadEquip(EquipType.Body)]
    public class ElagentBody : ModItem
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

            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<StarSilk>(), 9);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 9);
            recipe.AddIngredient(ItemID.Feather, 12);
            recipe.AddIngredient(ItemID.Bone, 16);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();

        }
    }
}
