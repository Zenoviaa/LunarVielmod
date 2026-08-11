using Stellamod.Common.Shaders;

namespace Stellamod.Core.Utilities;

public class OutlineShader : CrystalShader<OutlineShader>
{
    private EffectParameter _texelSizeParam;
    public Vector2 TexelSize
    {
        set
        {
            _texelSizeParam ??= Effect.Parameters["texelSize"];
            _texelSizeParam.SetValue(value);
        }
    }

}

public class WhiteOutlineShader : CrystalShader<WhiteOutlineShader>
{
    private EffectParameter _texelSizeParam;
    public Vector2 TexelSize
    {
        set
        {
            _texelSizeParam ??= Effect.Parameters["texelSize"];
            _texelSizeParam.SetValue(value);
        }
    }

}
