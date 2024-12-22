using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class TorrentialLanceBubbleProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            Timer++;
            float progress = Timer / 30;
            Projectile.scale = Easing.InExpo(progress);
            Projectile.velocity.Y -= 0.05f;
            Projectile.velocity.X *= 0.98f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() / 2;
            spriteBatch.Draw(texture, position, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
