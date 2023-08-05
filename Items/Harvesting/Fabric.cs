using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Fabric : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcadiz Fabric");
			/* Tooltip.SetDefault("All the way from the Alcadiz temples.." +
			"\nStrong as steel yet so fabric like, its pouring energy!" +
			"\nBest use for magical items!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(silver: 10);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 1);
			recipe.AddIngredient(ItemID.Silk, 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();

		}
	}
}
