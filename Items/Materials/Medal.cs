using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
	internal class Medal : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ruin Medal");
			Tooltip.SetDefault("Ew! Its sticky! I wonder what else is sticky..." +
			"\nA sticky substance that resonates with the stars or the morrow!");
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.buyPrice(0, 20, 0, 0);
		}
	}
}
