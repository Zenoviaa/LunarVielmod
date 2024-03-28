using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    public class VoidBolt : ModProjectile
    {
        private const int Body_Radius = 4;
        private const int Kill_Particle_Count = 16;
        private const int Explosion_Particle_Count = 8;

        ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }


        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            if (Timer % 8 == 0)
            {
                //Main Body
                Vector2 position = Projectile.Center +
                     new Vector2(Main.rand.Next(0, Body_Radius), Main.rand.Next(0, Body_Radius));
                float size = Main.rand.NextFloat(0.75f, 1f);
                Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), size);

                p.layer = Particle.Layer.BeforeProjectiles;
                Particle tearParticle = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<VoidTearParticle>(),
                    default(Color), size + 0.025f);

                tearParticle.layer = Particle.Layer.BeforePlayersBehindNPCs;
            }

            if(Timer % 16 == 0)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                Dust dust = Dust.NewDustPerfect(position, DustID.GemAmethyst, Scale: Main.rand.NextFloat(0.5f, 3f));
                dust.noGravity = true;
            }

            Projectile.scale = VectorHelper.Osc(0.9f, 1f, 5f);
            DrawHelper.AnimateTopToBottom(Projectile, 4);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            //REPLACE SOUND AT SOME POINT
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            for (int i = 0; i < Explosion_Particle_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1.5f, 1.5f);
                ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
                    default(Color), Main.rand.NextFloat(0.9f, 1.33f));
            }

            //Just some dusts so it looks nicer when it dies
            for (int i = 0; i < Kill_Particle_Count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }
}
