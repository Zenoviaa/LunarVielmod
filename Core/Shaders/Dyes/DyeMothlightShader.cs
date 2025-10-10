using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Core.Shaders.Dyes
{
    public class DyeMothlightShader : BaseShader
    {
        private static DyeMothlightShader _instance;
        public static DyeMothlightShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> NoiseTexture { get; set; }
        public Color PrimaryColor { get; set; }
        public Color NoiseColor { get; set; }
        public Color OutlineColor { get; set; }
        public float Speed { get; set; }
        public float Distortion { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NoiseTexture = TrailRegistry.StarTrail;
            PrimaryColor = Color.White;
            NoiseColor = Color.White;
            OutlineColor = Color.White;
            Speed = 5;
            Distortion = 0.2f;
        }

        public override void Apply()
        {
            Data.UseImage1(NoiseTexture);
            Effect.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector3());
            Effect.Parameters["noiseColor"].SetValue(NoiseColor.ToVector3());
            Effect.Parameters["outlineColor"].SetValue(OutlineColor.ToVector3());
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["distortion"].SetValue(Distortion);
        }
    }
}
