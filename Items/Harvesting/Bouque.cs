using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Bouque : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gloria Bouquet");
			/* Tooltip.SetDefault("A bag of flowers for the dear" +
			"\nUsed in various recipes"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(gold: 20);
		}
	}


}
