using Microsoft.Xna.Framework;
using ReLogic.Threading;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    /// <summary>
    /// Manages particles for the black star texture
    /// </summary>
    public class BlackStarParticleManager
    {
        public struct BlackStarParticle
        {
            public Vector2 position;
            public float time;
        }


        public BlackStarParticleManager(int particleCount, float duration)
        {
            MaxParticleCount = particleCount;
            Particles = new BlackStarParticle[particleCount];
            Duration = duration;
        }

        public readonly BlackStarParticle[] Particles;
        public readonly int MaxParticleCount;
        public readonly float Duration;
        public float time;
        public void Update(Vector2 spawnBounds)
        {
            time++;
            FastParallel.For(0, MaxParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    ref BlackStarParticle particle = ref Particles[i];
                    particle.time = (time + i) % Duration;
                    if (particle.time == 1)
                    {
                        //Reinitialize the particle
                        Vector2 newPosition = new Vector2();
                        newPosition.X = Main.rand.NextFloat(0f, spawnBounds.X);
                        newPosition.Y = Main.rand.NextFloat(0f, spawnBounds.Y);
                        particle.position = newPosition;
                    }
                }
            });
        }
    }
}
