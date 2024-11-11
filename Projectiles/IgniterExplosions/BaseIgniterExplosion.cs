using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public abstract class BaseIgniterExplosion : ModProjectile
    {
        private int _frame;
        private float _frameCounter;
        private bool _start;
        private ref float Timer => ref Projectile.ai[0];
        public virtual int FrameCount { get; }

        public float FrameSpeed { get; set; }
        public float DrawScale { get; set; }

        public virtual bool BlackIsTransparency => true;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FrameCount;
        }

 
        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 2f;
            FrameSpeed = 1f;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            SetExplosionDefaults();
            float f = FrameCount;
            if (FrameSpeed == 0)
            {
                FrameSpeed = 1f;
            }

            float length = f * (1f / FrameSpeed);
            Projectile.timeLeft = (int)length;
        }

        public virtual void SetExplosionDefaults()
        {

        }
        public virtual void Start() { }
        public override void AI()
        {
            base.AI();
            if (!_start)
            {
                Start();
                _start = true;
            }

            AI_Animate();
        }

        private void AI_Animate()
        {
            _frameCounter += FrameSpeed;
            if (_frameCounter >= 1f)
            {
                _frameCounter = 0;
                _frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.White;
            if (BlackIsTransparency)
                drawColor.A = 0;

            Rectangle animationFrame = texture.GetFrame(_frame, FrameCount);
            spriteBatch.Draw(texture, drawPos, animationFrame, drawColor, Projectile.rotation, animationFrame.Size() / 2, DrawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
