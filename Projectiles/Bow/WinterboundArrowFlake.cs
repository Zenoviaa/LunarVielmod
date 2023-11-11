using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class WinterboundArrowFlake : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace Cloud");
        }

        public override void SetDefaults()
        {

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 210;
            Projectile.tileCollide = false;
            Projectile.height = 35;
            Projectile.width = 35;
            Projectile.penetrate = 10;
            Projectile.alpha = 60;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            Projectile.alpha++;

            float num = 1f - Projectile.alpha / 255f;
            Projectile.velocity *= .98f;
            num *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 1.25f * Main.essScale);
            return true;
        }
        float alphaCounter;
        public override void AI()
        {
            alphaCounter += 0.04f;
            Projectile.rotation += 0.01f;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.DeepSkyBlue with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
            }

            return true;
        }
    }
}
