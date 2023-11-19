
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Altride
{
    internal class Radial : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 34;
        }
        

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 68;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = false;
            DrawOriginOffsetY = 0;
        }

        public override bool PreAI()
        {

            Projectile.tileCollide = false;

            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(130, 130, 130, 0) * (1f - Projectile.alpha / 50f);
        }


        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.velocity *= 0.93f;
            if (Projectile.ai[1] == 1)
            {

               

                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
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


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 34)
                {
                    Projectile.frame = 0;
                }
            }

        }

        public override void OnKill(int timeLeft)
        {
       
           
            for (int i = 0; i < 14; i++)
            {
                ParticleManager.NewParticle(Projectile.Center, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), ParticleManager.NewInstance<FabledParticle5>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
            }

           

            var entitySource = Projectile.GetSource_FromThis();

        }

      

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}
