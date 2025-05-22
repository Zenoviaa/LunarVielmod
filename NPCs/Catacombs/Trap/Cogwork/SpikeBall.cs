using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Cogwork
{
    internal class SpikeBall : ModProjectile
	{
        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.penetrate = 32;
			Projectile.timeLeft = 180;
			Projectile.light = 0.75f;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] += 0.1f;
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.velocity.X = -oldVelocity.X;
				}
				if (Projectile.velocity.Y != oldVelocity.Y)
				{
					Projectile.velocity.Y = -oldVelocity.Y;
				}
				Projectile.velocity *= 0.94f;
			}

			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			return false;
		}


		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 0.7f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}

		public Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.DarkGray, Color.Transparent, completionRatio);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
			DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkGray, Color.Transparent, ref lightColor);
			return base.PreDraw(ref lightColor);
		}

		public override void AI()
		{
			//We don't want these to crit
			Projectile.rotation += 0.25f;
			Projectile.velocity.Y += 0.3f;
			Vector2 speed = new Vector2(0, -1);
            if (Main.rand.NextBool(3))
            {
				for (int i = 0; i < 1; i++)
				{
					Dust.NewDust(Projectile.Center, 2, 2, DustID.Iron, speed.X, speed.Y);
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] += 0.1f;
			Projectile.velocity *= 0.75f;
		}
	}
}
