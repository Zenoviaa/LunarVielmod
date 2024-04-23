using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostFlowerProj : ModProjectile
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
            if(Timer == 1)
            {
                //Play sound an effects and stuff
                //Freezing sound, probably like crumbling paper or something
            }

            float progress = Timer / LifeTime;
            float easedProgress = Easing.SpikeCirc(progress);
            Projectile.width = Projectile.height = (int)(64 * easedProgress);
            if(progress >= 0.75f)
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
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            float progress = Timer / LifeTime;
            float easedProgress = Easing.SpikeCirc(progress);
            float scale = easedProgress * MaxScale;

            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for(int i = 0; i < 4; i++)
            {
                float rotOffset = MathHelper.TwoPi * ((float)i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * ((float)i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
        
            return false;
        }
    }
}
