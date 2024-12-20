using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Elements
{
    internal class RoyalMagicElement : BaseElement
    {
        int trailingMode = 0;

        public override int GetOppositeElementType()
        {
            return ModContent.ItemType<DreadElement>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

        }

        public override Color GetElementColor()
        {
            return ColorFunctions.RoyalMagicPink;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: ColorFunctions.RoyalMagicPink,
                primaryColor: Color.White,
                noiseColor: Color.Black);
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, ColorFunctions.RoyalMagicPink);
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

                Color color = ColorFunctions.RoyalMagicPink;
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity * 0.5f, color);
            }
        }

        #region Visuals

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.Lerp(Color.White, Color.Pink, p);
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }


        public override void DrawTrail()
        {
            base.DrawTrail();
            DrawMainShader();
            DrawOutlineShader();
        }

        private Color ColorFunction(float completionRatio)
        {
            Color c = Color.White;
            if(trailingMode == 0)
            {
                Color backColor = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.3f), Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));
                Color frontColor = Color.Lerp(Color.Pink, Color.White, VectorHelper.Osc(0f, 1f, speed: 3f));
                c = Color.Lerp(frontColor, backColor, Easing.OutExpo(completionRatio, 5f));
                return c;
            }
           

            c.R = 0;
            c.G = 0;
            c.B = 0;
            c.A = 0;
            return c;
        }

        private float WidthFunction(float completionRatio)
        {
            float width = 16 * 6f * MagicProj.ScaleMultiplier;
            completionRatio = Easing.SpikeOutCirc(completionRatio);
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

        private void DrawMainShader()
        {
            trailingMode = 0;
            var shader = MagicRoyalMagicShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DreadTrail;
            shader.NoiseTexture = TrailRegistry.Clouds3;
            shader.PrimaryColor = Color.White;
            shader.NoiseColor = Color.White;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.5f;
            shader.Distortion = 2.5f;
            shader.Alpha = 0.25f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }

        private void DrawOutlineShader()
        {
            trailingMode = 1;
            var shader = MagicRadianceOutlineShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DottedTrailOutline;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            Color pink = new Color(255, 59, 247);
            Color blue = Color.Blue;
            Color finalColor = Color.Lerp(pink, blue, VectorHelper.Osc(0f, 1f, speed: 3f));


            Color c = Color.White;
            c = finalColor;
            finalColor = Color.Lerp(finalColor, Color.Black, 0.5f);
            shader.PrimaryColor = Color.White;
            shader.NoiseColor = finalColor;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.8f;
            shader.Distortion = 0.85f;
            shader.Power = 2.5f;

            TrailDrawer.Draw(Main.spriteBatch, MagicProj.OldPos, Projectile.oldRot, ColorFunction, WidthFunction, shader, offset: Projectile.Size / 2);
        }
        #endregion
    }
}
