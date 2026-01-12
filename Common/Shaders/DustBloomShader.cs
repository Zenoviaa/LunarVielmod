namespace Stellamod.Common.Shaders
{
    public class DustBloomShader : BaseShader
    {
        private EffectParameter _innerColorParam;
        private EffectParameter _bloomColorParam;
        private static DustBloomShader _instance;
        public static DustBloomShader Instance
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

        public Color BloomColor
        {
            set
            {
                _bloomColorParam ??= Effect.Parameters["bloomColor"];
                _bloomColorParam.SetValue(value.ToVector3());
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.White;
            BloomColor = Color.Red;
        }
    }
}
