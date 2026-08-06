using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class OverworldRuneI : ModItem
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
            recipe.AddIngredient(ItemID.Leather, 3);
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.Register();
        }
    }
}
