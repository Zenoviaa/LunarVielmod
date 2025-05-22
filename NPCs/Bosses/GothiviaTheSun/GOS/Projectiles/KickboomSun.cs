using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    internal class KickboomSun : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.localNPCHitCooldown = 6;
            Projectile.usesLocalNPCImmunity = true;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = npc.Center;

            //Lighting
            Vector3 RGB = new(2.89f, 2.53f, 2.0f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(1f, 1, 50);
        }

        
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new Rectangle(0, 0, 518, 518);
            rectangle.X = ((int)trueFrame % 5) * rectangle.Width;
            rectangle.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * rectangle.Height;

            Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = Projectile.rotation;
            float drawScale = 2f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                (Color)GetAlpha(lightColor), drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
