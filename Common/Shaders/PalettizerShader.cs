using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Palettes;
using Terraria;
using Terraria.ModLoader;

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
                Effect.Parameters["ditherAlpha"].SetValue(value);
            }
        }
        public float Spread
        {
            set
            {
                Effect.Parameters["spread"].SetValue(value);
            }
        }

        public Texture3D PaletteTexture
        {
            set
            {
                Effect.Parameters["ColorSpectrumTexture"].SetValue(value);
            }
        }
        public Texture2D DitherTexture
        {
            set
            {
                Main.graphics.GraphicsDevice.Textures[1] = value;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                Effect.Parameters["ditherTexelSize"].SetValue(Vector2.One / (value.Size()));
            }
        }
        public float Progress
        {
            set
            {
                Effect.Parameters["uProgress"].SetValue(value);
            }
        }
        public bool Dither
        {
            set
            {
                Effect.Parameters["dither"].SetValue(value);
            }
        }
        public Vector2 ImageSize
        {
            set
            {
                Effect.Parameters["uImageSize1"].SetValue(value);
            }
        }

        public Vector2 ScreenOffset
        {
            set
            {
                Effect.Parameters["screenOffset"].SetValue(value);
            }
        }
        
        public static PalettizerShader Use(string palette)
        {
            PalettizerShader palettizerShader = ShaderContent.GetInstance<PalettizerShader>();
            palettizerShader.PaletteTexture = PaletteHelper.GetColorSpectrum(palette);
            palettizerShader.Progress = 1f;
            palettizerShader.Dither = ModContent.GetInstance<LunarVeilClientConfig>().Dither;
            palettizerShader.ImageSize = new Vector2(131, 312) * 4f;
            palettizerShader.DitherAlpha = 0.125f;
            palettizerShader.DitherTexture = AssetManager.Dithering.Dither8x8Double.Asset.Value;
            return palettizerShader;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
