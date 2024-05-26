using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class ExampleVoidProjectile : ModProjectile
    {
            public override string Texture => TextureRegistry.FlowerTexture;

            private float Timer
            {
                get => Projectile.ai[0];
                set => Projectile.ai[0] = value;
            }

            private float LifeTime => 360;
            private float MaxScale => 0.66f;
            public override void SetDefaults()
            {
                Projectile.width = 64;
                Projectile.height = 64;
                Projectile.friendly = false;
                Projectile.hostile = true;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.timeLeft = (int)LifeTime;
            }

            public override void AI()
            {
                Timer++;
                if (Timer == 1)
                {
                    //Play sound an effects and stuff
                    //Freezing sound, probably like crumbling paper or something
                }

                float progress = Timer / LifeTime;
                float easedProgress = Easing.SpikeInOutCirc(progress);
                Projectile.width = Projectile.height = (int)(64 * easedProgress);
                if (progress >= 0.75f)
                {
                    Projectile.hostile = false;
                }
            }

            public override Color? GetAlpha(Color lightColor)
            {
                return new Color(
                    Color.LightCyan.R,
                    Color.LightCyan.G,
                    Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
            }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>(TextureRegistry.CircleOutline);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            Color drawColor = (Color)GetAlpha(lightColor);
            float drawScale = Projectile.scale * 0.8f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            shader.UseOpacity(0.3f);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(1f);

            //How fast the extra texture animates
            float speed = 0.2f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(Color.RoyalBlue);

            //Texture itself
            shader.UseImage1(texture);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            float drawRotation = MathHelper.TwoPi;
            float num = 16;
            for (int i = 0; i < num; i++)
            {
                float nextDrawScale = drawScale;
                float nextDrawRotation = drawRotation * (i / num);
                spriteBatch.Draw(texture.Value, drawPosition, null, (Color)GetAlpha(lightColor), nextDrawRotation, drawOrigin, nextDrawScale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }
    }
}
