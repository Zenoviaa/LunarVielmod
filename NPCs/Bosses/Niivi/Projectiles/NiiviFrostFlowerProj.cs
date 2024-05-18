using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostFlowerProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LifeTime => 360;
        private float MaxScale => 0.3f;
        private float Scale;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4/2) * Main.rand.NextFloat(0.5f, 1f), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }
            }
            if(Timer > LifeTime - 60)
            {
                Projectile.hostile = false;
                Scale = MathHelper.Lerp(Scale, 0f, 0.1f);
            }
            else
            {
                Scale = MathHelper.Lerp(Scale, 1f, 0.1f);
            }
            Projectile.velocity *= 0.96f;
            Projectile.rotation += 0.03f + Projectile.velocity.Length() * 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var textureAsset = ModContent.Request<Texture2D>("Stellamod/Particles/AuroranSlashParticle");

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = textureAsset.Size();
            Vector2 drawOrigin = drawSize / 2;
            Color drawColor = new Color(255, 255, 255, 0);
            //Calculate the scale with easing
            float drawScale = Projectile.scale * MaxScale * Scale;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);



            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            shader.UseOpacity(1f * Scale);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(1f);

            //How fast the extra texture animates
            float speed = 0.2f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(Color.White);

            //Texture itself
            shader.UseImage1(textureAsset);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            float drawRotation = MathHelper.TwoPi;
            float num = 8;
            for (int i = 0; i < num; i++)
            {
                float nextDrawScale = drawScale;
                float nextDrawRotation = Projectile.rotation + drawRotation * (i / num);
                spriteBatch.Draw(textureAsset.Value, drawPosition, null, drawColor, nextDrawRotation, drawOrigin, nextDrawScale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();
            spriteBatch.Begin();

            return false;
        }
    }
}
