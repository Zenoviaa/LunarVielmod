using System;

namespace Stellamod.Common.Particles;

public record struct ParticleFrameData(string Texture, int FrameCount)
{
    public static ParticleFrameData Create(Type type, int frameCount)
    {
        return new ParticleFrameData($"{type.Namespace}.{type.Name}".Replace('.', '/'), frameCount);
    }
}
