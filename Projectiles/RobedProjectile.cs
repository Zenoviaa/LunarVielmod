using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    class RobedProjectile : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.height = 11;
            Projectile.width = 10;
            Projectile.friendly = false;
            Projectile.penetrate = 20;
            Projectile.stepSpeed = 2;
            Projectile.damage = 20;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            // Loop through the 4 animation frames, spending 5 ticks on each.
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                {
                    Projectile.frame = 0;
                }
            }




        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {

            for (int i = 0; i < 10; i++)
            {
                int dustType = Main.rand.Next(110, 113);
                var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);

                dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
                dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

                dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
            }

        }
    }
}