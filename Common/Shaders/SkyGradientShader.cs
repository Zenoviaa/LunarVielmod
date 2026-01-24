namespace Stellamod.Common.Shaders
{
    public class SkyGradientShader : BaseShader
    {
        private EffectParameter _bendParam;
        private EffectParameter _hParam;
        private EffectParameter _startColorParam;
        private EffectParameter _midColorParam;
        private EffectParameter _endColorParam;
        private static SkyGradientShader _instance;
        public static SkyGradientShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Color StartColor
        {
            set
            {
                _startColorParam ??= Effect.Parameters["startColor"];
                _startColorParam.SetValue(value.ToVector4());
            }
        }

        public Color MidColor
        {
            set
            {
                _midColorParam ??= Effect.Parameters["midColor"];
                _midColorParam.SetValue(value.ToVector4());
            }
        }
        public Color EndColor
        {
            set
            {
                _endColorParam ??= Effect.Parameters["endColor"];
                _endColorParam.SetValue(value.ToVector4());
            }
        }

        public float Bend
        {
            set
            {
                _bendParam ??= Effect.Parameters["bend"];
                _bendParam.SetValue(value);
            }
        }


        public float H
        {
            set
            {
                _hParam ??= Effect.Parameters["h"];
                _hParam.SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Bend = 0.05f;
            H = 0.8f;
            StartColor = Color.Black;
            MidColor = Color.White;
            EndColor = Color.Black;
        }
    }
}
