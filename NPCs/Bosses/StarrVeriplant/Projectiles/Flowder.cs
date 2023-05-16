using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
	public class Flowder : ModProjectile
	{
		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Powdered death flower");

		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 50;
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
		}
		public override void AI()
		{

			

		}
		public override bool PreAI()
		{
			timer++; 


			Projectile.tileCollide = false;

			if (timer == 5)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f);
				Main.dust[dust].scale = 1.4f;
				timer = 0;
			}
			
		
			

			return true;
		}
		
	}
}