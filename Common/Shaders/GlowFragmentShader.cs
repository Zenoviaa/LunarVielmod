using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class GlowFragmentShader : BaseShader
    {
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private EffectParameter _timeParam;
        private EffectParameter _tilingParam;
        private EffectParameter _distortionParam;
        private EffectParameter _noiseTextureParam;
        private static GlowFragmentShader _instance;
        public static GlowFragmentShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Color InnerColor
        {
            set
            {
                _innerColorParam ??= Effect.Parameters["innerColor"];
        
                _innerColorParam.SetValue(value.ToVector3());
            }
        }
        public Color OuterGlowColor
        {
            set
            {
                _outerColorParam ??= Effect.Parameters["outerColor"];
                _outerColorParam.SetValue(value.ToVector3());
            }
        }
        public float Time
        {
            set
            {
                _timeParam ??= Effect.Parameters["time"];
                _timeParam.SetValue(value);
            }
        }

        public Vector2 Tiling
        {
            set
            {
                _tilingParam ??= Effect.Parameters["tiling"];
                _tilingParam.SetValue(value);
            }
        }

        public float Distortion
        {
            set
            {
                _distortionParam ??= Effect.Parameters["distortion"];
                _distortionParam.SetValue(value);
            }
        }

        public Texture2D NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value);
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.White;
            OuterGlowColor = Color.Red;
            Time = Main.GlobalTimeWrappedHourly * 8;
            Tiling = Vector2.One * 1;
            Distortion = 0.1f;
            NoiseTexture = TextureRegistry.BlurryPerlinNoise2.Value;
        }

    }
}
