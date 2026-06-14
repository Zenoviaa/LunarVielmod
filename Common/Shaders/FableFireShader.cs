using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class FableFireShader : BaseShader
    {
        private EffectParameter _glowColorParam;
        private EffectParameter _tilingParam;
        private EffectParameter _distortionParam;
        private EffectParameter _noiseTextureParam3;
        private EffectParameter _noiseTextureParam2;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static FableFireShader _instance;
        public static FableFireShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Asset<Texture2D> NoiseTexture3
        {
            set
            {
                _noiseTextureParam3 ??= Effect.Parameters["noiseTexture3"];
                _noiseTextureParam3.SetValue(value.Value);
            }
        }

        public Asset<Texture2D> NoiseTexture2
        {
            set
            {
                _noiseTextureParam2 ??= Effect.Parameters["noiseTexture2"];
                _noiseTextureParam2.SetValue(value.Value);
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
        public Color GlowColor
        {
            set
            {
                _glowColorParam ??= Effect.Parameters["glowColor"];
                _glowColorParam.SetValue(value.ToVector3());
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
            GlowColor = Color.Blue;
            InnerColor = Color.Red;
            OuterColor = Color.Yellow;
            BlendState = BlendState.Additive;
            NoiseTexture3 = TrailRegistry.WaterTrail;
            NoiseTexture2 = TrailRegistry.DirnTrail;
            NoiseTexture = TrailRegistry.DirnTrail;
            Time = Main.GlobalTimeWrappedHourly * -14;
            Distortion = 0.214f;
            Tiling = Vector2.One * 1;
        }
    }
}
