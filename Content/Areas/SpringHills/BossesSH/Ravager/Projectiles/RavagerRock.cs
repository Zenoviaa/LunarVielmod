using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Ravager.Projectiles
{
    public class RavagerRock : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.hostile = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.Outline(Color.Red, ref lightColor);
            this.DrawCentered(ref lightColor);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
            }
            if (Projectile.velocity.Y < 5)
            {
                Projectile.velocity.Y = 5;
            }
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += 0.015f;
            Projectile.rotation += Projectile.velocity.Length() * 0.002f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 12; f++)
            {
                float lerp = f / 12f;
                float rot = lerp * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2();
                vel *= Main.rand.NextFloat(2, 5);
                Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
}
