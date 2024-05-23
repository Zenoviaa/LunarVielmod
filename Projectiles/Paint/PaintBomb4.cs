using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
	public class PaintBomb4 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 26;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 68;
			Projectile.height = 80;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 52;
			Projectile.scale = 1.3f;
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 26)
				{
					Projectile.frame = 0;
				}
			}
			return true;
		}
	}
}