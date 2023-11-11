using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Voyager
{
	public class InkBullet1 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("MeatBall");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 12;
			Projectile.height = 24;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ownerHitCheck = true;
			Projectile.timeLeft = 360;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;

		public override void AI()
		{
			Timer2++;
			Projectile.velocity *= 0.97f;
			Timer++;
			if (Timer == 4)
			{

				if (Main.rand.NextBool(2))
				{

					float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXabc * 0f, speedYabc * 0f, ModContent.ProjectileType<Ink1>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
				}

				if (Main.rand.NextBool(1))
				{

					float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXabc * 0f, speedYabc * 0f, ModContent.ProjectileType<Ink2a>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
				}

				if (Main.rand.NextBool(3))
				{

					float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X , Projectile.position.Y , speedXabc * 0f, speedYabc * 0f, ModContent.ProjectileType<Ink3>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
				}


				//	float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				//	float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;


				//	Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabc, Projectile.position.Y + speedYabc, speedXabc * 0, speedYabc * 0, ModContent.ProjectileType<VoyagerShotProj>(), (int)(Projectile.damage * 1), 0f, Projectile.owner, 0f, 0f);
				Timer = 0;


			}






		}
	}
}