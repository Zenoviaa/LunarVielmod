using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;

namespace Stellamod.Common.Shaders
{

    internal class MotionBlurShader : BaseShader
    {
        private static MotionBlurShader _instance;
        public static MotionBlurShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Vector2 Velocity { get; set; }
        public float BlurStrength { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Velocity = -Vector2.UnitX * 0.1f;
            BlurStrength = 0.5f;
        }

        public override void Apply()
        {
            Effect.Parameters["velocity"].SetValue(Velocity);
            Effect.Parameters["blurStrength"].SetValue(BlurStrength);
        }
    }
}
