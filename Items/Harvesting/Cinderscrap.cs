using Terraria;
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
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 5);
		}
	}


}
