using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;


namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class UvilisElement : BaseElement
    {
        int trailingMode = 0;

        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<PrismaticElement>());
        }
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<HexElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.UvilisCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.UvilisChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle uvilisHitStyle = SoundRegistry.UvilisHit;
            uvilisHitStyle.PitchVariance = 0.15f;
            HitSound = uvilisHitStyle;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.UvilisLightBlue,
                primaryColor: Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f),
                noiseColor: Color.Blue);
            return true;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.UvilisLightBlue;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.UvilisLightBlue);
        }

        public override void AI()
        {
            base.AI();
            AI_Particles();
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

                if (Main.rand.NextBool(2))
                {
                    Color color = Color.White;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, color);
                }
                else
                {
                    Color color = ColorFunctions.UvilisLightBlue;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
                    LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                if (Main.rand.NextBool(2))
                {
                    Color color = ColorFunctions.UvilisLightBlue;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, color);
                }
                else
                {

                    Color color = Color.White;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.2f, color);
                    LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, new Color(255, 255, 255, 0));
                }
            }
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 8 == 0)
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(2))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                    Color color = Color.White;
                    color.A = 0;
                    if (Main.rand.NextBool(7))
                    {
                        LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, color);
                    }

                    if (Main.rand.NextBool(16))
                    {
                        LegacyParticle.NewBlackParticle<WaterSparkleParticle>(spawnPoint, velocity, color);
                    }
                }
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.Blue;
            switch (trailingMode)
            {
                default:
                case 0:
                    break;
                case 1:
                    c.A = 0;
                    break;
                case 2:
                    c.A = 0;
                    break;
            }

            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 16 * 1.3f * MagicProj.ScaleMultiplier;
            completionRatio = EasingFunction.QuadraticBump(completionRatio);
            switch (trailingMode)
            {
                default:
                case 0:
                    return MathHelper.Lerp(0, width, completionRatio);
                case 1:
                    return MathHelper.Lerp(0, width, completionRatio);
                case 2:
                    return MathHelper.Lerp(0, width + 12, completionRatio);
            }
        }


        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            var shader = PixelMagicSparkleWaterShader.Instance;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = new Color(92, 100, 255);
            shader.OutlineColor = Color.Black;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.5f;
            shader.Power = 0.5f;
            shader.Threshold = 0.25f;
            shader.Apply();

            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.Lerp(ColorFunctions.UvilisLightBlue, Color.LightGoldenrodYellow, p);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, shader.Effect, Main.GameViewMatrix.ZoomMatrix);

            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale * 2f);

            spriteBatch.End();
            spriteBatch.Begin();
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        private void DrawMainShader(Vector2[] oldPos)
        {
            trailingMode = 0;
            var shader = MagicSparkleWaterShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
            shader.NoiseColor = new Color(92, 100, 255);
            shader.OutlineColor = Color.Black;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 0.5f;
            shader.Threshold = 0.25f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);

        }

        private void DrawOutlineShader(Vector2[] oldPos)
        {
            trailingMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = new Color(38, 204, 255);
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader2(Vector2[] oldPos)
        {
            trailingMode = 2;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;

            Color c = Color.White;
            shader.PrimaryColor = c;
            shader.NoiseColor = c;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.25f;
            shader.Power = 3.5f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            DrawMainShader(oldPos);
            DrawOutlineShader(oldPos);
            DrawOutlineShader2(oldPos);
        }
    }
}
