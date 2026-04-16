using Terraria;

namespace Stellamod.Common.Shaders
{
    public class BannerWavingShader : BaseShader
    {
        private EffectParameter _timeParam;
        private EffectParameter _xOffsetParam;
        private EffectParameter _oscStrengthParam;
        private static BannerWavingShader _instance;
        public static BannerWavingShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
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
        public float XOffset
        {
            set
            {
                _xOffsetParam ??= Effect.Parameters["xOffset"];
                _xOffsetParam.SetValue(value);
            }
        }
        public float OscStrength
        {
            set
            {
                _oscStrengthParam ??= Effect.Parameters["oscStrength"];
                _oscStrengthParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Time = Main.GlobalTimeWrappedHourly;
            XOffset = 0.2f;
            OscStrength = 0.2f;
        }
    }
    public class RadialShearShader : BaseShader
    {
        private EffectParameter _timeParam;
        private static RadialShearShader _instance;
        public static RadialShearShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
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

        public override void SetDefaults()
        {
            base.SetDefaults();
            Time = 0;
        }
    }
}
