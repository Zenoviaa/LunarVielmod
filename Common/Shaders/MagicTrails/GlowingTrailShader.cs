using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.Shaders.MagicTrails
{
    public class GlowingTrailShader : BaseShader
    {
        private EffectParameter _matrixParam;

        private EffectParameter _noiseTextureParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private EffectParameter _timeParam;
        private EffectParameter _distortionParam;
        private EffectParameter _tilingParam;

        private static GlowingTrailShader _instance;
        public static GlowingTrailShader Instance
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
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            InnerColor = Color.Red;
            OuterColor = Color.White;
            Distortion = 0.6f;
            BlendState = BlendState.AlphaBlend;
            NoiseTexture = TextureRegistry.BlurryPerlinNoise2;
            Time = Main.GlobalTimeWrappedHourly * 20;
            Tiling = Vector2.One * 0.5f;
        }
    }
}
