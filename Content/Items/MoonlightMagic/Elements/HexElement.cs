using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.Dyes;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class HexElement : BaseElement
    {
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<WindElement>());
        }

        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<UvilisElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.HexCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.HexChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.BasicMagicHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;
        }
        public override Color GetElementColor()
        {
            return ColorFunctions.HexPurple;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.HexPurple,
                primaryColor: Color.White,
                noiseColor: Color.DarkBlue);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.HexPurple);
        }

        public override void AI()
        {
            base.AI();
            AI_Particles();
        }

        private void AI_Particles()
        {
            if (MagicProj.GlobalTimer % 8 == 0)
            {
                for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
                {
                    if (!Main.rand.NextBool(4))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -8;
                    LegacyParticle.NewParticle<SparkleWindParticle>(spawnPoint, velocity, Color.White);
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

                Color color = Color.White;
                color.A = 0;
                LegacyParticle.NewParticle<SparkleWindParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                color.A = 0;
                if (Main.rand.NextBool(2))
                    LegacyParticle.NewParticle<SparkleWindParticle>(spawnPoint, velocity.RotatedByRandom(MathHelper.TwoPi), color);
                LegacyParticle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
            }
        }

        #region Visuals
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            var shader = DyeMothlightShader.Instance;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.PrimaryColor = new Color(195, 158, 255);
            shader.NoiseColor = new Color(78, 76, 180);//new Color(78, 76, 180);
            shader.OutlineColor = Color.White;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.1f;
            shader.Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, shader.Effect, Main.GameViewMatrix.ZoomMatrix);

            //Draw The Base Form
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale * 1.5f);

            spriteBatch.End();
            spriteBatch.Begin();
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            var shader = MagicWindShader.Instance;

            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.PrimaryColor = new Color(195, 158, 255);
            shader.NoiseColor = new Color(78, 76, 180);//new Color(78, 76, 180);
            shader.OutlineColor = Color.White;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.1f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }
        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.Lerp(Color.White, Color.Transparent, completionRatio);
            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 16 * 2 * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(width, 8, completionRatio);
        }
        #endregion
    }
}
