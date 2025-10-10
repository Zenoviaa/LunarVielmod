using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public abstract class DogmaSentenceProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.001f;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.98f;
            Projectile.rotation = VectorHelper.Osc(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2);
            Projectile.scale += 0.01f;

            if (Projectile.scale >= 1f)
            {
                Projectile.scale = 1f;
            }
            if (Projectile.timeLeft < 60)
            {
                float timeLeft = (float)Projectile.timeLeft;
                float progress = timeLeft / 60f;
                Projectile.scale *= progress;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 position = Projectile.position - Main.screenPosition;
            Main.EntitySpriteDraw(texture, position, null, Color.White.MultiplyRGBA(lightColor),
                Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    public class DogmaSentence1 : DogmaSentenceProj { }
    public class DogmaSentence2 : DogmaSentenceProj { }
    public class DogmaSentence3 : DogmaSentenceProj { }
}
