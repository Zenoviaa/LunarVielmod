using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public class BaseHitEffect : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 192;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 1f;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] <= 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Particle.NewParticle<GlowParticle>(spawnPoint, velocity, Color.White);




                }

                for (int i = 0; i < 1; i++)
                {
                    Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                    Vector2 velocity = Main.rand.NextVector2Circular(0, 1);
                    Particle.NewParticle<StrikeParticle>(spawnPoint, velocity, Color.White);
                    Particle.NewParticle<Strike2Particle>(spawnPoint, velocity, Color.White);
                }

                Projectile.rotation = Main.rand.Next(0, 360);
            }


            Projectile.frameCounter++;


            if (++_frameTick >= 3)
            {
                _frameTick = 0;
                if (++_frameCounter >= 7)
                {
                    Projectile.active = false;
                    _frameCounter = 0;
                }
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 192 * 2;
            float height = 192 * 2;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 3;
            int frameCount = 7;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                Color.White, Projectile.rotation, origin, 0.3f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.ShimmerSplash, 0, 60, 133);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(8, 8);
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Particle.NewParticle<GlowParticle>(spawnPoint, velocity, Color.White);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}