using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingFire : ScarletProjectile
    {
        private float _fireTime;
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        private Vector2[] _oldSmokeCenterPos;
        public Vector2[] SmokeOldCenterPos
        {
            get
            {
                if (_oldSmokeCenterPos == null)
                    _oldSmokeCenterPos = new Vector2[SmokeTrailCacheLength];
                return _oldSmokeCenterPos;
            }
            private set
            {
                _oldSmokeCenterPos = value;
            }
        }


        private Vector2 StartWhipPosition;
        private Vector2 TargetWhipPosition;
        private Vector2 InitialVelocity;
        private Vector2 TargetVelocity;


        public override string Texture => TextureRegistry.CandleFlame;

        public int SmokeTrailCacheLength;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(StartWhipPosition);
            writer.WriteVector2(TargetWhipPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            StartWhipPosition = reader.ReadVector2();
            TargetWhipPosition = reader.ReadVector2();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 15;
            SmokeTrailCacheLength = 25;
            Projectile.width = 11;
            Projectile.height = 11;
            Projectile.hostile = true;
            Projectile.light = 0.278f;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            base.AI();
            if (SmokeTrailCacheLength > 0)
            {
                for (int i = SmokeTrailCacheLength - 1; i > 0; i--)
                {
                    SmokeOldCenterPos[i] = SmokeOldCenterPos[i - 1];
                }
                SmokeOldCenterPos[0] = Projectile.Center;
            }

            Color twinColor = GetTwinColor();
            Timer++;
            float lightningAuraProgress = Timer / 180f;
            float easedLightningAuraProgress = Easing.SpikeOutCirc(lightningAuraProgress);
            if (Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }

            if (Timer > 30f)
            {
                Projectile.extraUpdates = 0;
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer < 30 && Timer % 5 == 0)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                  innerColor: twinColor,
                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                  outerGlowColor: Color.Black, duration: 5, baseSize: 0.04f);
            }
            if (Timer == 30)
            {
                LegacyParticle.NewParticle<SkullParticle>(Projectile.Center, Vector2.Zero, Color.Red);
            }
            if (Timer == 70)
            {
                //Ping Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                for (float i = 0; i < 2; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    //     rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(TargetWhipPosition,
                        innerColor: Color.White,
                        glowColor: GetTwinColor(),
                        outerGlowColor: Color.Black,
                        baseSize: 0.1f,
                        duration: 15);
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            if (Timer == 30)
            {
                //Ping Sound
                var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                                  innerColor: twinColor,
                                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                                  outerGlowColor: Color.Red, duration: 12, baseSize: 0.06f);
                part.Scale *= 0.5f;
            }
            if (Timer == 50)
            {
                //Ping Sound
                var part = FXUtil.GlowCircleBoom(TargetWhipPosition,
                                  innerColor: twinColor,
                                  glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                                  outerGlowColor: Color.Black, duration: 12, baseSize: 0.06f);
                part.Scale *= 0.5f;
            }

            if (Timer > 200)
            {
                _fireTime += MathHelper.Lerp(0.1f, 0.0f, (Timer - 200) / 40f);
            }
            else
            {
                _fireTime += 0.1f;
            }

            if (Timer > 90 && Timer % 4 == 0)
            {
                LegacyParticle.NewParticle<FlareParticle>(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero);
            }
            if (Timer > 90 && Timer < 100)
            {
                Projectile.velocity *= 1.1f;
            }
            else if (Timer > 100)
            {
                if (Projectile.velocity.Length() > InitialVelocity.Length())
                {
                    Projectile.velocity *= 0.9f;
                }
            }
            if (Timer % 4 == 0)
            {
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    innerColor: twinColor,
                    glowColor: Color.Lerp(twinColor, Color.Black, 0.5f),
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = -Projectile.velocity.RotatedByRandom(0.6f);
                particle.Scale *= 0.5f;
                particle.Rotation = particle.Velocity.ToRotation();
            }

            if (Timer > 200)
            {
                Projectile.velocity *= 0.96f;
            }
            if (Timer % 6 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.Yellow;
                    spark.outerColor = Color.Red;
                }
            }


            if (Timer > 90)
            {

            }
            if (Timer >= 120 && Projectile.velocity.Length() <= 3)
            {
                Projectile.Kill();
            }

            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1000);
            if (player != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 1);
            }
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        private Color GetTwinColor() => DescendingTwins.GetTwinColor(Variant);


        public float WidthFunction(float completionRatio)
        {
            float w = MathHelper.SmoothStep(26, 54, EasingFunction.QuadraticBump(completionRatio));
            //       w = MathHelper.Lerp(w, 0f, EasingFunction.InOutSine((Timer - 200) / 40f));
            return w;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color twinColor = GetTwinColor();
            Color tipColor = Color.Lerp(twinColor, Color.Lerp(twinColor, Color.DarkBlue, 0.5f), completionRatio);
            Color finalColor = Color.Lerp(twinColor, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
            Color finalColor2 = Color.Lerp(Color.Transparent, finalColor, EasingFunction.QuadraticBump(completionRatio));
            finalColor2 = Color.Lerp(finalColor2, Color.DarkRed, (Timer - 200) / 40f);
            return finalColor2;
        }
        public float SmokeWidthFunction(float completionRatio)
        {
            float w = MathHelper.SmoothStep(0, 75, EasingFunction.QuadraticBump(completionRatio));
            return w;
        }

        public Color SmokeColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.InOutSine(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BlackFireSmokeShader blackSmokeShader = BlackFireSmokeShader.Instance;
            TrailDrawer.Draw(Main.spriteBatch, SmokeOldCenterPos, OldCenterRot, SmokeColorFunction, SmokeWidthFunction, blackSmokeShader, Vector2.Zero);

            BlackFireShader blackFireShader = BlackFireShader.Instance;
            blackFireShader.Time = _fireTime;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color twinColor = GetTwinColor();
            Color darkerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            SoundStyle shot = AssetRegistry.Sounds.Magic.RadianceCast1;
            shot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot, Projectile.position);
            SoundStyle shot2 = SoundID.DD2_BetsyFireballImpact;
            shot2.PitchVariance = 0.3f;
            SoundEngine.PlaySound(shot2, Projectile.position);
            var part = FXUtil.GlowCircleBoom(Projectile.Center,
                              innerColor: twinColor,
                              glowColor: darkerColor,
                              outerGlowColor: Color.Black, duration: 24, baseSize: 0.14f);
            part.Scale *= 1.225f;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (int i = 0; i < SmokeOldCenterPos.Length; i++)
            {
                Vector2 pos = SmokeOldCenterPos[i];
                if (i < 8)
                    continue;
                if (Main.rand.NextBool(4))
                {
                    Vector2 velocity = -Projectile.oldVelocity;
                    Particle<ThickSmokeParticle>.Spawn(pos, velocity * 0.5f, Color.White);
                }
            }

            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Vector2.UnitY.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + offset,
                    innerColor: GetTwinColor(),
                    glowColor: darkerColor,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }

            FXUtil.ShakeCamera(Projectile.position, 100, 4);
            Vector2 position = Projectile.Center;
            Vector2 lvelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * 8;
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: twinColor,
                    outerColor: darkerColor,
                    fadeToColor: Color.Red,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
            sear.innerColor = twinColor;
            sear.outerColor = Color.Lerp(sear.innerColor, Color.Black, 0.5f);
            sear.fadeToColor = Color.Black;
            for (float f = 0; f < 4; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), pVelocity, newColor: Color.Black);
            }

        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);

        }
    }
}
