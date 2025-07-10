using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellamod.Core.Helpers.Math;
using System.Threading;

namespace Stellamod.Core.Effects.Trails
{
    public class MoonIceTrailer : TrailDrawer
    {
        public MoonIceTrailer()
        {
            Shader = new MoonIceTrail();
            TrailWidthFunction = DefaultWidthFunction;
            TrailColorFunction = DefaultColorFunction;
        }

        private float DefaultWidthFunction(float interpolant)
        {
            return MathHelper.SmoothStep(48, 0, interpolant);
        }

        private Color DefaultColorFunction(float interpolant)
        {
            Color tipColor = Color.Lerp(Color.LightCyan, Color.DeepSkyBlue, MathHelper.SmoothStep(0f, 1f, ExtraMath.Osc(0f, 1f, speed: 3)));
            return Color.Lerp(Color.LightCyan, tipColor, interpolant);
        }
    }

    public class MoonIceTrail : Shader
    {
        public MoonIceTrail()
        {
            //Cache parameters
            TransformMatrixParam = Effect.Parameters["transformMatrix"];
            TrailTextureParam = Effect.Parameters["trailTexture"];
            NoiseTextureParam = Effect.Parameters["noiseTexture"];
            TimeParam = Effect.Parameters["Time"];
            OffsetParam = Effect.Parameters["Offset"];
            TilingParam = Effect.Parameters["Tiling"];
            DistortionParam = Effect.Parameters["Distortion"];
            BaseColorParam = Effect.Parameters["BaseColor"];
            HighlightColorParam = Effect.Parameters["HighlightColor"];
            InnerGlowColorParam = Effect.Parameters["InnerGlowColor"];
            InnerCoreColorParam = Effect.Parameters["InnerCoreColor"];
            SparkleColorParam = Effect.Parameters["SparkleColor"];
            BlendState = BlendState.AlphaBlend;

            //Set defaults
            Offset = Vector2.Zero;
            Tiling = Vector2.One;
            Distortion = 0.1f;
            Time = 0.0f;
            BaseColor = Color.LightCyan;
            HighlightColor = Color.LightGoldenrodYellow;
            InnerCoreColor = Color.Lerp(Color.DeepSkyBlue, Color.Black, 0.5f);
            InnerGlowColor = Color.Lerp(Color.LightBlue, Color.Black, 0.5f);
            SparkleColor = Color.DarkBlue;

            TrailTexture = AssetRegistry.Textures.Trails.BulbyTrail.Value;
            NoiseTexture = AssetRegistry.Textures.Noise.Perlin.Value;
        }

        private readonly EffectParameter TransformMatrixParam;
        private readonly EffectParameter TrailTextureParam;
        private readonly EffectParameter NoiseTextureParam;
        private readonly EffectParameter TimeParam;
        private readonly EffectParameter OffsetParam;
        private readonly EffectParameter TilingParam;
        private readonly EffectParameter DistortionParam;
        private readonly EffectParameter BaseColorParam;
        private readonly EffectParameter HighlightColorParam;
        private readonly EffectParameter InnerGlowColorParam;
        private readonly EffectParameter InnerCoreColorParam;
        private readonly EffectParameter SparkleColorParam;

        public Matrix TransformMatrix { get; set; }
        public Texture2D TrailTexture { get; set; }
        public Texture2D NoiseTexture { get; set; }
        public float Time { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Tiling { get; set; }
        public float Distortion { get; set; }
        public Color BaseColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color InnerGlowColor { get; set; }
        public Color InnerCoreColor { get; set; }
        public Color SparkleColor { get; set; }
        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            Time += 0.12f;
            //Set Defaults
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            TransformMatrixParam.SetValue(TransformMatrix);
            TrailTextureParam.SetValue(TrailTexture);
            NoiseTextureParam.SetValue(NoiseTexture);
            TimeParam.SetValue(Time);
            OffsetParam.SetValue(Offset);
            TilingParam.SetValue(Tiling);
            DistortionParam.SetValue(Distortion);
            BaseColorParam.SetValue(BaseColor.ToVector4());
            HighlightColorParam.SetValue(HighlightColor.ToVector4());
            InnerGlowColorParam.SetValue(InnerGlowColor.ToVector4());
            InnerCoreColorParam.SetValue(InnerCoreColor.ToVector4());
            SparkleColorParam.SetValue(SparkleColor.ToVector4());
        }
    }
}
