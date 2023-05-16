using Terraria;
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
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 100);
		}
	}
}
