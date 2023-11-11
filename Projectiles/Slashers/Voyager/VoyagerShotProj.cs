using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Voyager
{
	public class VoyagerShotProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Balls");
			Main.projFrames[Projectile.type] = 17;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 45;
			Projectile.height = 45;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 17;
			Projectile.scale = 0.7f;
			
		}

		bool Moved;
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		int timer2 = 0;
        public override void AI()
        {

			Player player = Main.player[Projectile.owner];

			timer2++;
			if (timer2 == 1)
            {
				player.GetModPlayer<MyPlayer>().SwordComboSlash += 1;

			}
			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
			Projectile.velocity *= .96f;
			Projectile.ai[1]++;
			if (!Moved && Projectile.ai[1] >= 0)
			{
				

				Projectile.spriteDirection = Projectile.direction;
				Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
				for (int j = 0; j < 10; j++)
				{
					Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
					vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
					vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
					int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PinkCrystalShard, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
					Main.dust[num8].scale = 1.3f;
					Main.dust[num8].noGravity = true;
					Main.dust[num8].position = Projectile.Center + vector2;
					Main.dust[num8].velocity = Projectile.velocity * 0.1f;
					Main.dust[num8].noLight = true;
					Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
				}
				Moved = true;
			}
			if (Projectile.ai[1] >= 20)
			{
				Projectile.tileCollide = true;
			}
			if (Projectile.alpha <= 255)
			{
				Projectile.alpha += 7;
			}
			if (Projectile.alpha >= 255)
			{

			}

			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - (float)Projectile.alpha / 50f);
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 17)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
	
	}
}