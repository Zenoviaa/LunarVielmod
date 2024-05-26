
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostBreathProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private string FrostTexture => "Stellamod/Effects/Masks/ZuiEffect";
        private float LifeTime = 90;
        private float MaxScale = 1f;

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.99f;
            Projectile.rotation += 0.05f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Chance to freeze the ground with an icey flower!

            //Return false to not kill itself
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Chilled, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frozen, 60);
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R, 
                Color.LightCyan.G, 
                Color.LightCyan.B, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureRegistry.CloudTexture.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutCubic(progress);
            float scale = easedProgress * MaxScale;

            //This should make it fade in and then out
            float alpha = Easing.SpikeInOutCirc(progress);
            alpha += 0.05f;
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor = drawColor * alpha;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            float opacityProgress = Easing.SpikeInOutCirc(progress);

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.5f;
            shader.UseOpacity(opacityProgress * opacity);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(1f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(123, 255, 231, 0));

            //Texture itself
            shader.UseImage1(TextureRegistry.CloudTexture);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            for (int i = 0; i < 4; i++)
            {
                float drawScale = scale * (i / 4f);
                float drawRotation = Projectile.rotation * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, (Color)GetAlpha(lightColor), drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
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
