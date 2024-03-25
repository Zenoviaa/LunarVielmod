using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Slashers.Voyager;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
	public class BlossomingScissor : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Melee;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.mana = 0;
			Item.damage = 240;
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
			Item.knockBack = 10;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.DD2_SonicBoomBladeSlash;
			Item.autoReuse = false;
			Item.value = 100000;
			Item.shoot = ModContent.ProjectileType<VoyagerShotProj>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

		//	if (Item.shoot == ModContent.ProjectileType<FrostSwProj>())
		//		Item.shoot = ModContent.ProjectileType<FrostSwProj3>();
		//	else
			//	Item.shoot = ModContent.ProjectileType<FrostSwProj>();

			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}
	}
}