using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Paint
{
	public class PaintBomb8 : ModProjectile
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
			Projectile.timeLeft = 54;
			Projectile.scale = 1.5f;

		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override void AI()
		{

			//Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			/// The multiplication here wasn't doing anything
			//Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
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