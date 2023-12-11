using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class DesertRuneI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcadiz metal");
			/* Tooltip.SetDefault("Woa this is super hard and magical.." +
			"\nWhere does this ore come from?" +
			"\nBest use for metalic weapons and magical items!"); */
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(silver: 20);
		}
	}
}
