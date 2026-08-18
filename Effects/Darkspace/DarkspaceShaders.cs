using Stellamod.Common.Shaders;
using Terraria;

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

    public Texture2D SilkTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }
}
