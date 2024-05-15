using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Test
{
    internal class ExampleSilhouetteProjectile : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var shader = ShaderRegistry.MiscSilPixelShader;
            float progress = 1f + MathF.Sin(Timer * 0.1f);

            //The color to lerp to
            shader.UseColor(Color.White);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            shader.UseSaturation(progress);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 
                Projectile.rotation,
                Projectile.Frame().Size() / 2, 
                Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }
    }
}
