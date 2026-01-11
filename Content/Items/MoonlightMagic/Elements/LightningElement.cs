using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class LightningElement : BaseElement
    {
        private float _randAmplitude;
        private float _randFrequency;
        private float _randOffset;
        private float _flashTimer;
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<NaturalElement>());
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle castStyle = SoundID.DD2_LightningAuraZap;
            castStyle.PitchVariance = 0.15f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundID.DD2_LightningBugZap;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.BasicCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;
        }

        public override Color GetElementColor()
        {
            return new Color(120, 215, 255);
        }

        public override void DustEffects()
        {
            base.DustEffects();
            _flashTimer--;
            _randOffset *= 0.94f;
            if (MagicProj.GlobalTimer % 24 == 0 || MagicProj.GlobalTimer == 2)
            {
                _randOffset = Main.rand.NextFloat(16f, 0f);
                SoundStyle castStyle = SoundID.DD2_LightningAuraZap;
                castStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(castStyle, Projectile.position);

                _flashTimer = 15;
            }
            if (MagicProj.orb)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spawnCenter = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                    Vector2 spawnVelocity = -Projectile.velocity.RotatedByRandom(0.5).SafeNormalize(Vector2.Zero) * 8 * Main.rand.NextFloat(0.5f, 1f);


                    LightningSparkParticle sparkParticle = Particle<LightningSparkParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), 
                        color: Color.Goldenrod, Scale: Main.rand.NextFloat(0.3f, 0.5f));
                    //    sparkParticle.parent = Projectile;
                    sparkParticle.gravity = 0f;
                    sparkParticle.dampening = 0.05f;
                    sparkParticle.fast = true;
                    sparkParticle.parent = Projectile;
                }
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 spawnCenter = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 spawnVelocity = -Projectile.velocity.RotatedByRandom(0.5).SafeNormalize(Vector2.Zero) * 8 * Main.rand.NextFloat(0.5f, 1f);

                DustParticle dp2 = DustParticle.Spawn(spawnCenter, spawnVelocity);
                dp2.innerColor = Color.Goldenrod;
                dp2.outerColor = Color.Turquoise;
                //     dp2.parent = Projectile;
                dp2.Scale *= 1.25f;
                dp2.gravity = 0.1f;
                dp2.dampening = Main.rand.NextFloat(0.05f, 0.2f);
                dp2.fast = true;

                // smokeParticle.parent = Projectile;

            }
            if (Main.rand.NextBool(8))
            {
                Vector2 spawnCenter = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 spawnVelocity = -Projectile.velocity.RotatedByRandom(0.5).SafeNormalize(Vector2.Zero) * 8 * Main.rand.NextFloat(0.5f, 1f);

                SmokeParticle smokeParticle = SmokeParticle.SpawnInAlphaLayer(spawnCenter, spawnVelocity * 0.2f, Color.White, Main.rand.NextFloat(1.2f, 2f));
                smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.8f);
                smokeParticle.fast = true;

            }

            if (Main.rand.NextBool(32))
            {
                Vector2 spawnCenter = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 spawnVelocity = -Projectile.velocity.RotatedByRandom(0.5).SafeNormalize(Vector2.Zero) * 8 * Main.rand.NextFloat(0.5f, 1f);

                SmokeParticle smokeParticle2 = SmokeParticle.SpawnInAlphaLayer(spawnCenter, spawnVelocity * 0.2f, Color.White, Main.rand.NextFloat(1.2f, 2f));
                smokeParticle2.initialColor = Color.Lerp(Color.White, Color.Black, 0.8f);
                smokeParticle2.parent = Projectile;
                smokeParticle2.fast = true;
            }
            if (MagicProj.GlobalTimer % 8 == 0)
            {
                FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                dp.innerColor = Color.Goldenrod;
                dp.outerColor = Color.Turquoise;
                dp.parent = Projectile;
                dp.gravity = 0f;
                dp.dampening = 0.05f;
                dp.fast = true;

                LightningSparkParticle sparkParticle = Particle<LightningSparkParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), color: Color.Goldenrod, Scale: Main.rand.NextFloat(0.13f, 0.25f));
                //    sparkParticle.parent = Projectile;
                sparkParticle.gravity = 0f;
                sparkParticle.dampening = 0.05f;
                sparkParticle.fast = true;
            }
        }

        public override void OnKill()
        {
            base.OnKill();

            for(float n = 0; n < 8; n++)
            {
                Vector2 backVelocity = -Projectile.oldVelocity.RotateRandom(0.5f);
                backVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var dp = DustParticle.Spawn(Projectile.Center, backVelocity);
                dp.outerColor = Color.Goldenrod;
            }
            var part = FXUtil.GlowCircleBoom(Projectile.Center + Projectile.oldVelocity,
                innerColor: Color.White,
                glowColor: Color.Goldenrod,
                outerGlowColor: Color.Turquoise, duration: 12, baseSize: 0.14f);
        }


        #region Visuals
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);

            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Goldenrod;
            glowColor = Color.Lerp(Color.Goldenrod, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;

            float rotation = Projectile.velocity.ToRotation();
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, rotation, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.2f * MagicProj.ScaleMultiplier * new Vector2(2f, 1f), SpriteEffects.None, 0);
            for(int i = 1; i < MagicProj.OldPos.Length; i++)
            {
                if (i % 2 != 0)
                    continue;
                Vector2 oldPosition = MagicProj.OldPos[i];
                Vector2 oldDrawPosition = oldPosition - Main.screenPosition;
                float rot = (oldPosition - MagicProj.OldPos[i - 1]).ToRotation();
                Color afterImageGlowColor = Color.Goldenrod;
                afterImageGlowColor = Color.Lerp(Color.Goldenrod, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 8));
    
                float ratio = (float)i / (float)MagicProj.OldPos.Length;
                afterImageGlowColor = Color.Lerp(afterImageGlowColor, Color.Black, MathHelper.SmoothStep(0.6f, 1f, ratio));
                afterImageGlowColor.A = 0;

                float scale = MathHelper.SmoothStep(1f, 0f, ratio);
                spriteBatch.Draw(glowMask, oldDrawPosition, null, afterImageGlowColor, rot, glowDrawOrigin, 
                    scale * Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f * MagicProj.ScaleMultiplier * new Vector2(2f, 0.6f), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(formTexture, drawPos, null, Color.Black, drawRotation, formTexture.Size() / 2f, drawScale * 1.25f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);



            if (MagicProj.orb)
            {
                glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
                glowDrawOrigin = glowMask.Size() / 2f;
                glowColor = Color.Lerp(Color.Goldenrod, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 8));
                glowColor.A = 0;
                spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);
                // spriteBatch.RestartDefaults();


                glowMask = AssetManager.GlowMask.SpiralVortex.Value;
                glowDrawOrigin = glowMask.Size() / 2f;
                glowColor = Color.Goldenrod;
                glowColor.A = 0;
                spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            }
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            if (MagicProj.GlobalTimer % 3 == 0)
            {
                _randFrequency = Main.rand.NextFloat(-3f, 3f);
                _randAmplitude = Main.rand.NextFloat(0.5f, 1f);

            }
            //Apply offsets to create a more jagged motion
            Vector2[] lightningPoints = new Vector2[oldPos.Length];
            for (int i = 0; i < oldPos.Length; i++)
            {

                Vector2 trailPoint = oldPos[i];
                if (i > 1 && i < oldPos.Length - 4)
                {
                    float ratio = (float)i / (float)oldPos.Length;

                    Vector2 oldTrailPoint = oldPos[i - 1];
                    Vector2 velocity = (trailPoint - oldTrailPoint).SafeNormalize(Vector2.Zero);
                    Vector2 upVector = velocity.RotatedBy(MathHelper.PiOver2);

                    float frequency = 16;
                    frequency += _randFrequency;

                    //Applying a random offset here will make it jump from left to right sometimes
                    // frequency += Main.rand.NextFloat(-2f, 2);
                    float amplitude = MathHelper.SmoothStep(32, 16, ratio);
                    amplitude *= _randAmplitude;
                    Vector2 lightningVelocity = upVector * MathF.Sin(ratio * frequency + _randOffset) * amplitude;
                    lightningPoints[i] = trailPoint + lightningVelocity;
                }
                else
                {
                    lightningPoints[i] = trailPoint;
                }

            }


            var shader = RichLaserShader.Instance;

            Color laserColor = Color.Lerp(Color.White, Color.Goldenrod, 0.5f);
            Color innerColor = Color.Goldenrod;
            Color outerColor = Color.Lerp(Color.Turquoise, Color.Turquoise, 0.5f);

            float flashRatio = _flashTimer / 15f;
            float flashLerp = 1f - flashRatio;
            flashLerp = EasingFunction.InExpo(flashLerp);


            laserColor = Color.Lerp(laserColor, Color.Turquoise, flashLerp);
            laserColor = Color.Lerp(laserColor, Color.Black, flashLerp);
            innerColor = Color.Lerp(innerColor, Color.Black, flashLerp);
            outerColor = Color.Lerp(outerColor, Color.Black, flashLerp);

            shader.LaserColor = laserColor;
            shader.InnerColor = innerColor;
            shader.OuterColor = outerColor;


            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.Tiling = new Vector2(1f, 0.5f);
            TrailDrawer.Draw(Main.spriteBatch, lightningPoints, ColorFunction, WidthFunction, shader);

            if (_flashTimer >= 12)
            {
                TrailDrawer.Draw(Main.spriteBatch, lightningPoints, ColorFunction, WidthFunction, shader);


            }
        }

        private float WidthFunction(float completionRatio)
        {
            float baseWidth = 32;
            if (MagicProj.laserLike)
                baseWidth = MagicProj.GetTrailLaserWidth(completionRatio);
            return MathHelper.SmoothStep(baseWidth, baseWidth * 0.15f, completionRatio);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
            trailColor = Color.Lerp(trailColor, Color.Goldenrod, ExtraMath.Osc(0f, 1f, speed: 16));
            trailColor = Color.Lerp(trailColor, Color.Black, EasingFunction.QuadraticBump(p));
            return trailColor;
        }
        #endregion
    }
}
