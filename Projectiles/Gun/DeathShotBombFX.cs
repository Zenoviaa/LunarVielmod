using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Gun
{
    public class DeathShotBombFX : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("DeathShotBomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 400;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.extraUpdates = 1;
        }
        Vector2 alphaPos;
        float alphaCounter = 4f;
        float alphaCounter2 = 0.2f;
        int counter;
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                counter = Main.rand.Next(1, 3);
            }
            Projectile.velocity *= 0.97f;
            alphaCounter2 *= 1.01f;
            if (alphaCounter >= 0)
            {
                alphaCounter -= 0.02f;
            }
            else
            {
                Projectile.active = false;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            alphaPos.X = Projectile.Center.X;
            alphaPos.Y = Projectile.Center.Y - alphaCounter2;
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            if(counter == 1)
            {
                Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.4f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.4f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture2D4, alphaPos - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.65f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            }


            return true;
        }

    }
}