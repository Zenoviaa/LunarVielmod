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
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

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

                    Color color = Color.White;
                    color.A = 0;
                    Particle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
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
                Particle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                color.A = 0;
                Particle.NewBlackParticle<BloodSparkleParticle>(spawnPoint, velocity, color);
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
            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.Lerp(Color.Red, Color.Black, p);
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        public override void DrawTrail(Vector2[] oldPos)
        {
            var shader = MagicDreadShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DreadTrail;
            shader.NoiseTexture = TextureRegistry.Clouds6;
            shader.PrimaryColor = new Color(255, 51, 51);
            shader.NoiseColor = Color.Lerp(shader.PrimaryColor, Color.Black, 0.5f);
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 10.5f;
            shader.Distortion = 0.1f;
            shader.Alpha = 0.25f;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.Red;
            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 24 * 1.5f * MagicProj.ScaleMultiplier;
            return MathHelper.Lerp(width, 0, completionRatio);
        }
        #endregion
    }
}
