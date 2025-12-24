using System.Collections.Generic;

namespace Stellamod.Core.Particles
{
    public class ParticleLoader
    {
        public static IList<LegacyParticle> Particles;
        public static int ParticleCount { get; private set; } = 0;

        public static LegacyParticle GetParticle(int type)
        {
            return type < ParticleCount ? Particles[type] : null;
        }
                
        public static int ReserveParticleID() => ParticleCount++;

        public static void Unload()
        {
            foreach (var item in Particles)
            {
                item.Unload();
            }

            Particles.Clear();
            Particles = null;
            ParticleCount = 0;
        }
    }
}
