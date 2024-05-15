using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class ExampleShaderOrbProjectile : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.78f;
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation += 0.05f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.Red.R,
                Color.Red.G,
                Color.Red.B, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/ZuiEffect");

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            Color drawColor = (Color)GetAlpha(lightColor);
            float drawScale = Projectile.scale * 4f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 1f;
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
            //I think that one texture will work
            //The vortex looking one
            //And make it spin
            return false;
        }
    }
}
