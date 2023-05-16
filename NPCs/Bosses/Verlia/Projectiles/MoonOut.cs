using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
	public class MoonOut : ModProjectile
	{
		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Great Moon Blow");

		}
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 100;
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
			Projectile.aiStyle = 2;
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
				int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, 0f);
				Main.dust[dust3].scale = 1f;
				timer = 0;
			}
			
		
			

			return true;
		}
		
	}
}