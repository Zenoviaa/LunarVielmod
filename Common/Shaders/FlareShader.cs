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
    public class FlareShader : BaseShader
    {
        private EffectParameter _powerParam;
        private EffectParameter _distortionTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _distortionParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static FlareShader _instance;
        public static FlareShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
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

        public float Power
        {
            set
            {
                _powerParam ??= Effect.Parameters["power"];
                _powerParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.Yellow;
            OuterColor = Color.Red;
            BlendState = BlendState.Additive;
            Distortion = 0.15f;
            Power = 1;
            DistortionTexture = AssetRegistry.Textures.Noise.Perlin;
            Time = Main.GlobalTimeWrappedHourly * 5;
        }
    }
}
