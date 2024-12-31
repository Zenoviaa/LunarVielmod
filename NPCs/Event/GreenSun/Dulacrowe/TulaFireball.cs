
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Event.GreenSun.Dulacrowe
{
    internal class TulacroweFireball : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private PrimitiveTrail TrailDrawer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.light = 0.3f;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.ForestGreen.R,
                Color.ForestGreen.G,
                Color.ForestGreen.B, 0);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return (Projectile.width * Projectile.scale * 1f - completionRatio) * (1f - Easing.OutCubic(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var textureAsset = TextureRegistry.CloudTexture;

       
            float drawScale = 0.1f;
            float drawRotation = Projectile.rotation;

            Vector2 drawSize = textureAsset.Value.Size();
            Vector2 drawOrigin = drawSize / 2;
            Color drawColor = (Color)GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + new Vector2(16, 16);

            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            TrailDrawer.SpecialShader.UseColor(Color.SpringGreen);
            TrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);
            TrailDrawer.Draw(Projectile.oldPos, -Main.screenPosition + new Vector2(16, 16), 32);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(textureAsset.Value, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            
            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.75f;
            shader.UseOpacity(opacity);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(0.15f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(Color.ForestGreen.R, Color.ForestGreen.G, Color.ForestGreen.B, 0));

            //Texture itself
            shader.UseImage1(TrailRegistry.WaterTrail);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);


            for (int i = 0; i < 4; i++)
            {
                float ds = drawScale * (i / 4f);
                float dr = Projectile.rotation * (i / 4f);
                spriteBatch.Draw(textureAsset.Value, drawPosition, null, (Color)GetAlpha(lightColor), dr, drawOrigin, ds, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(textureAsset.Value, drawPosition, null, (Color)GetAlpha(lightColor), drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, speed * 17, Scale: 5f);
                d.noGravity = true;
            }
        }
    }
}
