using System.Collections.Generic;

namespace Stellamod.Core.Particles
{
    public partial class ParticleSystem
    {
        public class ParticleComparer : IComparer<LegacyParticle>
        {
            public int Compare(LegacyParticle x, LegacyParticle y)
            {
                return x.GetShaderPath().CompareTo(y.GetShaderPath());
            }
        }
    }
}
