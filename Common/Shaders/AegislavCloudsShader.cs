namespace Stellamod.Common.Shaders;

public class AegislavCloudsShader : CrystalShader<AegislavCloudsShader>
{
    private EffectParameter _xStretchParam;
    private EffectParameter _parallaxParam;
    private EffectParameter _timeParam;
    private EffectParameter _texelSizeParam;
    public Vector2 TexelSize
    {
        set
        {
            _texelSizeParam ??= Effect.Parameters["texelSize"];
            _texelSizeParam.SetValue(value);
        }
    }
    public float XStretch
    {
        set
        {
            _xStretchParam ??= Effect.Parameters["xStretch"];
            _xStretchParam.SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public Vector2 Parallax
    {
        set
        {
            _parallaxParam ??= Effect.Parameters["parallax"];
            _parallaxParam.SetValue(value);
        }
    }
}
