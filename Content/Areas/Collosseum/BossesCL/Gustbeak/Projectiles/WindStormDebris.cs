using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public class WindStormDebris : BaseWindProjectile,
        IDrawOutlines
    {
        private Vector2 _scale;
        private ref float FallDownTime => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            _scale = Vector2.Lerp(_scale, Vector2.One, 0.1f);


            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                FallDownTime = Main.rand.NextFloat(15, 100);
                Projectile.netUpdate = true;
            }

            Projectile.rotation += 0.02f;
            Projectile.rotation -= Projectile.velocity.Length() * 0.025f;
            if (Timer > FallDownTime)
            {
                Projectile.tileCollide = true;
                if (Projectile.velocity.Y < 16)
                    Projectile.velocity.Y += 1f;
            }
            else
            {
                Projectile.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 12; f++)
            {
                float rot = f / 12f * MathHelper.TwoPi;
                Vector2 velOffset = rot.ToRotationVector2() * 4;
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
            }
            for (float f = 0; f < 4; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }
            SoundEngine.PlaySound(SoundID.Item70, Projectile.position);
            FXUtil.ShakeCamera(Projectile.position, 1024, 8);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawWindSlashes(ref lightColor);
            this.DrawCentered(ref lightColor, _scale);
            return false;
        }

        public override float StripWidth(float progressOnStrip)
        {
            return base.StripWidth(progressOnStrip) * 0.66f;
        }
    }
}
