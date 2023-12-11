using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class Medal : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ruin Medal");
			/* Tooltip.SetDefault("An old ancient medal.." +
			"\nKeep this in your inventory, can be sold and may attract someone, romantically..."); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.buyPrice(0, 20, 0, 0);
		}
	}
}
