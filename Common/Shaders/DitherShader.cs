namespace Stellamod.Common.Shaders
{
    public class DitherShader : BaseShader
    {
        private EffectParameter _sizeParam;
        private static DitherShader _instance;
        public static DitherShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Vector2 ImageSize
        {
            set
            {
                _sizeParam ??= Effect.Parameters["uImageSize1"];
                _sizeParam.SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
