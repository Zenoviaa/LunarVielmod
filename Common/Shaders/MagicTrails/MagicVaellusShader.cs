using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Common.Shaders.MagicTrails
{
    internal class MagicVaellusShader : BaseShader
    {
        private static MagicVaellusShader _instance;
        public static MagicVaellusShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> PrimaryTexture { get; set; }
        public Asset<Texture2D> NoiseTexture { get; set; }
        public Asset<Texture2D> OutlineTexture { get; set; }
        public Color PrimaryColor { get; set; }
        public Color NoiseColor { get; set; }
        public Color OutlineColor { get; set; }
        public float Speed { get; set; }
        public float Distortion { get; set; }
        public float Power { get; set; }
        public float Alpha { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            PrimaryTexture = TrailRegistry.LightningTrail2;
            NoiseTexture = TrailRegistry.LightningTrail3;
            OutlineTexture = TrailRegistry.LightningTrail2Outline;
            PrimaryColor = new Color(69, 70, 159);
            NoiseColor = new Color(224, 107, 10);
            OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointWrap;
            Speed = 5.2f;
            Distortion = 0.15f;
            Power = 0.25f;
            Alpha = 1f;
        }

        public override void Apply()
        {
            Effect.Parameters["transformMatrix"].SetValue(TrailDrawer.WorldViewPoint2);
            Effect.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector3());
            Effect.Parameters["noiseColor"].SetValue(NoiseColor.ToVector3());
            Effect.Parameters["outlineColor"].SetValue(OutlineColor.ToVector3());
            Effect.Parameters["primaryTexture"].SetValue(PrimaryTexture.Value);
            Effect.Parameters["noiseTexture"].SetValue(NoiseTexture.Value);
            Effect.Parameters["outlineTexture"].SetValue(OutlineTexture.Value);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["power"].SetValue(Power);
            Effect.Parameters["alpha"].SetValue(Alpha);
        }
    }

    internal class PixelMagicVaellusShader : BaseShader
    {
        private static PixelMagicVaellusShader _instance;
        public static PixelMagicVaellusShader Instance
        {
            get
            {
                _instance ??= new();
                _instance.SetDefaults();
                return _instance;
            }
        }
        public Asset<Texture2D> PrimaryTexture { get; set; }
        public Asset<Texture2D> NoiseTexture { get; set; }
        public Asset<Texture2D> OutlineTexture { get; set; }
        public Color PrimaryColor { get; set; }
        public Color NoiseColor { get; set; }
        public Color OutlineColor { get; set; }
        public float Speed { get; set; }
        public float Distortion { get; set; }
        public float Power { get; set; }
        public float Blend { get; set; }
        public override void SetDefaults()
        {
            base.SetDefaults();
            PrimaryTexture = TrailRegistry.LightningTrail2;
            NoiseTexture = TrailRegistry.LightningTrail3;
            OutlineTexture = TrailRegistry.LightningTrail2Outline;
            PrimaryColor = new Color(69, 70, 159);
            NoiseColor = new Color(224, 107, 10);
            OutlineColor = Color.Lerp(new Color(31, 27, 59), Color.Black, 0.75f);
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointWrap;
            Speed = 5.2f;
            Distortion = 0.15f;
            Power = 0.25f;
            Blend = 0.4f;
        }

        public override void Apply()
        {
            Effect.Parameters["transformMatrix"].SetValue(TrailDrawer.WorldViewPoint2);
            Effect.Parameters["primaryColor"].SetValue(PrimaryColor.ToVector3());
            Effect.Parameters["noiseColor"].SetValue(NoiseColor.ToVector3());
            Effect.Parameters["outlineColor"].SetValue(OutlineColor.ToVector3());
            Effect.Parameters["primaryTexture"].SetValue(PrimaryTexture.Value);
            Effect.Parameters["noiseTexture"].SetValue(NoiseTexture.Value);
            Effect.Parameters["outlineTexture"].SetValue(OutlineTexture.Value);
            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * Speed);
            Effect.Parameters["distortion"].SetValue(Distortion);
            Effect.Parameters["power"].SetValue(Power);
            Effect.Parameters["blend"].SetValue(Blend);
        }
    }
}
