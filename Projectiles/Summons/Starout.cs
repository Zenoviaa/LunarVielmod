using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class Starout : ModProjectile
	{
		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Death Stars");

		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 100;
			Projectile.ignoreWater = true;
			Projectile.hostile =false;
		}
		public override void AI()
		{

			

		}
		public override bool PreAI()
		{
			timer++; 


			Projectile.tileCollide = false;

			if (timer == 2)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f);
				Main.dust[dust].scale = 0.6f;
				int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f);
				Main.dust[dust2].scale = 0.6f;
				int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, 0f);
				Main.dust[dust3].scale = 1f;
				timer = 0;
			}
			
		
			

			return true;
		}
		
	}
}