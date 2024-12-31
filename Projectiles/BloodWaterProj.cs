using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class BloodWaterProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 32;
            Projectile.timeLeft = 150;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 24;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] += 0.1f;
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.velocity.X = -oldVelocity.X;
				}
				if (Projectile.velocity.Y != oldVelocity.Y)
				{
					Projectile.velocity.Y = -oldVelocity.Y;
				}
				Projectile.velocity *= 0.9f;
			}

			return false;
		}

        public override void AI()
        {
			//We don't want these to crit
			Projectile.CritChance = 0;
			Projectile.rotation += 0.25f;
			Projectile.velocity.Y += 0.5f;
			Vector2 speed = new Vector2(0, -1);
			for(int i = 0; i < 2; i++)
            {
				Dust.NewDust(Projectile.Center, 2, 2, DustID.BloodWater, speed.X, speed.Y);
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] += 0.1f;
			Projectile.velocity *= 0.75f;
		}

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
			int count = Main.rand.Next(15, 30);
			for (int i = 0; i < count; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(2, 2);
				Dust.NewDust(Projectile.Center, 2, 2, DustID.BloodWater, speed.X, speed.Y);
			}
		}
    }
}
