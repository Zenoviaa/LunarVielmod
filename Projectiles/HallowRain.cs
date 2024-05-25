using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class HallowRain : ModProjectile
    {
        private const int Dust_Rate = 6;
        private const int Particle_Rate = 3;
        private const int Particle_Radius = 2;
        private const int Death_Particle_Explosion_Count = 16;
        private const float Death_Particle_Explosion_Radius = 2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

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
                Projectile.velocity.X += Main.rand.NextFloat(-2.5f, 2.5f);
                Projectile.netUpdate = true;
            }

            Projectile.velocity.Y += 0.3f;
            Visuals();
        }

        //Trails
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            return base.PreDraw(ref lightColor);
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
                float size = Main.rand.NextFloat(0.5f, 0.75f);
                Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<SparkleTrailParticle>(), default(Color), size);
                p.timeLeft = 20;
                p.layer = Particle.Layer.BeforeProjectiles;
            }

            if (dust_Counter >= Dust_Rate)
            {
                dust_Counter = 0;
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Particle_Radius / 2, Particle_Radius / 2);
                float size = Main.rand.NextFloat(1 / 4f, 1 / 3f);
                Dust dust = Dust.NewDustPerfect(position, DustID.HallowedWeapons, newColor: Color.White, Scale: size);
                dust.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Death_Particle_Explosion_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(Death_Particle_Explosion_Radius, Death_Particle_Explosion_Radius);
                float size = Main.rand.NextFloat(0.75f, 1f);
                Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<SparkleTrailParticle>(),
                    default(Color), size);
                p.timeLeft = 20;
                p.layer = Particle.Layer.BeforeProjectiles;
            }
        }
    }
}
