using Stellamod.Assets;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Common.Shaders;

public class AuroraShader : BaseShader
{
    private static AuroraShader _instance;
    public static AuroraShader Instance
    {
        get
        {
            _instance ??= new AuroraShader();
            _instance.SetDefaults();
            return _instance;
        }
    }

    private EffectParameter _gradientTextureParam;
    private EffectParameter _gradientBackTextureParam;
    private EffectParameter _distortionTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _wavinessParam;
    private EffectParameter _rtSizeParam;
    private EffectParameter _parallaxParam;
    public Texture2D GradientTexture
    {
        set
        {
            _gradientTextureParam ??= Effect.Parameters["GradientTexture"];
            _gradientTextureParam.SetValue(value);
        }
    }
    public Texture2D GradientBackTexture
    {
        set
        {
            _gradientBackTextureParam ??= Effect.Parameters["GradientBackTexture"];
            _gradientBackTextureParam.SetValue(value);
        }
    }


    public Texture2D DistortionTexture
    {
        set
        {
            _distortionTextureParam ??= Effect.Parameters["DistortionTexture"];
            _distortionTextureParam.SetValue(value);
        }
    }

    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["Time"];
            _timeParam.SetValue(value);
        }
    }
    public float Waviness
    {
        set
        {
            _wavinessParam ??= Effect.Parameters["Waviness"];
            _wavinessParam.SetValue(value);
        }
    }

    public Vector2 RTSize
    {
        set
        {
            _rtSizeParam ??= Effect.Parameters["RTSize"];
            _rtSizeParam.SetValue(value);
        }
    }
    public Vector2 Parallax
    {
        set
        {
            _parallaxParam ??= Effect.Parameters["Parallax"];
            _parallaxParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Time = Main.GlobalTimeWrappedHourly * 0.1f;
        Waviness = 2.2f;
        GradientTexture = AssetManager.GlowMask.AuroraGradient.Value;
        GradientBackTexture = AssetManager.GlowMask.AuroraBackGradient.Value;
    }
}

public class GoldenAuroraShader :
    CrystalShader<GoldenAuroraShader>
{
    private EffectParameter _distortionAmtParam;
    private EffectParameter _distortionTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _tilingParam;

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["Tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public Texture2D DistortionTexture
    {
        set
        {
            _distortionTextureParam ??= Effect.Parameters["DistortionTexture"];
            _distortionTextureParam.SetValue(value);
        }
    }

    public float DistortionAmt
    {
        set
        {
            _distortionAmtParam ??= Effect.Parameters["DistortionAmt"];
            _distortionAmtParam.SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["Time"];
            _timeParam.SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Time = Main.GlobalTimeWrappedHourly * 0.1f;
    }
}
