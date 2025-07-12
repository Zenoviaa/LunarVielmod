using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles
{
    internal class JiitasKnife : ScarletProjectile
    {
        public SlashTrailer Trailer { get; set; }
        private ref float Timer => ref Projectile.ai[0];
        private Vector2 ShootVelocity;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 64;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                ShootVelocity = Projectile.velocity;
                SoundStyle soundStyle = AssetRegistry.Sounds.Jiitas.JiitasKnifeThrow;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                  innerColor: Color.White,
                  glowColor: Color.LightGray,
                  outerGlowColor: Color.DarkGray, duration: 12, baseSize: 0.03f);
            }

            if (Timer < 60)
            {
                Projectile.velocity = Vector2.Zero;
            }

            Projectile.rotation = ShootVelocity.ToRotation();

            if (Timer == 10)
            {
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightGray,
                        outerGlowColor: Color.DarkGray,
                        baseSize: Main.rand.NextFloat(0.035f, 0.065f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }

            if (Timer == 60)
            {
                SoundStyle soundStyle = AssetRegistry.Sounds.Jiitas.JiitasKnifeSlash;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Projectile.velocity = ShootVelocity;
                Projectile.extraUpdates = 24;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Trailer ??= new SlashTrailer();
            Trailer.DrawTrail(ref lightColor, OldCenterPos);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundStyle shotSound = AssetRegistry.Sounds.Jiitas.JiitasGunShot;
            shotSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shotSound, Projectile.position);

            //IMPACT EFFECT
            FXUtil.ShakeCamera(Projectile.position, 1024, 2);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.DarkGray, duration: 25, baseSize: 0.03f);

            for (float f = 0; f < 4; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.DarkGray,
                    baseSize: Main.rand.NextFloat(0.025f, 0.05f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}
