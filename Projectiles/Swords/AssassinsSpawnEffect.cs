using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Swords
{
    public class AssassinsSpawnEffect : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune Spawn Effect");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public float Rot;
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 50;
            Projectile.height = 50;
            Projectile.width = 50;
            Projectile.extraUpdates = 1;
        }

        private float alphaCounter = 5;
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                Rot = Main.rand.NextFloat(0.1f, 0.4f);
            }
            Projectile.rotation += Rot;
            alphaCounter -= 0.18f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Extra_63").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(05f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(256, 256), 0.2f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(05f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(256, 256), 0.4f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            return true;
        }
    }
}