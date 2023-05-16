using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class MorrowRocks : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Stone");
			/* Tooltip.SetDefault("Woa this is super hard.." +
			"\nAn object from harvesting" +
			"\nBest use for plants, items and morrowed weapons!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 20);
		}
	}


}
