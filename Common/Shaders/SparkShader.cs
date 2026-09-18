namespace Stellamod.Common.Shaders;

public class SparkShader : BaseShader
{
    private EffectParameter _innerColorParam;
    private EffectParameter _outerColorParam;
    private EffectParameter _powerParam;
    private static SparkShader _instance;
    public static SparkShader Instance
    {
        get
        {
            _instance ??= new();
            _instance.SetDefaults();
            return _instance;
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

    public float Power
    {
        set
        {
            _powerParam ??= Effect.Parameters["power"];
            _powerParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        InnerColor = Color.White;
        OuterColor = Color.Yellow;
        BlendState = BlendState.Additive;
        Power = 1;
    }
}
