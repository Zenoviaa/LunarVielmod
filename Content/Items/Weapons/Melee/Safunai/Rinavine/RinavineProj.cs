using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Dusts;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Urdveil.Common.Bases;

namespace Stellamod.Content.Items.Weapons.Melee.Safunai.Rinavine
{
    public class RinavineProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.LightGreen,
                WindColor = Color.DarkTurquoise,
                LightColor = Color.Turquoise,
                RimHighlightColor = Color.White,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
            };

            Trailer.Shader = SlashEffect;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer.TrailWidthFunction = GetTrailWidth;
        }

        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutCubic(interpolant) * 24;
        }

        private Color GetTrailColor(float interpolant)
        {
            return Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.3f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, target.Center);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) * 0.01f;
            if (Slam)
            {
                //Hit Sound
                SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine;
                parendineHitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(parendineHitSound, target.Center);

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Turquoise, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.MediumTurquoise, 1f).noGravity = true;
                }
                FXUtil.ShakeCamera(target.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.LightGreen,
                    glowColor: Color.Turquoise,
                    outerGlowColor: Color.DarkBlue, duration: 25, baseSize: boomSize);


                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(target.Center,
                        innerColor: Color.LightGreen,
                        glowColor: Color.Turquoise,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(Main.rand.NextFloat(0, 360));
                }
            }
            else
            {
                float boomSize = Main.rand.NextFloat(0.08f, 0.12f);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.LightGreen,
                    glowColor: Color.Turquoise,
                    outerGlowColor: Color.DarkBlue, duration: 25, baseSize: boomSize);

                SoundStyle parendineHitSound = AssetRegistry.Sounds.Melee.Parendine2;
                parendineHitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(parendineHitSound, target.Center);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Turquoise, 0.5f).noGravity = true;
                }
            }
        }
    }
}
