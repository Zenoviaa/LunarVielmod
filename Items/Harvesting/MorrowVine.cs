using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class MorrowVine : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Vine");
			/* Tooltip.SetDefault("A useful piece of string!" +
			"\nFrom some Alcadiz warriors of the morrow" +
			"\nUsed in many recipes for weapons, bags and such"); */
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 5);
		}
	}
}
