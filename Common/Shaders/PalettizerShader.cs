using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class PalettizerShader : BaseShader
    {
        private EffectParameter _ditherAlphaParam;
        private EffectParameter _spreadParam;
        private EffectParameter _colorSpectrumParam;
        private EffectParameter _progressParam;
        private EffectParameter _ditherParam;
        private EffectParameter _sizeParam;
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
        public float DitherAlpha
        {
            set
            {
                _ditherAlphaParam ??= Effect.Parameters["ditherAlpha"];
                _ditherAlphaParam.SetValue(value);
            }
        }
        public float Spread
        {
            set
            {
                _spreadParam ??= Effect.Parameters["spread"];
                _spreadParam.SetValue(value);
            }
        }

        public Texture3D PaletteTexture
        {
            set
            {
                _colorSpectrumParam ??= Effect.Parameters["ColorSpectrumTexture"];
                _colorSpectrumParam.SetValue(value);
            }
        }
        public float Progress
        {
            set
            {
                _progressParam ??= Effect.Parameters["uProgress"];
                _progressParam.SetValue(value);
            }
        }
        public bool Dither
        {
            set
            {
                _ditherParam ??= Effect.Parameters["dither"];
                _ditherParam.SetValue(value);
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
