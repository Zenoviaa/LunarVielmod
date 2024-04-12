using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Helpers;
using ParticleLibrary;
using Stellamod.Particles;

namespace Stellamod.Projectiles.Magic
{
    internal class AquariusSlashMini : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        private Vector2 _scale;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 24;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
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
            if(Timer == 1)
            {
                _scale = new Vector2(
                    Main.rand.NextFloat(0.5f, 0.8f),
                    Main.rand.NextFloat(0.5f, 0.8f));
            }

            _scale *= 1.03f;
            Projectile.alpha++;
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

            float rotation = Projectile.velocity.ToRotation();
            float width = 187;
            float height = 187;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 24;
            SpriteBatch spriteBatch = Main.spriteBatch;
 
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), rotation, origin, _scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
