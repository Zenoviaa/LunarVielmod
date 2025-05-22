using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class Coinspa : ModProjectile
	{
		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Death Stars");
			Main.projFrames[Projectile.type] = 8;
		}
		public override void SetDefaults()
		{
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 400;
			Projectile.ignoreWater = true;
			Projectile.hostile =false;
			
		}
		public override void AI()
		{
			Projectile.velocity.Y += 0.2f;
			

		}
		public override bool PreAI()
		{
			timer++; 


			Projectile.tileCollide = false;



			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 8)
				{
					Projectile.frame = 0;
				}
			}


			return true;
		}
		
	}
}