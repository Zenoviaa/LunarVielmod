using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class BoomCircle : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 28;
			Projectile.scale = 0.9f;
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworkFountain_Yellow, 0f, 0f);
			int moredust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, 0f);
			int evenmoredust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworkFountain_Pink, 0f, 0f);
			Main.dust[dust].scale = 0.4f;
			Main.dust[moredust].scale = 0.3f;
			Main.dust[evenmoredust].scale = 0.3f;

			return true;
		}
		public override void AI()
		{
			Projectile.scale *= 0.96f;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 248, 10, 0) * (1f - Projectile.alpha / 255f);
		}
	}
}