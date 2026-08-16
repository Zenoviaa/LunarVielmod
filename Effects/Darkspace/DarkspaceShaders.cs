using Stellamod.Common.Shaders;

namespace Stellamod.Effects.Darkspace;

public class SilkStrandShader : CrystalShader<SilkStrandShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["bloomColor"].SetValue(value.ToVector3());
        }
    }
}
