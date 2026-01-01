using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders
{
    public class BackgroundParallaxShader : BaseShader
    {
        private EffectParameter _parallaxParam;
        public Vector2 Parallax
        {
            set
            {
                _parallaxParam ??= Effect.Parameters["uImageOffset"];
                _parallaxParam.SetValue(value);
            }
        }
        private static BackgroundParallaxShader _instance;
        public static BackgroundParallaxShader Instance
        {
            get
            {
                _instance ??= new BackgroundParallaxShader();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Parallax = Vector2.Zero;
        }
    }
}
