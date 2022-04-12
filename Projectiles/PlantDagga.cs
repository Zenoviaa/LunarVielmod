using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
	public class PlantDagga : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("PlantDagga");
		}

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.alpha = 0;
			Projectile.tileCollide = false;
		}

		float counter = 3;
		int trailcounter = 0;
		Vector2 holdOffset = new Vector2(0, -3);
		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];
			if (Projectile.owner == Main.myPlayer)
			{
				Vector2 direction2 = Main.MouseWorld - (Projectile.position);
				direction2.Normalize();
				direction2 *= counter;
				Projectile.ai[0] = direction2.X;
				Projectile.ai[1] = direction2.Y;
				Projectile.netUpdate = true;
			}
			Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);
			if (player.channel)
			{
				Projectile.position = player.position + holdOffset;
				player.velocity.X *= 0.95f;
				if (counter < 9)
				{
					counter += 0.05f;
				}
				Projectile.rotation = direction.ToRotation() - 1.57f;
				if (direction.X > 0)
				{
					holdOffset.X = -10;
					player.direction = 1;
				}
				else
				{
					holdOffset.X = 10;
					player.direction = 0;
				}
				trailcounter++;
				float speedX = Projectile.velocity.X * 2;
				float speedY = Projectile.velocity.Y * 2;
				if (trailcounter % 5 == 0 && Projectile.owner == Main.myPlayer)
					Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<PlantDaggerProj>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f); //predictor trail, please pick a better dust Yuy
			}
			else
			{
				float speedX = Projectile.velocity.X * 2;
				float speedY = Projectile.velocity.Y * 2;
				if (Projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<PlantDaggerProj>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
				}
				Projectile.active = false;
			}
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 30;
			player.itemAnimation = 30;
			//	player.itemRotation = 0;
			return true;
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			float speedX = Projectile.velocity.X * 2;
			float speedY = Projectile.velocity.Y * 2;
			base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}