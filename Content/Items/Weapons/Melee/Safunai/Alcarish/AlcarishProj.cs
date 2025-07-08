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

namespace Stellamod.Content.Items.Weapons.Melee.Safunai.Alcarish
{
    public class AlcarishProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.Gray,
                WindColor = Color.DarkGray,
                LightColor = Color.LightGray,
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
            return Color.Lerp(Color.LightGray, Color.Transparent, interpolant) * 0.3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;

            if (Slam)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                    target.Center.X + speedX, 
                    target.Center.Y + speedY, speedX, speedY, ProjectileID.ThrowingKnife, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                    target.Center.X + speedX, 
                    target.Center.Y + speedY, speedX * 2, speedY, ProjectileID.ThrowingKnife, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

                SoundStyle explosionSound = AssetRegistry.Sounds.Melee.MorrowExp;
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, target.position);
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
                }


                FXUtil.ShakeCamera(target.Center, 1024, 32);
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                   innerColor: Color.White,
                   glowColor: Color.Black,
                   outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 0.5f).noGravity = true;
                }
            }
        }
    }
}
