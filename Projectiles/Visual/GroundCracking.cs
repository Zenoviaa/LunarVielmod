using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Visual
{
    internal class GroundCracking : ModProjectile
    {

        private ref float Timer => ref Projectile.ai[0];
        private float Lifetime => 120;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = (int)Lifetime;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D crackTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Black.MultiplyRGB(lightColor);

            float colorProgress = Timer / Lifetime;
            drawColor = Color.Lerp(drawColor, Color.Transparent, colorProgress);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(crackTexture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D crackTexture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;

            float colorProgress = Timer / Lifetime;
            drawColor = Color.Lerp(Color.Black, Color.Transparent, colorProgress);
            float drawRotation = Projectile.rotation;
            float drawScale = 1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
    
            for (int i = 0; i < 4; i++)
            {
                float progress = Timer / 10f;
                progress = MathHelper.Clamp(progress, 0f, 1f);
                progress = 1f - progress;

                //This will be pretty cool
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(4, 4) * progress;
                spriteBatch.Draw(crackTexture, flameDrawPos, Projectile.Frame(), drawColor * 0.25f, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

        }
    }
}
