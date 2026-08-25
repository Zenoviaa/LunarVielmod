using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class BlackFireOldShader : BaseShader
    {
        private EffectParameter _tilingParam;
        private EffectParameter _matrixParam;
        private EffectParameter _bloomTextureParam;
        private EffectParameter _primaryTextureParam2;
        private EffectParameter _primaryTextureParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _distortionTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _distortionParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private EffectParameter _backColorParam;
        private static BlackFireOldShader _instance;
        public static BlackFireOldShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Matrix TransformMatrix
        {
            set
            {
                _matrixParam ??= Effect.Parameters["transformMatrix"];
                _matrixParam.SetValue(value);
            }
        }

        public Asset<Texture2D> PrimaryTexture
        {
            set
            {
                _primaryTextureParam ??= Effect.Parameters["primaryTexture"];
                _primaryTextureParam.SetValue(value.Value);
            }
        }
        public Asset<Texture2D> PrimaryTexture2
        {
            set
            {
                _primaryTextureParam2 ??= Effect.Parameters["primaryTexture2"];
                _primaryTextureParam2.SetValue(value.Value);
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



        public Color BackColor
        {
            set
            {
                _backColorParam ??= Effect.Parameters["backColor"];
                _backColorParam.SetValue(value.ToVector3());
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
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            InnerColor = Color.Yellow;
            OuterColor = Color.Red;


            BackColor = Color.DarkRed;
            BlendState = BlendState.Additive;
            Distortion = 0.1f;

            PrimaryTexture = TrailRegistry.Beamlight;
            PrimaryTexture2 = TrailRegistry.SmallWhispyTrail;

            NoiseTexture = TrailRegistry.WhispyTrail;
            DistortionTexture = AssetRegistry.Textures.Noise.PerlinNoise;
            Time = Main.GlobalTimeWrappedHourly * 8;
            Tiling = Vector2.One * 3;
        }
    }
}
