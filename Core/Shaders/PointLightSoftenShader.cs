namespace Stellamod.Core.Shaders
{
    public class PointLightSoftenShader : BaseShader
    {
        private static PointLightSoftenShader _instance;
        public static PointLightSoftenShader Instance
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
