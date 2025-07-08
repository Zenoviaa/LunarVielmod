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

namespace Stellamod.Content.Items.Weapons.Ranged.Safunai.Halhurish
{
    public class HalhurishProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.Red,
                WindColor = Color.OrangeRed,
                LightColor = Color.Orange,
                RimHighlightColor = Color.Yellow,
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
            //Play Explosion SOund
            SoundStyle expSound = AssetRegistry.Sounds.Melee.MorrowExp;
            expSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(expSound, target.position);


            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
            if (Slam)
            {
                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
                }

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
                }

                FXUtil.ShakeCamera(target.Center, 1024, 32);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(target.Center, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }


                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(target.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.12f);


                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 0.5f).noGravity = true;
                }

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.5f).noGravity = true;
                }
            }
        }
    }
}
