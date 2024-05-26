using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostTelegraphProj : ModProjectile
    {
        public override string Texture => TextureRegistry.CircleOutline;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LifeTime => 60;
        private float MaxScale = 1.5f;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
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
            float easedProgress = Easing.InOutCubic(1 - progress);
            float scale = easedProgress * MaxScale;

            //This should make it fade in and then out
            float alpha = Easing.SpikeInOutCirc(progress);
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor = drawColor * alpha;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);

            //I think that one texture will work
            //The vortex looking one
            //And make it spin
            return false;
        }
    }
}
