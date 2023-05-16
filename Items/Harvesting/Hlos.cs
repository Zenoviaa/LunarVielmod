using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Hlos : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magical Handle");
			/* Tooltip.SetDefault("Magical Handle omg!" +
			"\nBest use for arcanal weapons"); */

		
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 20);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
	}
}
