using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Common.Shaders
{
    public class DescendingLightningBallShader : BaseShader
    {
        private EffectParameter _timeParam;
        private EffectParameter _gradientStartColor;
        private EffectParameter _gradientMidColor;
        private EffectParameter _gradientEndColor;
        private EffectParameter _noiseTexture;
        private static DescendingLightningBallShader _instance;
        public static DescendingLightningBallShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Color GradientStartColor
        {
            set
            {
                _gradientStartColor ??= Effect.Parameters["gradientStartColor"];
                _gradientStartColor.SetValue(value.ToVector3());
            }
        }

        public Color GradientMidColor
        {
            set
            {
                _gradientMidColor ??= Effect.Parameters["gradientMidColor"];
                _gradientMidColor.SetValue(value.ToVector3());
            }
        }
        public Color GradientEndColor
        {
            set
            {
                _gradientEndColor ??= Effect.Parameters["gradientEndColor"];
                _gradientEndColor.SetValue(value.ToVector3());
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

        public Asset<Texture2D> NoiseTexture
        {
            set
            {
                _noiseTexture ??= Effect.Parameters["noiseTexture"];
                _noiseTexture.SetValue(value.Value);
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            GradientStartColor = Color.Yellow;
            GradientMidColor = Color.Red;
            GradientEndColor = Color.Blue;
            NoiseTexture = AssetRegistry.NoiseTextures.IceWaterCaustics;
            BlendState = BlendState.AlphaBlend;
            Time = Main.GlobalTimeWrappedHourly * 1;
        }
    }
}
