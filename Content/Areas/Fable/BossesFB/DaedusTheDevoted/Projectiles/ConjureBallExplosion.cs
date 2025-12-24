using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class ConjureBallExplosion : ModProjectile
    {
        private float _lightningPower;
        private float _lightningTime;
        private bool _calculatedStrikePoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        private Vector2[] _lightningZaps;
        public CoreLightning Lightning { get; set; } = new CoreLightning();

        public Vector2[] BeamPoints;
        public float[] BeamRot;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningZaps = new Vector2[12];
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle explosionSound;
                if (Main.rand.NextBool(2))
                {
                    explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_StormSpike");
                    explosionSound.PitchVariance = 0.15f;
                }
                else
                {
                    explosionSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_StormSpike2");
                    explosionSound.PitchVariance = 0.15f;
                }

                SoundEngine.PlaySound(explosionSound, Projectile.position);
            }
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
            if (Timer == 12)
            {
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
            }

            if (Timer == 24)
            {
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
            }


            if (Timer == 28)
            {
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.Scale *= 0.5f;
                    spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                }
            }
            float explosionProgress = Timer / 60f;
            float easedExplosionProgress = Easing.OutExpo(explosionProgress);
            float widthOffset = MathHelper.Lerp(0, 80, easedExplosionProgress);
            Lightning.WidthMultiplier = Easing.SpikeOutCirc(explosionProgress);
            if (Timer % 3 == 0)
            {


            }

            if (!_calculatedStrikePoints)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity;
                float numPoints = 80;
                float randOffset = Main.rand.NextFloat(-1f, 1f);
                Vector2 start = Projectile.Center;
                Vector2 end = Projectile.Center + direction * 256;

                for (float i = 0; i <= numPoints; i++)
                {
                    float interp = i / numPoints;
                    Vector2 point = Vector2.Lerp(start, end, interp);
                    beamPoints.Add(point);
                }
                BeamPoints = beamPoints.ToArray();
                BeamRot = new float[BeamPoints.Length];
                _calculatedStrikePoints = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (BeamPoints == null)
                return false;
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningShader lightningShader = LightningShader.Instance;
            lightningShader.Time = _lightningTime;
            lightningShader.Power = _lightningPower;
            TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);
            TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);

            return false;
        }
        private float LightningWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(180, 0, completionRatio);
        }

        private Color LightningColorFunction(float completionRatio)
        {
            Color lerpColor = Color.Lerp(Color.White, Color.Blue, Timer / 30f);
            return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
        }

    }
}
