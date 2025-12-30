namespace Stellamod.Common.Shaders
{
    public class FeatherShader : BaseShader
    {
        private static FeatherShader _instance;
        public static FeatherShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
    }
}
