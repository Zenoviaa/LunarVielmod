using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Bagitem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust Bag");
			Tooltip.SetDefault("A bag made for igniter dusts!" +
			"\nWhere does this come from?" +
			"\nUsed in every dust recipe, bring to the dust bench!");
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
