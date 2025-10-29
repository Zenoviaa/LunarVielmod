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
    public class BossHealthbarShader : BaseShader
    {
        private EffectParameter _noiseTextureParam;
        private EffectParameter _timeParam;
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

        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.Lerp(Color.White, Color.Black, 0.75f);
            OuterColor = Color.Lerp(Color.DarkBlue, Color.Black, 0.9f);

            BlendState = BlendState.AlphaBlend;
            NoiseTexture = TrailRegistry.Clouds3;
            Time = Main.GlobalTimeWrappedHourly * 0.5f;
        }
    }
}
