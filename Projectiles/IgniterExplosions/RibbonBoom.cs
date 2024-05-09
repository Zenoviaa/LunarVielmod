using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    internal class RibbonBoom : ModProjectile
    {
        private float _scale;
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.width = 119;
            Projectile.height = 116;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                _scale = 2f + Main.rand.NextFloat(0.75f, 1f);
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                        ModContent.GoreType<RibbonRed>());
                }

                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                {
                    Vector2 velocity = Vector2.Zero;
                    velocity.X = Main.rand.NextFloat(-16, 16);
                    velocity.Y = Main.rand.NextFloat(-10, -20);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<RibbonStaffStreamerProj>(), Projectile.damage / 10, Projectile.knockBack / 10, Projectile.owner);
                }

                for (int i = 0; i < 8; i++)
                {
                    //Get a random velocity
                    Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                    //Get a random
                    float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                    ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Color.DarkGoldenrod, randScale);
                }
            }

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }


        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 129;
            float height = 129;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, _scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
