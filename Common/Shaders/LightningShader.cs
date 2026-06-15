using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class LightningShader : BaseShader
    {
        private EffectParameter _matrixParam;
        private EffectParameter _primaryTextureParam;
        private EffectParameter _noiseTextureParam;
        private EffectParameter _timeParam;
        private EffectParameter _powerParam;
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static LightningShader _instance;
        public static LightningShader Instance
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
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            InnerColor = Color.White;
            OuterColor = Color.Yellow;
            BlendState = BlendState.Additive;
            Power = 5;

            PrimaryTexture = TrailRegistry.LightningTrail;
            NoiseTexture = TrailRegistry.LightningTrail;
            Time = Main.GlobalTimeWrappedHourly * 8;
        }
    }
}
