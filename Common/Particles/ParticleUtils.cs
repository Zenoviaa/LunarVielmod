using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod.Common.Particles;

/// <summary>
/// A collection of utilities for spawning particles
/// </summary>
public static partial class ParticleUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateSmokeCircle(in Vector2 initialPosition, in Color smokeinitialColor, in int particleCount, in float radius)
    {
        for (var f = 0; f < particleCount; f++)
        {
            var pos = initialPosition + Main.rand.NextVector2Circular(radius, radius);
            var vel = (pos - initialPosition).Resize(12) * Main.rand.NextFloat(0.5f, 1.5f);
            var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(pos, vel, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            smoke.initialColor = smokeinitialColor;
            smoke.dampening = 0.15f;
            smoke.Scale *= 2.5f;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateSmokeCircleBig(in Vector2 initialPosition, in Color smokeinitialColor, in int particleCount, in float radius)
    {
        for (var f = 0; f < particleCount; f++)
        {
            var pos = initialPosition + Main.rand.NextVector2Circular(radius, radius);
            var vel = (pos - initialPosition).Resize(12) * Main.rand.NextFloat(0.5f, 1.5f);
            var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(pos, vel, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            smoke.initialColor = smokeinitialColor;
            smoke.dampening = 0.15f;
            smoke.Scale *= 3.5f;
        }
    }
}
