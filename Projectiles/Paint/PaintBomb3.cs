using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
	public class PaintBomb3 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 27;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.width = 69;
			Projectile.height = 58;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 27;
			Projectile.scale = 1f;
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