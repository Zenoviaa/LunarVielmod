using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
    public class PaintBomb6 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 28;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 68;
			Projectile.height = 80;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 56;
			Projectile.scale = 1.5f;
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 28)
				{
					Projectile.frame = 0;
				}
			}
			return true;
		}
	}
}