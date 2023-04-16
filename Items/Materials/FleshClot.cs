using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
	internal class FleshClot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kaeva Clot");
			Tooltip.SetDefault("Flesh-like substance used for many items!" +
			"\nObtained from blood-typed enemies");
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
