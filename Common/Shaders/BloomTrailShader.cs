using ReLogic.Content;
using Stellamod.Assets;

namespace Stellamod.Common.Shaders;

public class BloomTrailShader : BaseShader
{
    private EffectParameter _tilingParam;
    private EffectParameter _matrixParam;
    private EffectParameter _bloomTextureParam;

    private EffectParameter _innerColorParam;
    private EffectParameter _outerColorParam;
    private static BloomTrailShader _instance;
    public static BloomTrailShader Instance
    {
        get
        {
            _instance ??= new();
            _instance.SetDefaults();
            return _instance;
        }
    }

    public Matrix TransformMatrix
    {
        set
        {
            _matrixParam ??= Effect.Parameters["transformMatrix"];
            _matrixParam.SetValue(value);
        }
    }


    public Asset<Texture2D> BloomTexture
    {
        set
        {
            _bloomTextureParam ??= Effect.Parameters["bloomTexture"];
            _bloomTextureParam.SetValue(value.Value);
        }
    }


    public Color InnerColor
    {
        set
        {
            _innerColorParam ??= Effect.Parameters["innerColor"];
            _innerColorParam.SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            _outerColorParam ??= Effect.Parameters["outerColor"];
            _outerColorParam.SetValue(value.ToVector3());
        }
    }

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TransformMatrix = TrailDrawer.WorldViewPoint2;
        InnerColor = Color.Yellow;
        OuterColor = Color.Red;
        BlendState = BlendState.Additive;
        BloomTexture = AssetManager.LaserTextures.Bloom;
        Tiling = Vector2.One * 1;
    }
}
