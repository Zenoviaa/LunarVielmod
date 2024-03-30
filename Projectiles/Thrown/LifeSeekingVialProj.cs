using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Thrown
{
	public class LifeSeekingVialProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flask of KABOOM");
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

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, 0f, 0f);
			Main.dust[dust].scale = 0.6f;
		}


		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player Player = Main.player[Projectile.owner];
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GlassBreak>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
			float Speed = Main.rand.Next(4, 7);
			float offsetRandom = Main.rand.Next(0, 50);
			Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2048f, 12f);

			float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(1, 0) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;


			for (int i = 0; i < 1; i++)
			{
				Player.Heal(2);
				offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Main.myPlayer);

				Projectile.netUpdate = true;
			}

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
			for (int j = 0; j < 5; j++)
			{
				Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
				vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
				vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
				int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
				Main.dust[num8].scale = 1.3f;
				Main.dust[num8].noGravity = true;
				Main.dust[num8].position = Projectile.Center + vector2;
				Main.dust[num8].velocity = Projectile.velocity * 0.1f;
				Main.dust[num8].noLight = true;
				Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
			}


			SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
			Projectile.Kill();
			return false;
		}

	}
}