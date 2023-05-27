using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Ambient
{
	public class FabledRay : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("BoomCirle");
		}
		public override void SetDefaults()
		{
	
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 1;
			Projectile.scale = 3f;
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = false;


			return true;
		}
		public override void AI()
		{
			//Projectile.alpha = Projectile.timeLeft;
		//	Timer++;
		//	if(Timer < 200)
         //   {
		//		Projectile.timeLeft += 1;
         //   }

		////	if (Timer > 200)
		////	{
		//		Projectile.timeLeft -= 1;
		//	}


		//	if (Timer == 300)
		//	{
		//		Projectile.timeLeft = 2;
		//	}


		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 248, 10, 0) * (1f - (float)Projectile.alpha / 255f);
		}
	}
}