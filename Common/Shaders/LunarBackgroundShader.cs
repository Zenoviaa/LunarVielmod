namespace Stellamod.Common.Shaders;

public class LunarBackgroundShader : CrystalShader<LunarBackgroundShader>
{
    private EffectParameter _dustTextureParam;
    private EffectParameter _fadeToColorParam;
    private EffectParameter _parallaxParam;
    private EffectParameter _timeParam;

    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }
    public Texture2D DustTexture
    {
        set
        {
            _dustTextureParam ??= Effect.Parameters["dustTexture"];
            _dustTextureParam.SetValue(value);
        }
    }
    public Vector2[] Parallax
    {
        set
        {
            _parallaxParam ??= Effect.Parameters["parallax"];
            _parallaxParam.SetValue(value);
        }
    }

    public Color FadeToColor
    {
        set
        {
            _fadeToColorParam ??= Effect.Parameters["fadeToColor"];
            _fadeToColorParam.SetValue(value.ToVector4());
        }
    }
}
