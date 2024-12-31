using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class CondensedDirt : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Condensed Dirt");
			/* Tooltip.SetDefault("Ew! Its sticky! Why??" +
			"\nA sticky dirt-like substance that comes from the ground!" +
			"\nBest use for fertilizer and dirt-like items!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 5);
		}



		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 5);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}


}
