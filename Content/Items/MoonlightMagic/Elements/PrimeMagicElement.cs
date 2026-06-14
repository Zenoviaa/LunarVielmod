using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;


namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class PrimeMagicElement : BaseElement
    {
        private ZappingTrail _lightningTrail;
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<DeeyaElement>());
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle castStyle = SoundRegistry.PrimeMagicCast;
            castStyle.PitchVariance = 0.25f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundRegistry.PrimeMagicHit;
            hitStyle.PitchVariance = 0.25f;
            HitSound = hitStyle;

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.BasicCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;
        }

        public override Color GetElementColor()
        {
            return Color.Orange;
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
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.Orange);
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if(_lightningTrail == null)
            {
                _lightningTrail ??= new();
                _lightningTrail.RandomPositions(MagicProj.OldPos);
            }
            if (Main.rand.NextBool(8))
            {
                _lightningTrail ??= new();
                _lightningTrail.RandomPositions(MagicProj.OldPos);
            }

            if (Main.rand.NextBool(8))
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(8))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                    Color color = Color.Orange;
                    if (Main.rand.NextBool(2))
                        color = Color.Purple;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color, Scale: 0.5f);
                }
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
                velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                Color color = Color.Orange;
                if (Main.rand.NextBool(2))
                    color = Color.Purple;
                color.A = 0;
                LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.Orange;
                if (Main.rand.NextBool(2))
                    color = Color.Purple;
                color.A = 0;
                LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
            }
        }

        #region Visuals
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            var shader = PixelMagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.CloudsSmall;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = new Color(69, 70, 159);
            shader.NoiseColor = new Color(224, 107, 10);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0f;
            shader.Power = 3f;
            shader.Blend = 0.4f;
            shader.Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, shader.Effect, Main.GameViewMatrix.ZoomMatrix);

            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicVaellusShader.Instance;
            shader.PrimaryTexture = TrailRegistry.LightningTrail2;
            shader.NoiseTexture = TrailRegistry.LightningTrail3;
            shader.OutlineTexture = TrailRegistry.LightningTrail2Outline;
            shader.PrimaryColor = new Color(69, 70, 159);
            shader.NoiseColor = Color.Lerp(Color.LightBlue, Color.Purple, 0.5f);
            shader.OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;
            shader.Alpha = 1f;

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 1;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 2;
            _lightningTrail.Draw(spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }

        private float WidthFunction(float completionRatio)
        {
            float baseWidth = 32;
            if (MagicProj.laserLike)
            {
                baseWidth = MagicProj.GetTrailLaserWidth(completionRatio) * 0.7f;
                return baseWidth;
            }
            float progress = completionRatio / 0.3f;
            float rounded = EasingFunction.QuadraticBump(progress);
            float spikeProgress = EasingFunction.QuadraticBump(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, EasingFunction.OutExpo(1.0f - completionRatio));
            float midWidth = baseWidth * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.BlueViolet, Color.Purple, p);
            return trailColor;
        }
        #endregion
    }
}
