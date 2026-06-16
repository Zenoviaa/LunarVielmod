using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Simulates particles in a spiral fashion while slowly moving to the center point
/// </summary>
public sealed class VortexParticleSystem
{
    public struct VortexParticles
    {
        public VortexParticles(int maxParticles)
        {
            positions = new Vector2[maxParticles];
            velocities = new Vector2[maxParticles];
            active = new bool[maxParticles];
        }

        public int Length => positions.Length;
        public Vector2[] positions;
        public Vector2[] velocities;
        public bool[] active;
    }

    public VortexParticleSystem(int maxParticles)
    {
        particles = new VortexParticles(maxParticles);
    }

    public VortexParticles particles;
    public Vector2 centerPoint;
    public void Update()
    {

        int extraUpdates = 2;
        for (int i = 0; i < particles.Length; i++)
        {
            ref bool active = ref particles.active[i];
            if (!active)
                continue;

            for(int j = 0; j < extraUpdates; j++)
            {
                ref Vector2 pos = ref particles.positions[i];
                ref Vector2 vel = ref particles.velocities[i];

                pos = pos.RotatedBy(ExtraMath.Osc(0f, 0.01f, speed: 4, offset: i), centerPoint);
                //      vel += (centerPoint - pos).SafeNormalize(Vector2.Zero) * 0.3f;
                vel = Vector2.Lerp(vel, (centerPoint - pos).SafeNormalize(Vector2.Zero) * 4, 0.012f);
                pos += vel;
                float dist = Vector2.Distance(pos, centerPoint);
                if (dist < 16)
                {
                    active = false;
                }
            }

        }
    }

    public void SpawnParticle(Vector2 position, Vector2 initialVelocity)
    {
        int particleIndex = -1;
        for (int i = 0; i < particles.Length; i++)
        {
            ref bool active = ref particles.active[i];
            if (active)
                continue;

            particleIndex = i;
            active = true;
            break;
        }

        if (particleIndex == -1)
            return;

        ref Vector2 pos = ref particles.positions[particleIndex];
        pos = position;
      
        ref Vector2 velocity = ref particles.velocities[particleIndex];
        velocity = initialVelocity;
    }
}
