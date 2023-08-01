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

namespace Stellamod.Items.Armors.Winterborn
{
    [AutoloadEquip(EquipType.Body)]
    public class WinterbornBody : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Winterborn Body");

		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 80000;
            Item.rare = 14;

            Item.defense = 3;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BorealWood, 20);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 11);
            recipe.AddIngredient(ItemID.SnowBlock, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
