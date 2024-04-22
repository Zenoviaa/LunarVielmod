using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviIcicleProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
            }

            if (Main.rand.NextBool(30))
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, 0.5f);
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, scale);
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                }
        
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }
}
