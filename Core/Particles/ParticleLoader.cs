using System.Collections.Generic;

namespace Stellamod.Core.Particles
{
    /// <summary>
    /// 基本全抄的源码
    /// </summary>
    public class ParticleLoader
    {
        public static IList<Particle> Particles;
        public static int ParticleCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取粒子
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Particle GetParticle(int type)
                 => type < ParticleCount ? Particles[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
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
