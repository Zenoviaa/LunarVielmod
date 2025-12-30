using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class PalettizerShader : BaseShader
    {
        private static PalettizerShader _instance;
        public static PalettizerShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }



        public Texture3D PaletteTexture
        {
            set
            {
                Effect.Parameters["ColorSpectrumTexture"].SetValue(value);
            }
        }
        public float Progress
        {
            set
            {
                Effect.Parameters["uProgress"].SetValue(value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
