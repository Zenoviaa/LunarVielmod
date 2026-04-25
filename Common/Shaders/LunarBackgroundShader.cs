namespace Stellamod.Common.Shaders;

public class LunarBackgroundShader : CrystalShader<LunarBackgroundShader>
{
    private EffectParameter _fadeToColorParam;
    private EffectParameter _parallaxParam;
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
