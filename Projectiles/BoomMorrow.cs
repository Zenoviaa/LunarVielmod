using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class BoomMorrow : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomMorrow");
			Main.projFrames[Projectile.type] = 28;
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 46;
			Projectile.height = 46;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 45;
			Projectile.scale = 0.9f;
			Projectile.light = 1.5f;
			Projectile.CloneDefaults(ProjectileID.DaybreakExplosion);
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<SalfaceDust>(), 0f, 0f);
			Main.dust[dust].scale = 1f;
			return true;
		}
		public override void AI()
		{
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 28)
				{
					Projectile.frame = 0;
				}
			}
		}
	}
}