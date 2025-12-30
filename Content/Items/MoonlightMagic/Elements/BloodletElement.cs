using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

namespace Stellamod.Content.Items.MoonlightMagic.Elements
{
    public class BloodletElement : BaseElement
    {
        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SoundStyle hitStyle = SoundRegistry.BloodletHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;

            SoundStyle chargeSoundStyle = AssetRegistry.Sounds.MagicWand.BloodletCharge;
            chargeSoundStyle.PitchVariance = 0.15f;
            ChargeSound = chargeSoundStyle;

            SoundStyle chargeShotSoundStyle = AssetRegistry.Sounds.MagicWand.BloodletChargeShot;
            chargeShotSoundStyle.PitchVariance = 0.15f;
            CastSound = chargeShotSoundStyle;
        }

        public override void OnKill()
        {
            base.OnKill();
            SpawnDeathParticles();
        }

        private void SpawnDeathParticles()
        {
            //Kill Trail
            Vector2 vek = Projectile.oldVelocity;
            vek *= 0.1f;
            Vector2 pos = Projectile.Center;
            for (float f = 0; f < 16; f++)
            {
                Vector2 pVelocity = vek.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(pos, pVelocity);
                FXUtil.GlowFragmentParticle(pos, pVelocity,
                    innerColor: Color.Black,
                    outerColor: Color.Red,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<TSmokeDust>(),
                                     vek.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(),
                                     vek.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
                if (Main.rand.NextBool(4))
                {

                    var part = FXUtil.GlowFragmentParticle(pos, pVelocity,
                     innerColor: Color.DarkRed,
                     outerColor: Color.DarkBlue,
                     fadeToColor: Color.Black,
                     distortOut: false);
                    part.Scale *= 1.3f;

                }
            }
        }
        #region Visuals
        public override Color GetElementColor()
        {
            return ColorFunctions.DreadRed;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.DreadRed,
                primaryColor: Color.White,
                noiseColor: Color.DarkRed);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.DreadRed);
        }

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            drawPos -= Projectile.velocity * 1.5f;
            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.Lerp(Color.Red, Color.Black, p);
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);

            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.Red, drawRotation, drawOrigin, drawScale * 1.25f +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 8), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            spriteBatch.Draw(formTexture, drawPos, null, Color.Black,
   drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(formTexture, drawPos, null, Color.White * 0.3f, drawRotation, drawOrigin, drawScale +
                ExtraMath.Osc(-0.1f, 0.1f, speed: 4), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkBlue;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, flamingTrailShader);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.White, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            if (MagicProj.laserLike)
                return MagicProj.GetTrailLaserWidth(completionRatio) * 0.6f;
            float width = 18 * 1.5f * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(width, 0, EasingFunction.InOutExpo(completionRatio));
        }
        #endregion
    }
}
