using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Hlos : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magical Handle");
			Tooltip.SetDefault("Magical Handle omg!" +
			"\nBest use for arcanal weapons");

			Item.damage = 4;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.knockBack = 5;
			Item.shootSpeed = 6;
			Item.autoReuse = true;
			Item.rare = ItemRarityID.Blue;
			Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.SeedlerThorn;
			Item.value = Item.sellPrice(silver: 20);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
	}
}
