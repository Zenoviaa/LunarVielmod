using Terraria;

namespace Stellamod.Common.Shaders
{
    public class CelestialAuraShader : BaseShader
    {
        private EffectParameter _timeParam;
        private EffectParameter _tilingParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static CelestialAuraShader _instance;
        public static CelestialAuraShader Instance
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
            InnerColor = Color.Lerp(Color.DarkRed, Color.Black, 0.75f);
            OuterColor = Color.Lerp(Color.DarkGray, Color.Black, 0.9f);
            Tiling = Vector2.One;
            BlendState = BlendState.AlphaBlend;
            Time = Main.GlobalTimeWrappedHourly * 0.5f;
        }
    }
}
