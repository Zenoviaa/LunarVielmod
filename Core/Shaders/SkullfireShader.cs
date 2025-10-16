using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class SkullfireShader : BaseShader
    {
        private EffectParameter _fadeColorParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private EffectParameter _timeParam;
        private EffectParameter _tilingParam;
        private EffectParameter _distortionParam;
        private EffectParameter _noiseTextureParam;
        private static SkullfireShader _instance;
        public static SkullfireShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Color FadeColor
        {
            set
            {
                _fadeColorParam ??= Effect.Parameters["fadeColor"];

                _fadeColorParam.SetValue(value.ToVector3());
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
            InnerColor = Color.Yellow;
            OuterGlowColor = Color.Red;
            FadeColor = Color.OrangeRed;
            Time = Main.GlobalTimeWrappedHourly * 8;
            Tiling = Vector2.One * 2;
            NoiseTexture = TextureRegistry.BlurryPerlinNoise2.Value;
        }

    }
}
