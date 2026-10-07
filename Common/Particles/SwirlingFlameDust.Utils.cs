using Stellamod.Assets.ContentReader.Pal;
using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod.Common.Particles;

public static partial class ParticleUtils
{
    public record struct ParticleFactory(Vector2 start, Vector2 end, TriColorPalette palette, Vector2 scaleRange, Vector2 speedRange, int particleCount)
    {
        public static ParticleFactory FromSmallBurst(Vector2 start, Vector2 end, TriColorPalette palette, Vector2 speedRange)
        {
            return new ParticleFactory(start, end, palette, new Vector2(0.5f, 1.2f), speedRange, 12);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateSwirlingDustBurst(ParticleFactory factory)
    {
        for (var i = 0; i < factory.particleCount; i++)
        {
            var vel = (factory.end - factory.start);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel = vel.RotatedByRandom(0.3f);
            vel *= Main.rand.NextFloat(factory.speedRange.X, factory.speedRange.Y);
            var pos = factory.start;
            pos += Main.rand.NextVector2Circular(16, 16);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = factory.palette.primaryColor.ToVector4(),
                outerColor = factory.palette.secondaryColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(factory.scaleRange.X, factory.scaleRange.Y)),
                timeLeft = Main.rand.Next(30, 55)
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateSwirlingDustCircle(ParticleFactory factory)
    {
        for (var i = 0; i < factory.particleCount; i++)
        {
            var vel = (factory.end - factory.start);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel = vel.RotatedByRandom(6.28f);
            vel *= Main.rand.NextFloat(factory.speedRange.X, factory.speedRange.Y);
            var pos = factory.start;
            pos += Main.rand.NextVector2Circular(32, 32);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = factory.palette.primaryColor.ToVector4(),
                outerColor = factory.palette.secondaryColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(factory.scaleRange.X, factory.scaleRange.Y)),
                timeLeft = Main.rand.Next(90, 135)
            });
        }
    }
}