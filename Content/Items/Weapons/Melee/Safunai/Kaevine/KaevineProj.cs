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

namespace Stellamod.Content.Items.Weapons.Melee.Safunai.Kaevine
{
    public class KaevineProj : BaseSafunaiProjectile
    {
        public SlashEffect SlashEffect { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            //Define shader, set the shader
            SlashEffect = new()
            {
                BaseColor = Color.Green,
                WindColor = Color.DarkOliveGreen,
                LightColor = Color.LightGreen,
                RimHighlightColor = Color.LightGreen,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend
            };

            SlashEffect.TrailTexture = AssetRegistry.Textures.Trails.StringySlash1.Value;
            SlashEffect.HighlightTexture = AssetRegistry.Textures.Trails.StringySlash2.Value;
            SlashEffect.WindTexture = AssetRegistry.Textures.Trails.StringySlash3.Value;
            SlashEffect.RimHighlightTexture = AssetRegistry.Textures.Trails.StringySlash4.Value;

            Trailer.Shader = SlashEffect;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer.TrailWidthFunction = GetTrailWidth;
        }
        private float GetTrailWidth(float interpolant)
        {
            return EasingFunction.InOutCubic(interpolant) * 36;
        }
        private Color GetTrailColor(float interpolant)
        {
            return Color.Lerp(Color.LightGray, Color.Transparent, interpolant) * 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            int count = 24;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 4;
                Dust.NewDust(target.Center, 0, 0, DustID.Venom, vel.X, vel.Y);
            }

            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, target.position);
            SoundEngine.PlaySound(SoundID.Item17, target.position);
            if (Slam)
            {
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 stingerVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.NextFloat(6, 8);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        target.Center.X, target.Center.Y,
                        stingerVelocity.X, stingerVelocity.Y,
                        ProjectileID.QueenBeeStinger, Projectile.damage * 3, 0f, Projectile.owner);

                    Projectile stingerProj = Main.projectile[p];
                    stingerProj.hostile = false;
                    stingerProj.friendly = true;
                    stingerProj.usesLocalNPCImmunity = true;
                    stingerProj.penetrate = -1;
                    stingerProj.localNPCHitCooldown = -1;
                    stingerProj.netUpdate = true;
                }
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red, duration: 15, baseSize: 0.12f);


                FXUtil.ShakeCamera(target.Center, 1024, 32);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);

                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Green, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkOliveGreen, 1f).noGravity = true;
                }
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);

            }
            else
            {
                FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.Yellow,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);

                SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger;
                hitSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(hitSound, target.position);
                for (int i = 0; i < 1; i++)
                {
                    Vector2 stingerVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.NextFloat(4, 6);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        target.Center.X, target.Center.Y,
                        stingerVelocity.X, stingerVelocity.Y,
                        ProjectileID.QueenBeeStinger, Projectile.damage * 2, 0f, Projectile.owner);

                    Projectile stingerProj = Main.projectile[p];
                    stingerProj.hostile = false;
                    stingerProj.friendly = true;
                    stingerProj.usesLocalNPCImmunity = true;
                    stingerProj.penetrate = -1;
                    stingerProj.localNPCHitCooldown = -1;
                    stingerProj.netUpdate = true;

                }
            }

        }


    }
}
