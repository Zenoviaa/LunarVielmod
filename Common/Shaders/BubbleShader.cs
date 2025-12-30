using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class BubbleShader : BaseShader
    {
        private EffectParameter _tilingParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _distortionParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static BubbleShader _instance;
        public static BubbleShader Instance
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
            InnerColor = Color.Yellow;
            OuterColor = Color.Red;

            BlendState = BlendState.Additive;
            Distortion = 0.025f;

            NoiseTexture = TrailRegistry.Clouds3;
            Time = Main.GlobalTimeWrappedHourly * -3;
            Tiling = Vector2.One * 8;
        }
    }
}
