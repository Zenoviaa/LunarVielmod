using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Shaders
{
    public class BlackSeaSpriteShader : BaseShader
    {
        private static BlackSeaSpriteShader _instance;
        public static BlackSeaSpriteShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        private EffectParameter _multiplyColorParam;
        private EffectParameter _opacityParam;
        public Color MultiplyColor
        {
            set
            {
                _multiplyColorParam ??= Effect.Parameters["multiplyColor"];
                _multiplyColorParam.SetValue(value.ToVector3());
            }
        }

        public float Opacity
        {
            set
            {
                _opacityParam ??= Effect.Parameters["opacity"];
                _opacityParam.SetValue(value);
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            MultiplyColor = Color.White;
            Opacity = 1f;
        }

        public override void Apply()
        {

        }
    }
}
