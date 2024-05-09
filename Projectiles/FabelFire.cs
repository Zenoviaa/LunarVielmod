using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class FabelFire : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shadow Ball");
			Main.projFrames[Projectile.type] = 4;
		}
		public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 60;
			Projectile.scale = 0.5f;
		}
		public override bool PreAI()
		{
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 4)
				{
					Projectile.frame = 0;
				}
			}

			Projectile.tileCollide = true;
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.3f).noGravity = true;
            return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.4f).noGravity = true;
            }
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.4f).noGravity = true;
            }
            return false;
		}
		public override void OnKill(int timeLeft)
		{
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.oldVelocity.X * 0.3f, Projectile.oldVelocity.Y * 0.3f);
		}
	}
}