using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AquariusSlash : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 24;
        }

        public override void SetDefaults()
        {
            Projectile.width = 310;
            Projectile.height = 310;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 24)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            Timer++;
            if(Timer == 6)
            {

                for(int i = 0; i < Main.rand.Next(4, 9); i++)
                {
                    Vector2 velocity = Projectile.velocity.RotateRandom(MathHelper.PiOver4 / 1.2f) * Main.rand.NextFloat(0.3f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity.SafeNormalize(Vector2.Zero) * 80, velocity,
                        ModContent.ProjectileType<AquariusWaterBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            //Makes velocity not move the projectile
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                float scale = Main.rand.NextFloat(0.2f, 0.4f);
                ParticleManager.NewParticle(target.Center, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4 - (MathHelper.PiOver4 / 2);
            float width = 187;
            float height = 187;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 24;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), rotation, origin, 1.8f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
