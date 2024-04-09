using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class IlluriaStarGlow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.GetModPlayer<MyPlayer>().HasAlcaliteSet)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = player.Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texture = "Stellamod/Effects/Masks/ZuiEffect";
            Texture2D maskTexture = ModContent.Request<Texture2D>(texture).Value;

            Vector2 textureSize = new Vector2(143, 143);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = VectorHelper.Osc(0, 1);
            float alpha = VectorHelper.Osc(0, 1);
            Color color = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, progress);
            Color drawColor = Color.Multiply(new(color.R, color.G, color.B, 0), alpha);
            Vector2 drawPosition = Projectile.position - Main.screenPosition;
            Main.spriteBatch.Draw(maskTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return base.PreDraw(ref lightColor);
        }
    }
}
