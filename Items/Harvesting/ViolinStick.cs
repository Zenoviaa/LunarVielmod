using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class ViolinStick : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Violin Stick");
			/* Tooltip.SetDefault("Hmph, what is this used for?" +
			"\nBest use for weapons and musical items!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 6;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.knockBack = 5;
			Item.autoReuse = true;
			Item.rare = ItemRarityID.Blue;
			Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(silver: 20);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
	}
}
