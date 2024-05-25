using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Crossbows.Eckasect
{
	public class EckasectGenesisSwitch : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 60;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = 2;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
		}

		public override void AI()
		{
			Vector3 RGB = new(1.00f, 0.37f, 0.30f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f);
			Main.dust[dust].scale = 0.6f;
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

			for (int j = 0; j < 40; j++)
			{
	
				Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
				ParticleManager.NewParticle(Projectile.Center, speed2 * 5, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
			}
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"), Projectile.position);
			Projectile.Kill();
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			for (int j = 0; j < 40; j++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
				Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
				ParticleManager.NewParticle(Projectile.Center, speed2 * 5, ParticleManager.NewInstance<morrowstar>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"));

			}
			Projectile.Kill();
			return false;
		}

		public override bool PreAI()
		{		
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 60)
				{
					Projectile.frame = 0;
				}
			}
			return true;
		}
	}
}