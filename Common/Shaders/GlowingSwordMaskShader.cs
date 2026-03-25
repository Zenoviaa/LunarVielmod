using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders;

public class GlowingSwordMaskShader : BaseShader
{
    private static GlowingSwordMaskShader _instance;
    public static GlowingSwordMaskShader Instance
    {
        get
        {
            _instance ??= new();
            _instance.SetDefaults();
            return _instance;
        }
    }
    private EffectParameter _bloomParam;
    private EffectParameter _innerColorParam;
    private EffectParameter _outerColorParam;
    private EffectParameter _timeParam;
    private EffectParameter _trailTextureParam;
    private EffectParameter _distortionTextureParam;
    private EffectParameter _distortionParam;

    public float Bloom
    {
        set
        {
            _bloomParam ??= Effect.Parameters["Bloom"];
            _bloomParam.SetValue(value);
        }
    }
    public Color InnerColor
    {
        set
        {
            _innerColorParam ??= Effect.Parameters["InnerColor"];
            _innerColorParam.SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            _outerColorParam ??= Effect.Parameters["OuterColor"];
            _outerColorParam.SetValue(value.ToVector3());
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

    public float Distortion
    {
        set
        {
            _distortionParam ??= Effect.Parameters["Distortion"];
            _distortionParam.SetValue(value);
        }
    }

    public Asset<Texture2D> DistortionTexture
    {
        set
        {
            _distortionTextureParam ??= Effect.Parameters["DistortionTexture"];
            _distortionTextureParam.SetValue(value.Value);
        }
    }

    public Asset<Texture2D> TrailTexture
    {
        set
        {
            _trailTextureParam ??= Effect.Parameters["TrailTexture"];
            _trailTextureParam.SetValue(value.Value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        InnerColor = Color.White;
        OuterColor = Color.Goldenrod;
        Time = Main.GlobalTimeWrappedHourly * 8;
        Distortion = 0.1f;
        TrailTexture = TrailRegistry.BeamTrail;
        DistortionTexture = AssetManager.Noise.Whirly;
    }
}
