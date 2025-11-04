using System.Collections.Generic;

namespace Stellamod.Core.Particles
{
    public partial class ParticleSystem
    {
        public class ParticleComparer : IComparer<Particle>
        {
            public int Compare(Particle x, Particle y)
            {
                return x.GetShaderPath().CompareTo(y.GetShaderPath());
            }
        }
    }
}
