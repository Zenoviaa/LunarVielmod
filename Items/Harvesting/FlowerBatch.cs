using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class FlowerBatch : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Flower Batch");
			/* Tooltip.SetDefault("These flowers are very pretty.." +
			"\nAn object from harvesting" +
			"\nBest use for plants and some items!"); */
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 50);
		}
	}
}
