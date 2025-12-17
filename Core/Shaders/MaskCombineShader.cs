using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace Stellamod.Core.Shaders
{
    public class MaskCombineShader : BaseShader
    {
        private EffectParameter _mixTextureParam;
        private static MaskCombineShader _instance;
        public static MaskCombineShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Texture2D MixTexture
        {
            set
            {
                _mixTextureParam ??= Effect.Parameters["mixTexture"];
                _mixTextureParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

        }
    }
}
