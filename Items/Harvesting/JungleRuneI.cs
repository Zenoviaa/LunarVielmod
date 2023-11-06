using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class JungleRuneI : ModItem
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
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 20);
		}
	}


}
