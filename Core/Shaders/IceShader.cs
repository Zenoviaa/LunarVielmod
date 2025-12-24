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

namespace Stellamod.Core.Shaders
{
    public class IceShader : BaseShader
    {
        private EffectParameter _screenOffsetParam;
        private EffectParameter _tilingParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static IceShader _instance;
        public static IceShader Instance
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

        public Vector2 Tiling
        {
            set
            {
                _tilingParam ??= Effect.Parameters["tiling"];
                _tilingParam.SetValue(value);
            }
        }
        public Vector2 ScreenOffset
        {
            set
            {
                _screenOffsetParam ??= Effect.Parameters["screenOffset"];
                _screenOffsetParam.SetValue(value);
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.White;
            OuterColor = Color.Lerp(Color.LightGray, Color.Cyan, 0.5f);
            BlendState = BlendState.AlphaBlend;
            NoiseTexture = TrailRegistry.Clouds3;
            Tiling = Vector2.One * 8;
            ScreenOffset = -Main.screenPosition * 0.5f;
        }
    }
}
