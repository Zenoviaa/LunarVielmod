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
    public class NaturalElement : BaseElement
    {
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<LightningElement>());
        }
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.NatureCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.NatureChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.NatureHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.NaturalGreen;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.NaturalGreen,
                primaryColor: Color.White,
                noiseColor: Color.DarkGreen);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.NaturalGreen);
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
                    if (!Main.rand.NextBool(16))
                        continue;
                    Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                    Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                    velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                    Color color = Color.White;
                    color.A = 0;

                    if (Main.rand.NextBool(1))
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                LegacyParticle.NewBlackParticle<WhiteFlowerParticle>(spawnPoint, velocity, Color.White);
                                break;
                            case 1:
                                LegacyParticle.NewBlackParticle<PurpleFlowerParticle>(spawnPoint, velocity, Color.White);
                                break;
                            case 2:
                                LegacyParticle.NewBlackParticle<BlueFlowerParticle>(spawnPoint, velocity, Color.White);
                                break;
                        }

                    }

                    if (Main.rand.NextBool(32))
                    {
                        LegacyParticle.NewBlackParticle<MusicParticle>(spawnPoint, velocity, color);
                    }
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
                if (Main.rand.NextBool(1))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            LegacyParticle.NewBlackParticle<WhiteFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                        case 1:
                            LegacyParticle.NewBlackParticle<PurpleFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                        case 2:
                            LegacyParticle.NewBlackParticle<BlueFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                    }

                }
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                color.A = 0;
                if (Main.rand.NextBool(2))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            LegacyParticle.NewBlackParticle<WhiteFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                        case 1:
                            LegacyParticle.NewBlackParticle<PurpleFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                        case 2:
                            LegacyParticle.NewBlackParticle<BlueFlowerParticle>(spawnPoint, velocity, Color.White);
                            break;
                    }

                }
                LegacyParticle.NewBlackParticle<MusicParticle>(spawnPoint, velocity, color);
            }
        }

        #region Visuals

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            float p = MathUtil.Osc(0f, 1f, speed: 2);
            for (float f = 0; f < 1f; f += 0.25f)
            {
                float rot = f * MathHelper.TwoPi;
                float dist = 2;
                Vector2 offset = rot.ToRotationVector2() * dist;
                Vector2 outlineDrawPos = drawPos + offset;
                Color outlineDrawColor = Color.Lerp(ColorFunctions.NaturalGreen, Color.RosyBrown, p);
                base.DrawForm(spriteBatch, formTexture, outlineDrawPos, outlineDrawColor, lightColor, drawRotation, drawScale);
            }

            p = MathUtil.Osc(0f, 1f, speed: 2, offset: 3);
            drawColor = Color.Lerp(ColorFunctions.NaturalGreen, Color.RosyBrown, p);
            drawScale *= MathUtil.Osc(0.8f, 1f);
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            //Trail
            var shader = MagicNaturalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.NoiseTextureLeaves;
            shader.NoiseTexture = TrailRegistry.NoiseTextureLeaves;
            shader.ShapeTexture = TrailRegistry.DottedTrail;
            shader.BlendState = BlendState.AlphaBlend;
            shader.PrimaryColor = new Color(95, 106, 47);
            shader.NoiseColor = Color.White;
            shader.Speed = 0.5f;
            shader.Distortion = 0.1f;
            shader.Threshold = 0.1f;

            //This just applis the shader changes
            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White; ;
        }

        private float WidthFunction(float completionRatio)
        {
            int width = (int)(16 * 2.5f * MagicProj.ScaleMultiplier);
            completionRatio = EasingFunction.QuadraticBump(completionRatio);
            return MathHelper.Lerp(0, width, completionRatio);
        }
        #endregion
    }
}
