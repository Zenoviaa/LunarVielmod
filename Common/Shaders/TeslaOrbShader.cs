using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class TeslaOrbShader : BaseShader
    {
        private EffectParameter _powerParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;

        private EffectParameter _lightninginnerColorParam;
        private EffectParameter _lightningouterColorParam;

        private static TeslaOrbShader _instance;
        public static TeslaOrbShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
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

        public Texture2D NoiseTexture
        {
            set
            {
                _noiseTextureParam ??= Effect.Parameters["noiseTexture"];
                _noiseTextureParam.SetValue(value);
            }
        }

        public Color LightningInnerColor
        {
            set
            {
                _lightninginnerColorParam ??= Effect.Parameters["lightningInnerColor"];
                _lightninginnerColorParam.SetValue(value.ToVector3());
            }
        }

        public Color LightningOuterColor
        {
            set
            {
                _lightninginnerColorParam ??= Effect.Parameters["lightningOuterColor"];
                _lightninginnerColorParam.SetValue(value.ToVector3());
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
            InnerColor = Color.White;
            OuterColor = Color.Yellow;
            LightningInnerColor = Color.Blue;
            LightningOuterColor = Color.Goldenrod;
            BlendState = BlendState.Additive;
            Time = Main.GlobalTimeWrappedHourly * -32;
            NoiseTexture = TrailRegistry.WaterTrail.Value;
            Power = ExtraMath.Osc(1f, 2f, speed: 2);
        }
    }
}
