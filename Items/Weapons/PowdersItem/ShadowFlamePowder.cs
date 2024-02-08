using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class ShadowFlamePowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("ShadeFlame Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA shadow dust that explodes with your igniter!"); */
		}
		public override void SetDefaults()
		{
			Item.damage = 6;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType <ShadePowderProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.crit = 20;
			Item.UseSound = SoundID.AbigailAttack;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			int dir = player.direction;

			Projectile.NewProjectile(source, position, velocity *= player.GetModPlayer<MyPlayer>().IgniterVelocity, type, damage, knockback, player.whoAmI);
			return false;
		}


	}
}