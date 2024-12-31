using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Stellamod.Projectiles.IgniterExplosions
{
    internal class CinderBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
    }
}
