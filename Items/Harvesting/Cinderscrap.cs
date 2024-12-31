using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class Cinderscrap: ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Scrap of Cinder");
			/* Tooltip.SetDefault("Cinder scrap" +
			"\nBurned to infinity" +
			"\nUsed in plants"); */
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
			Recipe recipe = CreateRecipe(3);

			recipe.Register();
			recipe.AddIngredient(ItemID.Cobweb, 1);
			recipe.AddIngredient(ItemID.Wood, 1);
			recipe.AddIngredient(ItemID.Torch, 3);
		}
	}
}
