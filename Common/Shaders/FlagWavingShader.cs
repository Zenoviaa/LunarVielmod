using Terraria;

namespace Stellamod.Common.Shaders
{
    public class FlagWavingShader : BaseShader
    {
        private EffectParameter _timeParam;
        private EffectParameter _xOffsetParam;
        private EffectParameter _oscStrengthParam;
        private static FlagWavingShader _instance;
        public static FlagWavingShader Instance
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
}
