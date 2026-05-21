using Stellamod.Common.Shaders;

namespace Stellamod.Content.Areas.Underground.BunnyStormBoss;

public class BunnyStormShader : CrystalShader<BunnyStormShader>
{
    private EffectParameter _tilingParam;
    private EffectParameter _mixTextureParam;
    private EffectParameter _offsetParam;
    public Vector2 Tiling
    {
        set
        {
            _tilingParam = Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public Vector2 Offset
    {
        set
        {
            _offsetParam = Effect.Parameters["offset"];
            _offsetParam.SetValue(value);
        }
    }
    public Texture2D MixTexture
    {
        set
        {
            _mixTextureParam = Effect.Parameters["mixTexture"];
            _mixTextureParam.SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

    }
}
