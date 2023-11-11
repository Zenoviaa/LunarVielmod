using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Meatball2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Balls");
			Main.projFrames[Projectile.type] = 32;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.width = 45;
			Projectile.height = 45;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 32;
			Projectile.scale = 0.7f;
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {

			Vector3 RGB = new(2.55f, 2.55f, 0.94f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 1)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 32)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
	
	}
}