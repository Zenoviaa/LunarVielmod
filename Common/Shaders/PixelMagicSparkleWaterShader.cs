using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders
{
    internal class PixelMagicSparkleWaterShader : BaseShader
    {
        private static PixelMagicSparkleWaterShader _instance;
        public static PixelMagicSparkleWaterShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }

        public Asset<Texture2D> NoiseTexture { get; set; }
        public Asset<Texture2D> OutlineTexture { get; set; }
        public Color PrimaryColor { get; set; }
        public Color NoiseColor { get; set; }
        public Color OutlineColor { get; set; }
        public float Speed { get; set; }
        public float Distortion { get; set; }
        public float Power { get; set; }
        public float Threshold { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NoiseTexture = TrailRegistry.Clouds3;
            OutlineTexture = TrailRegistry.DottedTrailOutline;
            PrimaryColor = Color.White;
            NoiseColor = Color.White;
            OutlineColor = Color.White;
            Speed = 5;
            Distortion = 0.2f;
            Power = 1.5f;
        }
        public override void Apply()
        {
            Data.UseImage1(NoiseTexture);
            Data.UseImage2(OutlineTexture);
            Effect.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector3());
            Effect.Parameters["noiseColor"].SetValue(NoiseColor.ToVector3());
            Effect.Parameters["outlineColor"].SetValue(OutlineColor.ToVector3());
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["power"].SetValue(Power);
            Effect.Parameters["threshold"].SetValue(Threshold);
        }
    }
}
