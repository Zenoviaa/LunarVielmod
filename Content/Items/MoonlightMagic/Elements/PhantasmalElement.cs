using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class PhantasmalElement : BaseElement
    {
        public override void ModifySisters(List<int> sisters)
        {
            base.ModifySisters(sisters);
            sisters.Add(ModContent.ItemType<MothlightElement>());
        }

        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<RadianceElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.PhantasmalCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.PhantasmalChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;

            SoundStyle hitStyle = SoundRegistry.BasicMagicHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;
        }

        public override Color GetElementColor()
        {
            return ColorFunctions.PhantasmalGreen;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.PhantasmalGreen,
                primaryColor: Color.White,
                noiseColor: Color.DarkGreen);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.PhantasmalGreen);
        }

        public override void DustEffects()
        {
            base.DustEffects();
            if (Main.rand.NextBool(8))
            {
                int oldPosIndex = Main.rand.Next(0, MagicProj.OldPos.Length - 1);
                float lerpValue = (float)oldPosIndex / (float)MagicProj.OldPos.Length;
                float scaleFactor = MathHelper.Lerp(1.0f, 0f, lerpValue);

                Vector2 spawnPoint = MagicProj.OldPos[oldPosIndex] + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[oldPosIndex + 1] - MagicProj.OldPos[oldPosIndex];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -8;

                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                offset *= scaleFactor;
                spawnPoint += offset;

                Color color = Color.Lerp(Color.White, Color.Turquoise, 0.5f);
                //  color.A = 0;
                LegacyParticle.NewParticle<GlowParticle>(spawnPoint, velocity, color, Scale: MagicProj.ScaleMultiplier * scaleFactor);
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
            //Create particles all along the trail so it smooths out better
            for (int i = 0; i < MagicProj.OldPos.Length - 1; i++)
            {
                if (!Main.rand.NextBool(2))
                    continue;
                Vector2 offset = Main.rand.NextVector2Circular(16, 16);
                Vector2 spawnPoint = MagicProj.OldPos[i] + offset + Projectile.Size / 2;
                Vector2 velocity = MagicProj.OldPos[i + 1] - MagicProj.OldPos[i];
                velocity = velocity.SafeNormalize(Vector2.Zero) * -2;

                Color color = Color.Lerp(Color.White, Color.Turquoise, 0.5f);
                DustParticle dp = Particle<DustParticle>.Spawn(spawnPoint, velocity, Color.White, Scale: Main.rand.NextFloat(0.3f, 2f));
                dp.outerColor = Color.Green;
                dp.gravity = 0.05f;
            }

          
            //Create a backwards flash of particles from its death point
            for(int i = 0; i < 3; i++)
            {
                Vector2 inverseVelocity = -Projectile.oldVelocity;
                inverseVelocity = inverseVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                inverseVelocity *= Main.rand.NextFloat(0.5f, 1f) * 0.2f;
                SparkleParticle dp = Particle<SparkleParticle>.Spawn(Projectile.Center, inverseVelocity, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.outerColor = Color.Green;
            }

            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.LightGreen,
                glowColor: Color.Turquoise,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: boomSize);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.LightGreen,
               glowColor: Color.Turquoise,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: boomSize * 2);
        }

        #region Visuals
        public MoonSparkleShader SparkleShader;
        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            drawPos -= Projectile.velocity * 2f;

            Color glowColor = Color.White;
            glowColor.A = 0;
            spriteBatch.Draw(formTexture, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale * 1.25f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 16), SpriteEffects.None, 0);

            spriteBatch.Draw(formTexture, drawPos, null, Color.Black,
               drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            glowColor *= 0.3f;
            spriteBatch.Draw(formTexture, drawPos, null, glowColor, drawRotation, drawOrigin, drawScale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 4), SpriteEffects.None, 0);

            void DrawPixelatedZuiGlow(SpriteBatch spriteBatch, Vector2 screenPos)
            {

                Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
                Vector2 glowDrawOrigin = impactTexture.Size() / 2f;
                Vector2 glowDrawCenter = drawPos;
                drawColor = Color.Green;
                drawColor.A = 0;

                Vector2 glowScale = Vector2.One;
                glowScale.Y *= 0.5f;
                spriteBatch.Draw(impactTexture.Value, glowDrawCenter, null, drawColor, Projectile.velocity.ToRotation(), glowDrawOrigin, glowScale, SpriteEffects.None, 0);

                drawColor = Color.White;
                drawColor.A = 0;
                spriteBatch.Draw(impactTexture.Value, glowDrawCenter, null, drawColor, Projectile.velocity.ToRotation(), glowDrawOrigin, glowScale * 0.7f, SpriteEffects.None, 0);


            }
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedZuiGlow, DrawLayer.OverNPCsWithOutline);
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            var shader2 = RichLaserShader.Instance;
            shader2.LaserColor = Color.White;
            shader2.InnerColor = Color.Turquoise * 0.5f;
            shader2.OuterColor = Color.Blue;
            TrailDrawer.Draw(Main.spriteBatch, oldPos, ColorFunction, WidthFunction, shader2);
        }

        private Color ColorFunction(float completionRatio)
        {
            if(MagicProj.laserLike)
                return Color.Lerp( Color.SpringGreen, Color.White, EasingFunction.InExpo(completionRatio));
            return Color.Lerp(Color.White, Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            if (MagicProj.laserLike)
                return MagicProj.GetTrailLaserWidth(completionRatio) * 0.75f;

            float width = 52;
            return MathHelper.SmoothStep(width, 0f, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        #endregion
    }
}
