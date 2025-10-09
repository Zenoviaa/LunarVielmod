using Microsoft.Xna.Framework;

namespace Stellamod.Core.Shaders
{
    public class SpriteWhiteShader : BaseShader
    {
        private static SpriteWhiteShader _instance;
        public static SpriteWhiteShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
     
        }

        public override void Apply()
        {

        }
    }
}
