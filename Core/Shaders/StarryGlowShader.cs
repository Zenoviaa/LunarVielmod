using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Core.Shaders
{
    public class StarryGlowShader : BaseShader
    {

        private EffectParameter _timeParam;
        private EffectParameter _glowParam;
        private static StarryGlowShader _instance;
        public static StarryGlowShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Color GlowColor
        {
            set
            {
                _glowParam ??= Effect.Parameters["glowColor"];
                _glowParam.SetValue(value.ToVector3());
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
            Time = Main.GlobalTimeWrappedHourly * 0.5f;
            GlowColor = Color.Purple;
        }
    }
}
