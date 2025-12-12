using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class BloodStormShader : BaseShader
    {
        private EffectParameter _timeParam;
        private EffectParameter _centerColorParam;
        private EffectParameter _outerColorParam;
        private EffectParameter _vortexDarkColorParam;
        private EffectParameter _vortexLightColorParam;
        private static BloodStormShader _instance;
        public static BloodStormShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Color VortexDarkColor
        {
            set
            {
                _vortexDarkColorParam ??= Effect.Parameters["vortexDarkColor"];
                _vortexDarkColorParam.SetValue(value.ToVector3());
            }
        }

        public Color VortexLightColor
        {
            set
            {
                _vortexLightColorParam ??= Effect.Parameters["vortexLightColor"];
                _vortexLightColorParam.SetValue(value.ToVector3());
            }
        }
        public Color CenterColor
        {
            set
            {
                _centerColorParam ??= Effect.Parameters["centerColor"];
                _centerColorParam.SetValue(value.ToVector3());
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
            VortexDarkColor = Color.Black;
            VortexLightColor = Color.White;
            CenterColor = Color.Black;
            OuterColor = Color.Red;
            BlendState = BlendState.AlphaBlend;
            Time = Main.GlobalTimeWrappedHourly * 1;
        }
    }
}
