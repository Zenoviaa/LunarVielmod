using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class FunBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 44;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 194;
            Projectile.height = 166;
            Projectile.scale = 1f;
            Projectile.timeLeft = 44;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.OrangeRed);
            }

            Projectile.rotation -= 0.01f;
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 44)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }
}