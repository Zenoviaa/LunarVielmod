using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;

public class FixedRichLaserShader : RichLaserShader
{

}

public class RichLaserShader : BaseShader
{
    private EffectParameter _matrixParam;
    private EffectParameter _tilingParam;
    private EffectParameter _laserTextureParam;
    private EffectParameter _bloomTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _laserColorParam;
    private EffectParameter _bloomInnerColorParam;
    private EffectParameter _bloomOuterColorParam;

    private static RichLaserShader _instance;
    public static RichLaserShader Instance
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

    public Asset<Texture2D> LaserTexture
    {
        set
        {
            _laserTextureParam ??= Effect.Parameters["laserTexture"];
            _laserTextureParam.SetValue(value.Value);
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
            _bloomInnerColorParam ??= Effect.Parameters["bloomInnerColor"];
            _bloomInnerColorParam.SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            _bloomOuterColorParam ??= Effect.Parameters["bloomOuterColor"];
            _bloomOuterColorParam.SetValue(value.ToVector3());
        }
    }

    public Color LaserColor
    {
        set
        {
            _laserColorParam ??= Effect.Parameters["laserColor"];
            _laserColorParam.SetValue(value.ToVector3());
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
        LaserColor = Color.Cyan;
        InnerColor = Color.LightSkyBlue;
        OuterColor = Color.DeepSkyBlue;
        LaserTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BulbTrail");
        BloomTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/TexturedLaser");

        BlendState = BlendState.AlphaBlend;
        Time = Main.GlobalTimeWrappedHourly * 36;
        // Tiling = Vector2.One;
    }
}
