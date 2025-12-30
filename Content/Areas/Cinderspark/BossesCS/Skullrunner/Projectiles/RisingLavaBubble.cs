using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Skullrunner.Projectiles
{
    public class RisingLavaBubble : ModProjectile
    {
        private enum AIState
        {
            Rising,
            Exploding
        }
        private Vector2 _scale;
        private Vector2 _oscScale;
        private float _explosionScalar = 1f;
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private float ExplodeWarning;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
            ProjectileSets.ResistedByFlamecrestShield[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();
            if(Timer == 1)
            {
                SoundStyle lavaSpawnSound = SoundID.Splash;
                SoundEngine.PlaySound(lavaSpawnSound, Projectile.position);
            }
            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Lava);
            }

            _scale = Vector2.Lerp(_scale, Vector2.One * _explosionScalar, 0.1f);
            _oscScale = Vector2.Lerp(Vector2.One, Vector2.One * 0.8f, ExtraMath.Osc(0f, 1f));
            switch (State)
            {
                case AIState.Rising:
                    AI_Rising();
                    break;
                case AIState.Exploding:
                    AI_Exploding();
                    break;
            }
            DrawHelper.AnimateTopToBottom(Projectile, 8);
            Lighting.AddLight(Projectile.position, Color.OrangeRed.ToVector3() * 0.78f);
        }

        private void SwitchState(AIState state)
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }

        private void AI_Rising()
        {
            Timer++;
            _explosionScalar = 1f;
            float riseTime = 30f;
            float risingVelocityY = MathHelper.Lerp(-16, 0, EasingFunction.InOutSine(Timer / riseTime));
            Projectile.velocity.Y = risingVelocityY;
            if (Timer >= riseTime + 30f)
            {
                SwitchState(AIState.Exploding);
            }
        }

        private void AI_Exploding()
        {
            Timer++;
            float explodingTime = 30f;
            ExplodeWarning = Timer / explodingTime;
            if(Timer >= 13)
            {
                _explosionScalar = 1.5f;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 scale = _scale * _oscScale;
            Color outlineColor = State == AIState.Exploding ?
                Color.Lerp(Color.Transparent, Color.Yellow, ExplodeWarning) :
                Color.Transparent;
            this.Outline(outlineColor, ref lightColor, scale);

      
            this.DrawCentered(ref lightColor, scale);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(float f = 0; f < 8; f++)
            {
                float interp = f / 8f;
                float rot = interp * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * 8;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<LavaPop>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }


    }

    public class LavaPop : ScarletProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileSets.ResistedByFlamecrestShield[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 8;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.light = 0.3f;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                Vector2 velocity = Projectile.velocity;
                Vector2 position = Projectile.Center;
                for (float f = 0; f < 16; f++)
                {
                    Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                    var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                    FXUtil.GlowFragmentParticle(position, pVelocity,
                        innerColor: Color.Red,
                        outerColor: Color.Orange,
                        fadeToColor: Color.Purple,
                        distortOut: true);

                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                         velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                    }
                    if (Main.rand.NextBool(4))
                    {

                        var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                         innerColor: Color.DarkRed,
                         outerColor: Color.DarkBlue,
                         fadeToColor: Color.Black,
                         distortOut: false);
                        part.Scale *= 1.3f;
                    }
                }
            }

            if (Timer % 8 == 0)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 pVelocity = Main.rand.NextVector2Circular(4, 4);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Red,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Purple,
                    distortOut: true);
                frag.Scale *= 0.5f;

                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            TrailDrawer.Draw(spriteBatch, OldCenterPos, OldCenterRot, ColorFunction, WidthFunction, flamingTrailShader, Vector2.Zero);
            this.Outline(Color.Red, ref lightColor);
            this.DrawCentered(ref lightColor);
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Red, completionRatio) * 0.7f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Vector2 position = Projectile.Center;
            Vector2 velocity = Projectile.oldVelocity;
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.Red,
                    outerColor: Color.Orange,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     velocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(position, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;
                }
            }
        }

    }
}
