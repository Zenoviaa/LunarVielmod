using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class SkullfireShader : BaseShader
    {
        private EffectParameter _velocityParam;
        private EffectParameter _powerParam;
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

        public float Power
        {
            set
            {
                _powerParam ??= Effect.Parameters["power"];
                _powerParam.SetValue(value);
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
        public Vector2 Velocity
        {
            set
            {
                _velocityParam ??= Effect.Parameters["velocity"];
                _velocityParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.Red;
            OuterGlowColor = Color.Yellow;
            FadeColor = Color.Blue;
            Power = 1;
            Distortion = 0.1f;
            Time = Main.GlobalTimeWrappedHourly * 12;
            Tiling = new Vector2(1, 1f) * 1;
            Velocity = Vector2.Zero;
            NoiseTexture = TrailRegistry.WhispyTrail.Value;
        }

    }
}
