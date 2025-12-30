using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class DustShader : BaseShader
    {
        private EffectParameter _innerColorParam;
        private EffectParameter _outerColorParam;
        private static DustShader _instance;
        public static DustShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
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

        public override void SetDefaults()
        {
            base.SetDefaults();
            InnerColor = Color.White;
            OuterColor = Color.Red;
        }
    }
}
