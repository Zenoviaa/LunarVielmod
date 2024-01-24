using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
    public class Flowing : ModProjectile
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
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.98f;
			var entitySource = Projectile.GetSource_FromAI();
			timer++;
			if (timer == 30)
            {		
				ParticleManager.NewParticle(Projectile.Center, Projectile.velocity * 1, ParticleManager.NewInstance<Strip>(), Color.HotPink, Main.rand.NextFloat(1f, 1f));
			}
			if (timer == 60)
			{
				if(Main.myPlayer == Projectile.owner)
				{
                    NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y,
                        ModContent.NPCType<Flowerball>());
                }
			}
			if (timer == 70)
			{
				Projectile.Kill();

			}

		}
		
		
	}
}