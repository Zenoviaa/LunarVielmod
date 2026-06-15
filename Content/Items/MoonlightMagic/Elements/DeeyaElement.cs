using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
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
    public class DeeyaElement : BaseElement
    {
        int trailingMode = 0;


        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<PrimeMagicElement>());
        }
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<BloodletElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.DeeyaCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.DeeyaChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.DeeyaHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.DeeyaPink;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.DeeyaPink,
                primaryColor: Color.White,
                noiseColor: Color.Black);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.DeeyaPink);
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(8))
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(4))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                    Color color = Color.White;
                    color.A = 0;
                    LegacyParticle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
                }
            }
        }
        private void AI_Particles()
        {

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

                Color color = Color.White;
                color.A = 0;
                LegacyParticle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = ColorFunctions.DeeyaPink;
                color.A = 0;
                LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.5f, color);
            }
        }

        #region Visuals

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.Lerp(Color.White, Color.Pink, p);
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }


        public override void DrawTrail(Vector2[] oldPos)
        {
            DrawMainShader(oldPos);
            DrawOutlineShader(oldPos);
        }

        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.White;
            c.R = 0;
            c.G = 0;
            c.B = 0;
            c.A = 0;
            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float baseWidth = 16f;
            if (MagicProj.laserLike)
                baseWidth = MagicProj.GetTrailLaserWidth(completionRatio);
            float width = baseWidth * 2f * MagicProj.ScaleMultiplier;
            completionRatio = EasingFunction.QuadraticBump(completionRatio);
            switch (trailingMode)
            {
                default:
                case 0:
                    return MathHelper.Lerp(0, width, completionRatio);
                case 1:
                    return MathHelper.Lerp(0, width / 3f, completionRatio);
                case 2:
                    return MathHelper.Lerp(0, width / 2f, completionRatio);
            }
        }

        private void DrawMainShader(Vector2[] oldPos)
        {
            trailingMode = 0;
            var shader = MagicDreadShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DreadTrail;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.PrimaryColor = Color.Black;
            shader.NoiseColor = Color.Black;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.5f;
            shader.Distortion = 2.5f;
            shader.Alpha = 0.25f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }

        private void DrawOutlineShader(Vector2[] oldPos)
        {
            trailingMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            Color pink = new Color(255, 59, 247);
            Color c = Color.White;
            c = pink;
            pink = Color.Lerp(pink, Color.Black, 0.5f);
            shader.PrimaryColor = Color.White;
            shader.NoiseColor = pink;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.85f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader);
        }
        #endregion
    }
}
