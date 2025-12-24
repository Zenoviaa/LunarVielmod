using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Shaders
{
    public class FrozenShader : BaseShader
    {
        private EffectParameter _tintAlphaParam;
        private EffectParameter _tintColorParam;
        private static FrozenShader _instance;
        public static FrozenShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public Color TintColor
        {
            set
            {
                _tintColorParam ??= Effect.Parameters["tintColor"];
                _tintColorParam.SetValue(value.ToVector3());
            }
        }

        public float TintAlpha
        {
            set
            {
                _tintAlphaParam ??= Effect.Parameters["tintAlpha"];
                _tintAlphaParam.SetValue(value);
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TintColor = Color.Cyan;
            TintAlpha = 0.66f;
        }
    }
}
