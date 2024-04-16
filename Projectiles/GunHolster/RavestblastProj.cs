
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class RavestblastProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 16;
        }


        public override void SetDefaults()
        {
            Projectile.width = 448;
            Projectile.height = 225;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 255;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 0.1f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            DrawOriginOffsetY = 0;
        }
        
        public override bool PreAI()
        {

            Projectile.tileCollide = false;
            if (++_frameTick >= 2)
            {
                _frameTick = 0;
                if (++_frameCounter >= 16)
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


        public override void AI()
        {

            Projectile.scale *= 1.02f;
            Projectile.ai[1]++;
            Projectile.velocity *= 1.02f;
            if (Projectile.ai[1] == 1)
            {



                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CoralTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }

            }

            if (Projectile.ai[1] > 1)
            {
                Projectile.alpha++;
            }


         

        }
        private int _frameCounter;
        private int _frameTick;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 448;
            float height = 225;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 2;
            int frameCount = 16;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor),Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


        public override void OnKill(int timeLeft)
        {


            for (int i = 0; i < 14; i++)
            {
               // ParticleManager.NewParticle(Projectile.Center, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), ParticleManager.NewInstance<Rainbow>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
            }



            var entitySource = Projectile.GetSource_FromThis();

        }



        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CoralTorch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}
