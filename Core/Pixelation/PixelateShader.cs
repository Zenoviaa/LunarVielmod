using Stellamod.Common.Shaders;

namespace Stellamod.Core.Pixelation;

public class PixelateShader : CrystalShader<PixelateShader>
{
    private EffectParameter _widthParam;
    private EffectParameter _heightParam;
    public float Width
    {
        set
        {
            _widthParam ??= Effect.Parameters["width"];
            _widthParam.SetValue(value);
        }
    }

    public float Height
    {
        set
        {
            _heightParam ??= Effect.Parameters["height"];
            _heightParam.SetValue(value);
        }
    }
}
