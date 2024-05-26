using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostBlastProj : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float MaxScale => 1.5f;
        private float LifeTime => 90;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    float scale = Main.rand.NextFloat(0.3f, 0.5f);
                    ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, scale);
                }

                for(int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    float speed = 12;
                    Vector2 velocity = -Vector2.UnitY * speed;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 * 1.5f);

                    int type = ModContent.ProjectileType<NiiviIcicleProj>();
                    int damage = Projectile.damage / 2;
                    float knockback = Projectile.knockBack / 2;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        type, damage, knockback, Projectile.owner);
                }

                //Explosion sound goes here
            }

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            if (++_frameTick >= 3)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            int frameSpeed = 3;
            int frameCount = 30;
            Rectangle rect = texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false);

            Vector2 origin = rect.Size() / 2;
            //Calculate the scale with easing
            float progress = Timer / LifeTime;
            float easedProgress = Easing.SpikeInOutCirc(progress);
            float scale = easedProgress * MaxScale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, rect
            ,
                (Color)GetAlpha(lightColor), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
    }
}
