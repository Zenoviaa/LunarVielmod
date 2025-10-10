using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Core.Shaders.Dyes
{
    public class DyeInvisibleShader : BaseShader
    {
        private static DyeInvisibleShader _instance;
        public static DyeInvisibleShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> NoiseTexture { get; set; }
        public float Speed { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NoiseTexture = TrailRegistry.Clouds3;
            Speed = 5;
        }

        public override void Apply()
        {
            Data.UseImage1(NoiseTexture);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
        }
    }
}
