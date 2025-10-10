using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun

{
    public class VenShotIN : ModProjectile
    {
        private float DrawScale;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Timer++;
            float progress = Timer / 60f;
            float easedProgress = Easing.InOutCubic(progress);
            DrawScale = MathHelper.Lerp(0.75f, 0f, easedProgress) * VectorHelper.Osc(0.5f, 1f, offset: Projectile.whoAmI);

            Projectile.rotation -= 0.03f;
            Vector3 RGB = new(1.59f, 0.23f, 1.91f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 30)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle drawFrame = Projectile.Frame();
            Vector2 drawOrigin = drawFrame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = DrawScale;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, drawPos, drawFrame, drawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            return false;
        }
    }
}