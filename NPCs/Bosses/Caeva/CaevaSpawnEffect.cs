using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.Caeva
{
    public class CaevaSpawnEffect : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune Spawn Effect");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 50;
            Projectile.height = 90;
            Projectile.width = 90;
            Projectile.extraUpdates = 1;
        }


        float alphaCounter = 5;
        public override void AI()
        {
            alphaCounter -= 0.18f;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Extra_56").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(05f * alphaCounter), (int)(65f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(171, 51), 0.4f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(05f * alphaCounter), (int)(65f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(171, 51), 0.6f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            return true;
        }

    }
}