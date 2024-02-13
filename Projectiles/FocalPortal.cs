using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class FocalPortal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
       
                Rectangle playerRect = player.getRect();
                if(Projectile.Colliding(myRect, playerRect))
                {
                    //Teleport
                    Teleport(player);
                }
            }

            float range = 0.25f;
            float hover = VectorHelper.Osc(-range, range);
            Vector2 targetCenter = Projectile.Center + new Vector2(0, hover);
            Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter, 5);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
        }

        private void Teleport(Player player)
        {
            int x = (int)Projectile.ai[0];
            int y = (int)Projectile.ai[1];
            player.Center = new Vector2(x, y);
        }
    }
}
