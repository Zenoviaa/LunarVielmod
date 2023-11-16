using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class FireflyBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.PreDrawAnimatedProjectile(this, ref lightColor);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public void FadeInAndOut()
        {
            // If last less than 50 ticks — fade in, than more — fade out
            if (Projectile.ai[0] <= 50f)
            {
                // Fade in
                Projectile.alpha -= 25;
                // Cap alpha before timer reaches 50 ticks
                if (Projectile.alpha < 100)
                    Projectile.alpha = 100;

                return;
            }

            // Fade out
            Projectile.alpha += 25;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }

        public override bool PreAI()
        {
            int num1222 = 74;
            if (Main.rand.NextBool(4))
            {
                for (int k = 0; k < 2; k++)
                {
                    int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CopperCoin, Scale: 0.95f);
                    Main.dust[dust].position = Projectile.Center - Projectile.velocity / num1222 * k;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLight = false;
                }
            }


            return base.PreAI();
        }

        public override void AI()
        {
            base.AI();
            Projectile.ai[0] += 1f;
            FadeInAndOut();
            Projectile.velocity *= 0.98f;

            //Animate It
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            //Despawn after 3 seconds.
            if (Projectile.ai[0] >= 180f)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            int count = 8;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 2;
                Dust.NewDust(Projectile.position, 0, 0, DustID.CopperCoin, vel.X, vel.Y);
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starblast"), Projectile.position);
        }
    }
}
