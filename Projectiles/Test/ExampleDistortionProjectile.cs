using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Stellamod.Projectiles.Test
{
    internal class ExampleDistortionProjectile : ModProjectile
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

            var shader = ShaderRegistry.MiscDistortionShader;

            //How fast the distortion texture scrolls
            Vector2 scroll = new Vector2(Timer * 0.005f, Timer * 0.005f);
            shader.Shader.Parameters["scroll"].SetValue(scroll);
            shader.UseImage1(ModContent.Request<Texture2D>(TextureRegistry.NormalNoise1));

            //How much the texture distorts
            float blend = 0.25f;
            shader.Shader.Parameters["uProgress"].SetValue(blend);

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
