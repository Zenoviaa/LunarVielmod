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
			Projectile.penetrate = -1;
			Projectile.timeLeft = 28;
			Projectile.scale = 0.2f;
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;

			return true;
		}
		public override void AI()
		{
			Projectile.scale *= 1.04f;
			Projectile.alpha += 3;
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 248, 10, 0) * (1f - Projectile.alpha / 255f);
		}
	}
}