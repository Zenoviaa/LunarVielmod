using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class RadiantZap : ModProjectile
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
            }

            if (Timer == 15)
            {
                _lightningPower = 5;
            }

            if (Timer == 15)
            {
                _lightningPower = 30;
            }
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, 2400);
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
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                    Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                    d.noGravity = true;
                }

                _lightningHitPos = Projectile.Center + Projectile.velocity * BeamLength;
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
                Vector2 lvelocity = -Projectile.velocity * 8;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = Particle.NewParticle<GlowFragmentParticle>(position, pVelocity);
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
                    var spark = Particle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                var sear = Particle.NewParticle<SearParticle>(_lightningHitPos, Vector2.Zero);

                for (int i = 0; i < BeamPoints.Length; i++)
                {
                    if (Main.rand.NextBool(16))
                    {
                        Vector2 pos = BeamPoints[i];
                        pos += Main.rand.NextVector2Circular(32, 32);
                        var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));

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
                    Vector2 pVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<ZapParticle>(_lightningHitPos + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.Scale *= 0.5f;
                    spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                }
            }
            _lightningTime -= 0.1f;
            if (!_calculatedStrikePoints && BeamLength > 0)
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
            if (!_calculatedStrikePoints)
                return false;
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
            return MathHelper.Lerp(180, 0, completionRatio);
        }

        private Color LightningColorFunction(float completionRatio)
        {
            Color lerpColor = Color.Lerp(Color.White, Color.Blue, (Timer - 30f) / 30f);
            return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
        }
    }

    public class RadiantBall : ModProjectile
    {
        private float _scale;
        private ref float Timer => ref Projectile.ai[0];
        private ref float AttackNum => ref Projectile.ai[1];
        private int NPCIndex => (int)Projectile.ai[2];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            base.AI();

            float targetScale = ExtraMath.Osc(0.8f, 1f, speed: 2);
            _scale = MathHelper.Lerp(_scale, targetScale, 0.1f);
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Enrage");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

            }
            if (Timer % 12 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }
            }

            NPC npc = Main.npc[NPCIndex];
            Projectile.Center = npc.Center - Vector2.UnitY * 80;
            Projectile.velocity.Y = MathF.Sin(Timer * 0.1f) * 0.1f;
            if(AttackNum < 3 && Timer % 60 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Player target = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
                    if(target != null)
                    {
                        Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                            ModContent.ProjectileType<RadiantZap>(), Projectile.damage, Projectile.knockBack, Projectile.owner); ;
                    }
                
                }
                AttackNum++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;
            Vector2 stretchScale = Vector2.One;
            stretchScale.X *= 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Color.Yellow;
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White * 0.25f, Projectile.rotation - Main.GlobalTimeWrappedHourly, drawOrigin, stretchScale * 1.5f * ExtraMath.Osc(0.5f, 1f, speed: 6, offset: 3), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White * 0.25f, Projectile.rotation + Main.GlobalTimeWrappedHourly, drawOrigin, stretchScale * 1.5f * ExtraMath.Osc(0.5f, 1f, speed: 6), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 16; i++)
            {
                float progress = i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
        }
    }
}
