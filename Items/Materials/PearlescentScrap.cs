using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class PearlescentScrap : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pearlescent Scrap");
			/* Tooltip.SetDefault("True fragments of the moon" +
			"\nA strong resonance with sound and the stars..."); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 50);
		}

		
	}
}
