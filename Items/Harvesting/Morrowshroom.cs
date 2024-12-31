using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class Morrowshroom : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowshroom");
			/* Tooltip.SetDefault("Ew an even weirder mushroom.." +
			"\nBest use for planting"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 10);
		}
	}
}
