using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Stick : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stick");
			Tooltip.SetDefault("Welp, its a stick" +
			"\nBest use for weapons and planting items!");
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
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
			Item.value = Item.sellPrice(silver: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
	}
}
