using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Elements
{
    internal class MothlightElement : BaseElement
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

        }

        public override Color GetElementColor()
        {
            return new Color(27, 157, 241);
        }

        public override bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            base.DrawTextShader(spriteBatch, item, line, ref yOffset);
            EnchantmentDrawHelper.DrawTextShader(spriteBatch, item, line, ref yOffset,
                glowColor: new Color(27, 157, 241),
                primaryColor: Color.White,
                noiseColor: Color.Lerp(new Color(27, 157, 241), Color.Black, 0.3f));
            return true;
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
            MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(2f);
            TrailDrawer.DrawWithMiscShader(Main.spriteBatch,
                MagicProj.OldPos, Projectile.oldRot, StripColors, StripWidth, miscShaderData, offset: -Main.screenPosition + Projectile.Size / 2);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.Violet, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 2;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            return MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true) * MagicProj.ScaleMultiplier;
        }

        #endregion
    }
}
