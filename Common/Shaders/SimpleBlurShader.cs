namespace Stellamod.Common.Shaders
{
    public class SimpleBlurShader : BaseShader
    {
        private static SimpleBlurShader _instance;
        private EffectParameter _blurParameter;
        public static SimpleBlurShader Instance
        {
            get
            {
                _instance ??= new SimpleBlurShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Vector2 TexelSize
        {
            set
            {
                _blurParameter ??= Effect.Parameters["texelSize"];
                _blurParameter.SetValue(value);
            }
        }
    }
}
