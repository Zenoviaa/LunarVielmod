using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
    public class Flowder : ModProjectile
	{
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

		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (Projectile.timeLeft % 5 == 0)
            {
				int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 
					ModContent.DustType<Sparkle>(), 0f, 0f);
				Main.dust[dust].scale = 1.4f;
			}

			return true;
		}
	}
}