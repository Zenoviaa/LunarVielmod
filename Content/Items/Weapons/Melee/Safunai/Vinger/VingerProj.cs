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

namespace Stellamod.Content.Items.Weapons.Melee.Safunai.Vinger
{
    public class VingerProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.Pink,
                WindColor = Color.DarkMagenta,
                LightColor = Color.LightPink,
                RimHighlightColor = Color.Pink,
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
            Color glowPink = Color.DeepPink;
            Color darkPink = Color.Lerp(Color.Pink, Color.Black, 0.5f);
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) * 0.01f;
            if (Slam)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity, ProjectileID.SpikyBall, 
                    (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                }

                FXUtil.ShakeCamera(target.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: glowPink,
                    outerGlowColor: darkPink, duration: 25, baseSize: boomSize);

                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(target.Center,
                        innerColor: Color.White,
                        glowColor: glowPink,
                        outerGlowColor: darkPink,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(90);
                }
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: glowPink,
                   outerGlowColor: darkPink, duration: 25, baseSize: 0.12f);


                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
                }
            }
            if (Main.rand.NextBool(8))
                target.AddBuff(BuffID.ShadowFlame, 120);
        }
    }
}
