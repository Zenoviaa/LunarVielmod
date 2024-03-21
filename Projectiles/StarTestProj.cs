using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class StarTestProj : ModProjectile
    {
        ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Timer++;

            //Cool Visuals
            //Make a wave???
            float waveSpeed = 8;
            float waveRange = 16;

            float starOffset1 = MathF.Sin(MathHelper.ToRadians(Timer * waveSpeed)) * waveRange;
            float starOffset2 = MathF.Sin(-MathHelper.ToRadians(Timer * waveSpeed)) * waveRange;

            Vector2 rotatedDirection = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
            Vector2 starPos1 = Projectile.Center + rotatedDirection * starOffset1;
            Vector2 starPos2 = Projectile.Center + rotatedDirection * starOffset2;
            if(Timer % 2 == 0)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(0.5f, 0.5f);

                //Get a random color
                Color randColor = Main.rand.NextColor(Color.White, Main.DiscoColor, Color.Black);
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(starPos1, velocity, randColor, randScale);
                ParticleManager.NewParticle<StarParticle2>(starPos2, velocity, randColor, randScale);
            }
        }
    }
}
