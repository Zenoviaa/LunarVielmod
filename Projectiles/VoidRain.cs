using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class VoidRain : ModProjectile
    {
        private const int Dust_Rate = 6;
        private const int Particle_Rate = 2;
        private const int Particle_Radius = 2;
        private const int Death_Particle_Explosion_Count = 16;
        private const float Death_Particle_Explosion_Radius = 2;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            ref float ai_Counter = ref Projectile.ai[0];
            ai_Counter++;
            if (ai_Counter % 20 == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
                Projectile.netUpdate = true;
            }

            Projectile.velocity.Y += 0.1f;
            Visuals();
        }

        private void Visuals()
        {
            ref float visuals_Counter = ref Projectile.ai[1];
            ref float dust_Counter = ref Projectile.ai[2];
            visuals_Counter++;
            dust_Counter++;

            if (visuals_Counter >= Particle_Rate)
            {
                visuals_Counter = 0;
                Vector2 position = Projectile.Center + new Vector2(Main.rand.Next(0, Particle_Radius), Main.rand.Next(0, Particle_Radius));
                float size = Main.rand.NextFloat(1/4f, 1/3f);
                ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(), default(Color), size);
            }
            
            if(dust_Counter >= Dust_Rate)
            {
                dust_Counter = 0;
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Particle_Radius / 2, Particle_Radius / 2);
                float size = Main.rand.NextFloat(1/4f, 1/3f);
                Dust dust = Dust.NewDustPerfect(position, DustID.GemAmethyst, Scale: size);
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Death_Particle_Explosion_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(Death_Particle_Explosion_Radius, Death_Particle_Explosion_Radius);
                Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), 1 / 3f);
                p.layer = Particle.Layer.BeforeProjectiles;
            }
        }
    }
}
