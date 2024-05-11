using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Vixyl
{
    internal class VixylParryProj : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Empress's Moon Slash");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 14;
            Projectile.localNPCHitCooldown = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 1.5f;
            DrawOffsetX = -100;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer == 2)
            {
                Projectile.scale *= 0.98f;
                Timer = 0;
            }


            if (Projectile.scale == 0f)
            {
                Projectile.Kill();
            }

            //Visual Stuff
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }
    }
}
