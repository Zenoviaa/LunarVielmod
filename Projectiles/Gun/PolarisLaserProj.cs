using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class PolarisLaserProj : ModProjectile
    {
        //Don't change the sample points, 3 is good enough
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;
        public List<Vector2> BeamPoints;

        //No texture for this
        public override string Texture => TextureRegistry.EmptyTexture;

        ref float Size => ref Projectile.ai[0];
        float Timer;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            BeamPoints = new List<Vector2>();
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            Timer++;
            if (Timer == 1)
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast1, Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast2, Projectile.position);
                        break;
                }


                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(explosionCenter, 1024f, 32f);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero, ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                ShakeModSystem.Shake = 3;
                for (float f = 0; f < 12; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 12;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle.NewParticle<DustParticle>(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(1.3f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }

                for(float f = 0; f < 12; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    SmokeParticle smokeParticle = Particle.NewBlackParticle<SmokeParticle>(explosionCenter + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(1.3f, 3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }

                float numZaps = 4;
                for(float f = 0; f < numZaps; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);
                    ZapParticle zapParticle = Particle.NewParticle<ZapParticle>(explosionCenter + initialVelocity, Main.rand.NextVector2Circular(1, 1), Color.White);
                }


                FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(explosionCenter,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (float f = 0; f < 12f; f++)
                {
                    float progress = f / 12f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;

                }
                var sear = Particle.NewParticle<SearParticle>(explosionCenter, Vector2.Zero);
                sear.innerColor = Color.Cyan;
                sear.outerColor = Color.Blue;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * Size;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);

            float width = (float)Projectile.timeLeft / 45f;
            return (Projectile.width * Projectile.scale) * osc * width * Size * 5;
        }
        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 0.5f;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }
        public Color ColorFunction2(float completionRatio)
        {
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
      
        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i <= 8; i++)
            {
                Vector2 start = Projectile.Center;
                BeamPoints.Add(Vector2.Lerp(start, start + direction * BeamLength, i / 8f));
            }


            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Blue;
            shader.BlendState = BlendState.AlphaBlend;
            shader.LaserTexture = TrailRegistry.StarTrail;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints.ToArray(), ColorFunction, WidthFunction, shader);

            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints.ToArray(), ColorFunction2, WidthFunction2, shader);
        }
    }
}
