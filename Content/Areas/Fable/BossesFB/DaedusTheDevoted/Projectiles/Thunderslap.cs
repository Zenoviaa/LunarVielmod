using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class ThunderSlap : ModProjectile
    {
        private Vector2 _lightningHitPos;
        private bool _calculatedStrikePoints;
        public float BeamLength;
        public Vector2[] BeamPoints;
        public float[] BeamRot;
        private float _lightningPower;
        private float _lightningTime;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);
                _lightningPower = 10;
                Player targetPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, 1680);
                float offset = ProjectileHelper.PerformBeamHitscan(targetPlayer.Bottom - Vector2.UnitY, -Vector2.UnitY, 2400);
                Projectile.position = targetPlayer.Bottom + new Vector2(0, -offset * 0.7f);
            }

            if (Timer == 15)
            {
                _lightningPower = 5;
            }

            if (Timer == 15)
            {
                _lightningPower = 30;
            }
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Vector2.UnitY, 2400);
            BeamLength = targetBeamLength;
            if (Timer == 30)
            {
                _lightningPower = 0.9f;
                _lightningTime = 0;
                //Sound Effect Goooo
                SoundStyle lightningSoundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
                lightningSoundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                    Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                    d.noGravity = true;
                }


                _lightningHitPos = Projectile.position + new Vector2(0, BeamLength);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(_lightningHitPos, 1024, 40);

                for (int i = 0; i < 1; i++)
                {
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), _lightningHitPos, velocity,
                        ModContent.GoreType<FableRock1>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), _lightningHitPos, velocity,
                        ModContent.GoreType<FableRock2>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), _lightningHitPos, velocity,
                        ModContent.GoreType<FableRock3>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), _lightningHitPos, velocity,
                        ModContent.GoreType<FableRock4>());
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), _lightningHitPos + new Vector2(0, 24), Vector2.Zero,
                        ModContent.ProjectileType<GroundCracking>(), 0, 0, Projectile.owner);
                }

                var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue, duration: 12, baseSize: 0.14f);
                part.Scale *= 2;
                for (float f = 0; f < 32; f++)
                {
                    Dust.NewDustPerfect(_lightningHitPos, DustID.Torch,
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }


                for (float i = 0; i < 15; i++)
                {
                    float rot = rot = -Vector2.UnitY.ToRotation();
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);

                    Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                    var particle = FXUtil.GlowCircleDetailedBoom1(_lightningHitPos + offset,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                        duration: Main.rand.NextFloat(5, 25));
                    particle.Velocity = velocity;
                    particle.Scale *= 0.35f;
                    particle.Rotation = rot;
                }


                Vector2 position = _lightningHitPos;
                Vector2 lvelocity = -Vector2.UnitY * 8;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.White,
                        outerColor: Color.Yellow,
                        fadeToColor: Color.Purple,
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

                var sear = LegacyParticle.NewParticle<SearParticle>(_lightningHitPos, Vector2.Zero);
                sear.Scale *= 2;
                for (int i = 0; i < BeamPoints.Length; i++)
                {
                    if (Main.rand.NextBool(16))
                    {
                        Vector2 pos = BeamPoints[i];
                        pos += Main.rand.NextVector2Circular(32, 32);
                        var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));

                    }
                }
            }

            if (Timer > 35)
            {
                _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



            }

            if (Timer == 42)
            {
                _lightningPower = 1.5f;
            }
            if (Timer == 42)
            {
                var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
            }
            if (Timer == 52)
            {
                _lightningPower = 2.3f;
            }
            if (Timer == 52)
            {
                var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
            }


            if (Timer == 58)
            {
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(_lightningHitPos + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.Scale *= 0.5f;
                    spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                }
            }
            _lightningTime -= 0.1f;
            if (!_calculatedStrikePoints)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float numPoints = 80;
                float randOffset = Main.rand.NextFloat(-1f, 1f);
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + direction * BeamLength;
                end.X += Main.rand.Next(-16, -16);
                for (float i = 0; i <= numPoints; i++)
                {


                    float interp = i / numPoints;
                    Vector2 point = Vector2.Lerp(start, end, interp);
                    point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                    //if(i % 4 == 0)
                    //point.X += Main.rand.Next(-16, 16);
                    beamPoints.Add(point);
                }

                BeamPoints = beamPoints.ToArray();
                BeamRot = new float[BeamPoints.Length];

                _calculatedStrikePoints = true;
            }
        }

        public override bool? CanDamage()
        {
            return Timer > 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningShader lightningShader = LightningShader.Instance;
            lightningShader.Time = _lightningTime;
            lightningShader.Power = _lightningPower;
            TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);
            if (Timer >= 30)
                TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);

            return false;
        }

        private float LightningWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(242, 0, completionRatio);
        }

        private Color LightningColorFunction(float completionRatio)
        {
            Color lerpColor = Color.Lerp(Color.White, Color.Blue, (Timer - 30f) / 30f);
            return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
        }
    }
}
