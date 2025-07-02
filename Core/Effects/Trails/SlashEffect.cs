using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Terraria;

namespace Stellamod.Core.Effects.Trails
{
    internal class SlashTrailer : TrailDrawer
    {
        public SlashTrailer()
        {
            Shader = new SlashEffect();
        }
    }

    internal class SlashEffect : Shader
    {
        /*
         *  matrix transformMatrix;
            texture trailTexture;
            texture HighlightTexture;
            texture WindTexture;
            texture RimHighlightTexture;
            float2 Offset;
            float2 Tiling;

            float4 BaseColor;
            float4 HighlightColor;
            float4 RimHighlightColor;
            float4 WindColor;
         */
        public SlashEffect()
        {
            TransformMatrixParam = Effect.Parameters["transformMatrix"];
            TrailTextureParam = Effect.Parameters["trailTexture"];
            HighlightTextureParam = Effect.Parameters["HighlightTexture"];
            WindTextureParam = Effect.Parameters["WindTexture"];
            RimHighlightTextureParam = Effect.Parameters["RimHighlightTexture"];
            OffsetParam = Effect.Parameters["Offset"];
            TilingParam = Effect.Parameters["Tiling"];
            BaseColorParam = Effect.Parameters["BaseColor"];
            HighlightColorParam = Effect.Parameters["HighlightColor"];
            RimHighlightColorParam = Effect.Parameters["RimHighlightColor"];
            WindColorParam = Effect.Parameters["WindColor"];

            Offset = Vector2.Zero;
            Tiling = Vector2.One;
            BaseColor = Color.Lerp(Color.Cyan, Color.White, 0.5f);
            HighlightColor = Color.LightCyan;
            RimHighlightColor = Color.White;
            WindColor = Color.Blue * 0.5f;
            TrailTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide1.Value;
            HighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide2.Value;
            WindTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide3.Value;
            RimHighlightTexture = AssetRegistry.Textures.Trails.BasicSlash_Wide4.Value;
        }

        private readonly EffectParameter TransformMatrixParam;
        private readonly EffectParameter TrailTextureParam;
        private readonly EffectParameter HighlightTextureParam;
        private readonly EffectParameter WindTextureParam;
        private readonly EffectParameter RimHighlightTextureParam;
        private readonly EffectParameter OffsetParam;
        private readonly EffectParameter TilingParam;
        private readonly EffectParameter BaseColorParam;
        private readonly EffectParameter HighlightColorParam;
        private readonly EffectParameter RimHighlightColorParam;
        private readonly EffectParameter WindColorParam;

        public Matrix TransformMatrix { get; set; }
        public Texture2D TrailTexture { get; set; }
        public Texture2D HighlightTexture { get; set; }
        public Texture2D WindTexture { get; set; }
        public Texture2D RimHighlightTexture { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Tiling { get; set; }
        public Color BaseColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color RimHighlightColor { get; set; }
        public Color WindColor { get; set; }

        public override void ApplyToEffect()
        {
            base.ApplyToEffect();

            //Set Defaults
            TransformMatrix = TrailDrawer.WorldViewPoint2;
            TransformMatrixParam.SetValue(TransformMatrix);
            TrailTextureParam.SetValue(TrailTexture);
            HighlightTextureParam.SetValue(HighlightTexture);
            WindTextureParam.SetValue(WindTexture);
            RimHighlightTextureParam.SetValue(RimHighlightTexture);
            OffsetParam.SetValue(Offset);
            TilingParam.SetValue(Tiling);


            Color baseColor = BaseColor;
            Color highlightColor = HighlightColor;
            Color rimHighlightColor = RimHighlightColor;
            Color windColor = WindColor;

            baseColor = baseColor.MultiplyRGB(LightColor);
            highlightColor = highlightColor.MultiplyRGB(LightColor);
            rimHighlightColor = rimHighlightColor.MultiplyRGB(LightColor);
            windColor = windColor.MultiplyRGB(LightColor);

            BaseColorParam.SetValue(baseColor.ToVector4());
            HighlightColorParam.SetValue(highlightColor.ToVector4());
            RimHighlightColorParam.SetValue(rimHighlightColor.ToVector4());
            WindColorParam.SetValue(windColor.ToVector4());
        }
    }
}
