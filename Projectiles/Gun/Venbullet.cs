using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class Venbullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 10;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.hostile = false;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public float Timer2;

		public override void AI()
		{
			Timer2++;


			Timer++;
			if (Timer == 3)
			{




				float speedXabc = -Projectile.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYabc = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.00f + Main.rand.Next(0, 0) * 0.0f;


				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabc - 20, Projectile.position.Y + speedYabc - 20, speedXabc * 0, speedYabc * 0, ModContent.ProjectileType<VenShotIN>(), Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
				Timer = 0;


			}


		}
		
		


	}
}
