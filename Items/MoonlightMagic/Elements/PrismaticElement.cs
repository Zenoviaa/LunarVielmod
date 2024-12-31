using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Elements
{
    internal class PrismaticElement : BaseElement
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            /*
            SoundStyle castStyle = SoundID.Item28;
            castStyle.PitchVariance = 0.15f;
            CastSound = castStyle;

            SoundStyle hitStyle = SoundRegistry.PrismaticHit;
            hitStyle.PitchVariance = 0.15f;
            HitSound = hitStyle;*/
        }

        public override Color GetElementColor()
        {
            return Main.DiscoColor;
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: Main.DiscoColor,
                primaryColor: Color.White,
                noiseColor: Color.Lerp(Main.DiscoColor, Color.Black, 0.3f));
            return true;
        }

        public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Main.DiscoColor);
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
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                Vector2 spawnPoint = Projectile.position;
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(0f, 4f);

                Color color = Color.White;
                color.A = 0;
                Particle.NewBlackParticle<GlowParticle>(spawnPoint, velocity, color);
            }
        }

        #region Visuals

        public override void DrawForm(SpriteBatch spriteBatch, Texture2D formTexture, Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            float p = MathUtil.Osc(0f, 1f, speed: 3);
            drawColor = Color.White;
            base.DrawForm(spriteBatch, formTexture, drawPos, drawColor, lightColor, drawRotation, drawScale);
        }

        public override void DrawTrail()
        {
            base.DrawTrail();
            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(4f);
            TrailDrawer.DrawWithMiscShader(Main.spriteBatch,
                MagicProj.OldPos, Projectile.oldRot, StripColors, StripWidth, miscShaderData, offset: -Main.screenPosition + Projectile.Size / 2);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color value = Main.hslToRgb((progressOnStrip * 1.6f - Main.GlobalTimeWrappedHourly) % 1f, 1f, 0.5f);
            Color result = Color.Lerp(Color.White, value, Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A = 0;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 32f, num) * MagicProj.ScaleMultiplier; ;
        }

        #endregion
    }
}
