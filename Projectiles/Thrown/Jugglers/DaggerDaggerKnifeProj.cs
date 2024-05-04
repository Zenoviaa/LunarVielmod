using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown.Jugglers
{
    internal class DaggerDaggerKnifeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string glowTexture = Texture;
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(42, 46);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = VectorHelper.Osc(0f, 1f, 3);
            Color drawColor = Color.Lerp(Color.Transparent, Color.Red, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;

            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Red, Color.Transparent, ref lightColor);
            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
