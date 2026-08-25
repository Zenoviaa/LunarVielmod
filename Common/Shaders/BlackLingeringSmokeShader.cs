using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class BlackLingeringSmokeShader : BaseShader
    {
        private EffectParameter _tilingParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _distortionTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _distortionParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static BlackLingeringSmokeShader _instance;
        public static BlackLingeringSmokeShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Asset<Texture2D> NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value.Value);
            }
        }
        public Asset<Texture2D> DistortionTexture
        {
            set
            {
                _distortionTextureParam ??= Effect.Parameters["distortionTexture"];
                _distortionTextureParam.SetValue(value.Value);
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

        public Color OuterColor
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


        public float Distortion
        {
            set
            {
                _distortionParam ??= Effect.Parameters["distortion"];
                _distortionParam.SetValue(value);
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
        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.Lerp(Color.DarkRed, Color.Black, 0.75f);
            OuterColor = Color.Lerp(Color.DarkGray, Color.Black, 0.9f);

            BlendState = BlendState.AlphaBlend;
            Distortion = 0.15f;

            NoiseTexture = TrailRegistry.Clouds3;
            DistortionTexture = AssetRegistry.Textures.Noise.PerlinNoise;
            Time = Main.GlobalTimeWrappedHourly * 0.5f;
            Tiling = Vector2.One * 8;
        }
    }
}
