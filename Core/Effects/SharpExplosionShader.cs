using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Effects
{
    internal class SharpExplosionShader : Shader
    {
        public SharpExplosionShader() : base()
        {
            NoiseTextureParam = Effect.Parameters["NoiseTexture"];
            InterpolantParam = Effect.Parameters["Interpolant"];
            DistortionParam = Effect.Parameters["Distortion"];
            TilingParam = Effect.Parameters["Tiling"];
            InnerColorParam = Effect.Parameters["InnerColor"];
            GlowColorParam = Effect.Parameters["GlowColor"];
            UnderToneColorParam = Effect.Parameters["UnderToneColor"];
        }

        private readonly EffectParameter NoiseTextureParam;
        private readonly EffectParameter InterpolantParam;
        private readonly EffectParameter DistortionParam;
        private readonly EffectParameter TilingParam;
        private readonly EffectParameter InnerColorParam;
        private readonly EffectParameter GlowColorParam;
        private readonly EffectParameter UnderToneColorParam;

        public Texture2D NoiseTexture { get; set; }
        public float Interpolant { get; set; }
        public float Distortion { get; set; }
        public Vector2 Tiling { get; set; }
        public Color InnerColor { get; set; }
        public Color GlowColor { get; set; }
        public Color UnderToneColor { get; set; }
        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            NoiseTextureParam.SetValue(NoiseTexture);
            InterpolantParam.SetValue(Interpolant);
            DistortionParam.SetValue(Distortion);
            TilingParam.SetValue(Tiling);
            InnerColorParam.SetValue(InnerColor.ToVector3());
            GlowColorParam.SetValue(GlowColor.ToVector3());
            UnderToneColorParam.SetValue(UnderToneColor.ToVector3());
        }
    }
}
