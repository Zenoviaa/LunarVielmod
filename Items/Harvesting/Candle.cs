using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Candle : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Candle");
			/* Tooltip.SetDefault("A candle but you cant place it :(" +
			"\nWhy does it exist?" +
			"\nUsed in many recipes"); */
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
