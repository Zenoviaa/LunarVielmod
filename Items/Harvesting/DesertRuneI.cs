using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class DesertRuneI : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(silver: 20);
			Item.questItem = true;
			Item.rare = ItemRarityID.Quest;
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe.AddIngredient(ItemID.AntlionMandible, 3);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }
    }
}
