using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class BerserkLaser : VSProjectile
    {
        private Asset<Texture2D> _sparkTexture;
        private Vector2[] _laserPoints;

        private Vector2[] _dragLaserPoints;
        private float _inTimer;
        private float _growTimer;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.penetrate = -1;
            Projectile.timeLeft = 1020;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Vector2 zapPosition = Projectile.Center + Projectile.velocity;
            if (_dragLaserPoints == null)
                _dragLaserPoints = new Vector2[64];
            for (int i = _dragLaserPoints.Length - 1; i > 0; i--)
            {
                _dragLaserPoints[i] = _dragLaserPoints[i - 1];
            }   
            _dragLaserPoints[0] = zapPosition;


            _inTimer++;
            if (_inTimer == 1)
            {
                SoundStyle laserSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER");
                SoundEngine.PlaySound(laserSound, Projectile.position);
            }
            Timer++;
            if(Timer % 3 == 0)
            {
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                velocity *= Main.rand.NextFloat(20, 35);
                var zap = FXUtil.GlowStretch(Projectile.Center, velocity);
            }
            if(Timer % 5 == 0)
            {
                Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 5);
                var spark = LegacyParticle.NewParticle<SparkParticle>(zapPosition + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 5;

            }
            ShakeScreenPosition.Shake = 6;
            if (Timer % 5 == 0)
            {
                var zap = LegacyParticle.NewParticle<ZapParticle>(zapPosition + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
            }

            NPC parentNpc = GetParentNPC();
            Projectile.Center = parentNpc.Center;


            float rotatedRadians = MathHelper.ToRadians(1.5f);
     
            Projectile.velocity = Projectile.velocity.RotatedBy(rotatedRadians);
            if (Timer >= 120)
            {
                _growTimer++;
                Projectile.velocity = Projectile.velocity.RotatedBy(rotatedRadians * 0.5f);
                if (Timer % 3 == 0)
                {
                    Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                    velocity *= Main.rand.NextFloat(20, 35);
                    var zap = FXUtil.GlowStretch(Projectile.Center, velocity);
                }
            }


            Projectile.rotation += Main.rand.NextFloat(0f, 0.1f);

            List<Vector2> laserPoints = new List<Vector2>();
            float numPoints = 128;
            for (float n = 0; n < numPoints; n++)
            {
                float interpolant = n / numPoints;
                Vector2 velocity = Projectile.velocity;
                Vector2 laserPoint = Vector2.Lerp(Projectile.Center, Projectile.Center + velocity, interpolant);
                if (Main.rand.NextBool(128))
                {
                    LegacyParticle.NewParticle<ZapParticle>(laserPoint + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(8, 8));
                }
            //    laserPoint = laserPoint.RotatedBy(MathHelper.Lerp(1f, 0f, interpolant), GetParentNPC().Center);
                laserPoints.Add(laserPoint);
            }
            _laserPoints = laserPoints.ToArray();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(_laserPoints, projHitbox, targetHitbox);
        }

        private float WidthFunction(float interpolant)
        {
            float inScale = EasingFunction.InOutSine(_inTimer / 30f);
            float outScale = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
            float width = 70;
            float groScale = MathHelper.Lerp(1f, 2f, EasingFunction.InOutSine(_growTimer / 30f));
            return width * inScale * outScale * Main.rand.NextFloat(0.95f, 1f) * EasingFunction.QuadraticBump(interpolant) * groScale;
        }

        private Color ColorFunction(float interpolant)
        {
            Color color = Color.Lerp(Color.Black, Color.White, interpolant);
            Color blue = Color.Lerp(color, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 8));
            if(Timer > 120)
            {
                Color purple = Color.Lerp(Color.Yellow, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 8));
                color = color.MultiplyRGB(purple);
            }
            color = color.MultiplyRGB(blue);
            return color;
        }


        private Color ColorFunction2(float interpolant)
        {
            return Color.Black;
        }
        private float WidthFunction2(float interpolant)
        {
            return WidthFunction(interpolant) * 0.3f;
        }
        private float WidthFunction3(float completionRatio)
        {
            float w = 128;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (_laserPoints == null)
                return false;
            if (_dragLaserPoints == null)
                return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.BulbTrail;
            shader.NoiseTexture = TrailRegistry.WaterTrail;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = -4;
            shader.Repeats = 1f;
            TrailDrawer.Draw(Main.spriteBatch, _laserPoints, ColorFunction, WidthFunction, shader);

            shader.BlendState = BlendState.AlphaBlend;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            TrailDrawer.Draw(Main.spriteBatch, _laserPoints, ColorFunction2, WidthFunction2, shader);


  

            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            TrailDrawer.Draw(Main.spriteBatch, _dragLaserPoints, ColorFunction, WidthFunction3, shader);
            TrailDrawer.Draw(Main.spriteBatch, _dragLaserPoints, ColorFunction, WidthFunction3, shader);

            Vector2 drawPosition = _laserPoints[_laserPoints.Length - 1] - Main.screenPosition;
            Vector2 drawScale = Vector2.One;

            //Edge of laser draw like sparks thing
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.Lerp(Color.Yellow, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 32));
            sparkyShader.OuterColor = Color.Blue;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);

            _sparkTexture ??= ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect);
            Vector2 lightDrawOrigin = _sparkTexture.Size() / 2f;
            float inScale = EasingFunction.InOutSine(_inTimer / 30f);
            float sparkyRot = Projectile.rotation;
            float scaleOsc2 = ExtraMath.Osc(1f, 1.05f, speed: 8);
            scaleOsc2 *= Main.rand.NextFloat(0.75f, 1f);
            scaleOsc2 *= inScale;
            spriteBatch.Draw(_sparkTexture.Value, drawPosition, null, Color.White * 1f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(_sparkTexture.Value, drawPosition, null, Color.White * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);
            return false;
        }
    }
}
