using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;

namespace Stellamod.Core.Effects
{
    internal class MoonSparkleShader : Shader
    {
        public MoonSparkleShader()
        {
            StartColorParam = Effect.Parameters["StartColor"];
            GlowColorParam = Effect.Parameters["GlowColor"];
            NoiseTextureParam = Effect.Parameters["NoiseTexture"];
            TilingParam = Effect.Parameters["Tiling"];
            OffsetParam = Effect.Parameters["Offset"];

            StartColor = Color.LightCyan;
            GlowColor = Color.Yellow;
            NoiseTexture = AssetRegistry.Textures.Noise.Perlin;
            Tiling = Vector2.One;
            Offset = Vector2.Zero;
        }

        private readonly EffectParameter StartColorParam;
        private readonly EffectParameter GlowColorParam;
        private readonly EffectParameter NoiseTextureParam;
        private readonly EffectParameter TilingParam;
        private readonly EffectParameter OffsetParam;
        public Color StartColor { get; set; }
        public Color GlowColor { get; set; }
        public Asset<Texture2D> NoiseTexture;
        public Vector2 Tiling { get; set; }
        public Vector2 Offset { get; set; }
        public override void ApplyToEffect()
        {
            base.ApplyToEffect();
            Offset += new Vector2(-0.012f, 0f);
            StartColorParam.SetValue(StartColor.ToVector4());
            GlowColorParam.SetValue(GlowColor.ToVector4());
            NoiseTextureParam.SetValue(NoiseTexture.Value);
            TilingParam.SetValue(Tiling);
            OffsetParam.SetValue(Offset);
        }


    }
}
