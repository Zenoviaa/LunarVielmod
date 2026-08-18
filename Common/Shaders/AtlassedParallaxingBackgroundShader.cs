using Terraria;

namespace Stellamod.Common.Shaders;

public class AtlassedParallaxingBackgroundShader : CrystalShader<AtlassedParallaxingBackgroundShader>
{
    private EffectParameter _fadeToColorParam;
    private EffectParameter _parallaxParam;
    private EffectParameter _offsetsParam;
    private EffectParameter _tilingParam;

    public Vector2 Tiling
    {
        set
        {
            _tilingParam = Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public Vector2[] Parallax
    {
        set
        {
            _parallaxParam = Effect.Parameters["parallax"];
            _parallaxParam.SetValue(value);
        }
    }

    public Vector2[] Offsets
    {
        set
        {
            _offsetsParam = Effect.Parameters["offsets"];
            _offsetsParam.SetValue(value);
        }
    }

    public Color FadeToColor
    {
        set
        {
            _fadeToColorParam = Effect.Parameters["fadeToColor"];
            _fadeToColorParam.SetValue(value.ToVector4());
        }
    }


   public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
        }
    }

    public float HeatDistortion
    {
        set
        {
            Effect.Parameters["heatDistortion"].SetValue(value);
        }
    }

    public Texture2D NormalNoise1
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicWrap;
        }
    }
}
