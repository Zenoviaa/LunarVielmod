using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class YourFiredProj : ModProjectile
    {
        private PrimitiveTrail TrailDrawer;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                //Effects
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }


            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.78f * MathF.Sin(Timer * 0.5f));
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return (Projectile.width * Projectile.scale * 1f - completionRatio) * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawScale = 1f;
            float drawRotation = Projectile.rotation;

            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;
            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            TrailDrawer.SpecialShader.UseColor(Color.DarkGoldenrod);
            TrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);


            TrailDrawer.Draw(Projectile.oldPos, -Main.screenPosition + drawOrigin, 32);


            SpriteBatch spriteBatch = Main.spriteBatch;


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
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
            shader.UseColor(new Color(Color.OrangeRed.R, Color.OrangeRed.G, Color.OrangeRed.B, 0));

            //Texture itself
            shader.UseImage1(TrailRegistry.WaterTrail);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            spriteBatch.Draw(texture, drawPosition, null, Color.White, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin();

   

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballDeath"), Projectile.position);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
            float num = 8;
            float maxDelay = 30;
            for (int i = 0; i < num; i++)
            {
                float clusterRadius = 256;
                float progress = i / (float)num;
                float delay = progress * maxDelay;
                Vector2 randPosition = Projectile.Center + Main.rand.NextVector2Circular(clusterRadius, clusterRadius);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), randPosition, Vector2.Zero,
                    ModContent.ProjectileType<YourFiredExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: delay);
            }
        }
    }
}
