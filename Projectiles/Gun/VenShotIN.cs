using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun

{
    public class VenShotIN : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostShotIN");
			Main.projFrames[Projectile.type] = 30;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
			Projectile.scale = 1f;
		}

		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

        public override void AI()
        {
			Projectile.rotation -= 0.03f;
			Vector3 RGB = new(1.59f, 0.23f, 1.91f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 30)
				{
					Projectile.frame = 0;
				}
			}
			return true;		
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(100, 100, 100, 0) * (1f - Projectile.alpha / 50f);
		}
	}
}