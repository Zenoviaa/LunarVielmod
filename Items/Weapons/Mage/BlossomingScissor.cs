using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Slashers.Voyager;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
	public class BlossomingScissor : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Swing");
			/* Tooltip.SetDefault("Shoots one bone bolt to swirl and kill your enemies after attacking!" +
			"\nHitting foes with the melee swing builds damage towards the swing of the weapon"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 210;
			Item.DamageType = DamageClass.Magic;
			Item.width = 32;
			Item.mana = 10;
			Item.height = 32;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 0, 20);
			Item.shoot = ModContent.ProjectileType<VoyagerShotProj>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback);

		//	if (Item.shoot == ModContent.ProjectileType<FrostSwProj>())
		//		Item.shoot = ModContent.ProjectileType<FrostSwProj3>();
		//	else
			//	Item.shoot = ModContent.ProjectileType<FrostSwProj>();

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}
	}
}