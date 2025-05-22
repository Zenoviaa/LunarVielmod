using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
	public class PaintBomb2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 27;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 68;
			Projectile.height = 80;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 27;
			Projectile.scale = 1.3f;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 27)
				{
					Projectile.frame = 0;
				}
			}
			return true;
		}
	}
}