using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < Main.rand.Next(1, 4); i++)
                {
                    Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                    float size = Main.rand.NextFloat(0.7f, 0.9f);
                    ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<WaterParticle>(),
                        default(Color), size);
                }
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.Center, 2, 2, ModContent.DustType<BubbleDust>());
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.SplashWeak, Projectile.position);

            int count = 16;
            for (int i = 0; i < count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<WaterParticle>(),
                    default(Color), 1 / 2f);
            }

            for (int i = 0; i < count/2; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BubbleDust>(), speed);
            }
        }
    }
}
