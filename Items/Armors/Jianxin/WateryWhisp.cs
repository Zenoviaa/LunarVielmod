using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Jianxin
{
    internal class WateryWhisp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 220;
            Projectile.height = 220;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        float trueFrame = 0;
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;

            bool hasSetBonus = owner.GetModPlayer<MyPlayer>().Waterwhisps;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }




            //Lighting
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(1f, 1, 96);
        }

        
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new Rectangle(0, 0, 220, 220);
            rectangle.X = ((int)trueFrame % 6) * rectangle.Width;
            rectangle.Y = (((int)trueFrame - ((int)trueFrame % 6)) / 6) * rectangle.Height;

            Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.6f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                (Color)GetAlpha(lightColor), drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
