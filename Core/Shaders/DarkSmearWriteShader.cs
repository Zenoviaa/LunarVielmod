namespace Stellamod.Core.Shaders
{
    public class DarkSmearWriteShader : BaseShader
    {
        private static DarkSmearWriteShader _instance;
        public static DarkSmearWriteShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
