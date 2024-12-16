
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Altride
{
    internal class Radial : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 34;
        }


        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 68;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;
            DrawHelper.AnimateTopToBottom(Projectile, 2);
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.5f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
