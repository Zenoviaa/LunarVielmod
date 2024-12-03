using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders.MagicTrails
{
    internal class MagicIceShader : BaseShader
    {
        private static MagicIceShader _instance;
        public static MagicIceShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> TrailTexture { get; set; }
        public Asset<Texture2D> MorphTexture { get; set; }
        public Asset<Texture2D> DistortingTexture { get; set; }
        public Asset<Texture2D> InnerTexture { get; set; }
        public Color TrailColor { get; set; }
        public Color GlowColor { get; set; }
        public float Speed { get; set; }
        public float Distortion { get; set; }
        public float Pixelation { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailTexture = TrailRegistry.IceTrailFlat;
            MorphTexture = TrailRegistry.IceTrailSpiked;
            DistortingTexture = TrailRegistry.Clouds3;
            InnerTexture = TrailRegistry.IceTrail;
            TrailColor = Color.White;
            GlowColor = Color.LightBlue;
            Speed = 5;
            Distortion = 0.2f;
            Pixelation = 0.0125f;
        }

        public override void Apply()
        {
            Effect.Parameters["transformMatrix"].SetValue(TrailDrawer.WorldViewPoint2);
            Effect.Parameters["trailColor"].SetValue(TrailColor.ToVector3());
            Effect.Parameters["glowColor"].SetValue(GlowColor.ToVector3());
            Effect.Parameters["trailTexture"].SetValue(TrailTexture.Value);
            Effect.Parameters["morphTexture"].SetValue(MorphTexture.Value);
            Effect.Parameters["distortionTexture"].SetValue(DistortingTexture.Value);
            Effect.Parameters["innerTexture"].SetValue(InnerTexture.Value);
            Effect.Parameters["innerTexSize"].SetValue(InnerTexture.Value.Size());
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["px"].SetValue(Pixelation);
        }
    }
}
