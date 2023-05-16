using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class DreasFlower : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sky Blue Dreas Flower");
			/* Tooltip.SetDefault("All the way from the Alcadiz temples.." +
			"\nUsed for planting and harvesting" +
			"\nBest use for magical items and plants!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 100);
		}
	}
}
