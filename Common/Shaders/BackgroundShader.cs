using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class MaskedBackgroundParallaxShader : BaseShader
    {
        private EffectParameter _parallaxParam;
        private EffectParameter _maskTextureParam;
        public Vector2 Parallax
        {
            set
            {
                _parallaxParam ??= Effect.Parameters["uImageOffset"];
                _parallaxParam.SetValue(value);
            }
        }
        public Texture2D MaskTexture
        {
            set
            {
                _maskTextureParam ??= Effect.Parameters["maskTexture"];
                _maskTextureParam.SetValue(value);
            }
        }

        private static MaskedBackgroundParallaxShader _instance;
        public static MaskedBackgroundParallaxShader Instance
        {
            get
            {
                _instance ??= new MaskedBackgroundParallaxShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Parallax = Vector2.Zero;
        }
    }
    public class HologramShader : BaseShader
    {
        private EffectParameter _noiseTextureSizeParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _parallaxParam;
        private EffectParameter _timeParam;
        public Texture2D NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value);
            }
        }
        public Vector2 NoiseTextureSize
        {
            set
            {
                _noiseTextureSizeParam ??= Effect.Parameters["noiseTextureSize"];
                _noiseTextureSizeParam.SetValue(value);
            }
        }

        public Vector2 Parallax
        {
            set
            {
                _parallaxParam ??= Effect.Parameters["uImageOffset"];
                _parallaxParam.SetValue(value);
            }
        }

        public float Time
        {
            set
            {
                _timeParam ??= Effect.Parameters["uTime"];
                _timeParam.SetValue(value);
            }
        }
        private static HologramShader _instance;
        public static HologramShader Instance
        {
            get
            {
                _instance ??= new HologramShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Parallax = Vector2.Zero;
            Time = 0;
        }
      
    }
    public class BackgroundParallaxShader : BaseShader
    {
        private EffectParameter _parallaxParam;
        public Vector2 Parallax
        {
            set
            {
                _parallaxParam ??= Effect.Parameters["uImageOffset"];
                _parallaxParam.SetValue(value);
            }
        }
        private static BackgroundParallaxShader _instance;
        public static BackgroundParallaxShader Instance
        {
            get
            {
                _instance ??= new BackgroundParallaxShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Parallax = Vector2.Zero;
        }
    }
}
