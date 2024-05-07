using ParticleLibrary;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Stellamod.Projectiles.Whips
{
    internal class ColdheartAnklebiterIcicleProj :ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            //Don't hit for a lil while
            Timer++;
            if(Timer > 5)
            {
                Projectile.friendly = true;
            }

            Projectile.velocity.Y += 0.25f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool(32))
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

            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
        }
    }
}
