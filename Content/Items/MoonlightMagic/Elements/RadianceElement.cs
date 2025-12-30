using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class RadianceElement : BaseElement
    {
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<HolinessElement>());
        }
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<PhantasmalElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.FireCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.FireChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.RadianceHit1;
            hitStyle.PitchVariance = 0.25f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.RadianceYellow;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: Color.OrangeRed,
                primaryColor: Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f),
                noiseColor: new Color(206, 101, 0));
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RadianceYellow);
        }

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.LowDetailShadersToggle)
            {
                DrawHighDetailForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
            }
            else
            {
                DrawLowDetailForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
            }
        }

        private void DrawLowDetailForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }
        public MoonSparkleShader SparkleShader;
        private void DrawHighDetailForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            //   drawPos -= Projectile.velocity * 1.5f;
            drawScale *= 1.3f;
            SparkleShader ??= new MoonSparkleShader();
            SparkleShader.ApplyToEffect();
            spriteBatch.Restart(effect: SparkleShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White, drawRotation, drawOrigin, drawScale * 1.15f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


            spriteBatch.Draw(formTexture, drawPos, null, Color.Lerp(Color.Black, Color.Red, MathUtil.Osc(0f, 1f, speed: 12)),
               drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White * 0.3f, drawRotation, drawOrigin, drawScale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 4), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(8))
            {
                int oldPosIndex = Main.rand.Next(0, MagicProj.OldPos.Length - 1);
                float lerpValue = (float)oldPosIndex / (float)MagicProj.OldPos.Length;
                float scaleFactor = MathHelper.Lerp(1.0f, 0.8f, lerpValue);

                Vector2 spawnPoint = MagicProj.OldPos[oldPosIndex] + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[oldPosIndex + 1] - MagicProj.OldPos[oldPosIndex];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -4;

                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                offset *= scaleFactor;
                spawnPoint += offset;

                scaleFactor *= Main.rand.NextFloat(0.5f, 0.8f);

                Color color = Color.RosyBrown;
                LegacyParticle.NewParticle<FireSmokeParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier * scaleFactor);
            }
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            base.DrawTrail(oldPos);
            DrawMainShader(oldPos);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, time: 360);
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            SpawnDeathParticles();
        }

        private void SpawnDeathParticles()
        {
            //Kill Trail
            for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -1;
                if (Main.rand.NextBool(4))
                {
                    if (Main.rand.NextBool(2))
                    {
                        Color color = Color.RosyBrown;
                        color.A = 0;
                        LegacyParticle.NewBlackParticle<FireSmokeParticle>(spawnPoint, velocity, color);
                    }
                    else
                    {
                        Color color = ColorFunctions.RadianceYellow;
                        color.A = 0;
                        LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
                        LegacyParticle.NewBlackParticle<FireHeatParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                    }
                }
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 2f);

                if (Main.rand.NextBool(4))
                {
                    if (Main.rand.NextBool(2))
                    {
                        Color color = Color.RosyBrown;
                        color.A = 0;
                        LegacyParticle.NewParticle<FireSmokeParticle>(spawnPoint, velocity, color);
                    }
                    else
                    {

                        Color color = ColorFunctions.RadianceYellow;
                        if (Main.rand.NextBool(2))
                            color = Color.OrangeRed;

                        LegacyParticle.NewParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
                        LegacyParticle.NewParticle<FireHeatParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                    }
                }
     
            }

            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.Yellow,
                glowColor: Color.Red,
                outerGlowColor: Color.DarkRed, duration: 25, baseSize: boomSize);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Yellow,
               glowColor: Color.Red,
               outerGlowColor: Color.DarkRed, duration: 15, baseSize: boomSize * 2);
        }

        private float WidthFunction(float completionRatio)
        {
            if (MagicProj.laserLike)
            {
                return MagicProj.GetTrailLaserWidth(completionRatio);
            }
            float width = 128 * MagicProj.ScaleMultiplier;
            return MathHelper.SmoothStep(width, 0, completionRatio);
        }

        private Color ColorFunction(float completionRatio)
        {
            Color tipColor = Color.Lerp(Color.Goldenrod, Color.DarkRed, completionRatio);
            Color finalColor = Color.Lerp(Color.Red, tipColor, EasingFunction.QuadraticBump(MathF.Pow(completionRatio, 0.5f)));
            Color finalColor2 = Color.Lerp(Color.White, finalColor, EasingFunction.QuadraticBump(completionRatio));
            return finalColor2;
        }
        public float SmokeWidthFunction(float completionRatio)
        {
            if (MagicProj.laserLike)
            {
                return MagicProj.GetTrailLaserWidth(completionRatio) * 1.5f;
            }
            float w = 250;
            float ew = w / 10;
            float width = w * MagicProj.ScaleMultiplier;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w * MagicProj.ScaleMultiplier, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public Color SmokeColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.InOutSine(completionRatio));
        }
        private void DrawMainShader(Vector2[] oldPos)
        {
            BlackFireSmokeShader blackSmokeShader = BlackFireSmokeShader.Instance;
            TrailDrawer.Draw(Main.spriteBatch, oldPos, null, SmokeColorFunction, SmokeWidthFunction, blackSmokeShader, Vector2.Zero);

            BlackFireShader blackFireShader = BlackFireShader.Instance;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, null, ColorFunction, WidthFunction, blackFireShader, Vector2.Zero);

        }

        private void DrawOutlineShader(Vector2[] oldPos)
        {

        }
    }
}
