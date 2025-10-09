using Microsoft.Xna.Framework;
using Stellamod.Core.Effects.Trails;
using Stellamod.Helpers;
using Stellamod.Trails;

namespace Stellamod.Trailing
{
    public static class TrailPresets
    {
        public static SlashTrailer Starvast => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Pink,
                HighlightColor = Color.Pink,
                RimHighlightColor = Color.Pink,
                WindColor = Color.Blue,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                         WindTexture = TrailRegistry.StarTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Pink, Color.Cyan, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        }; 
        
        public static SlashTrailer Starvast2 => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.White,
                HighlightColor = Color.Cyan,
                RimHighlightColor = Color.Blue,
                WindColor = Color.Violet,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.StarTrail.Value
                //  WindTexture = TrailRegistry.CausticTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 32;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Yellow, Color.Violet, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, interpolant);
            }

        }; 
        
        public static SlashTrailer StarringBalls => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Purple,
                HighlightColor = Color.Red,
                RimHighlightColor = Color.LightCyan,
                WindColor = Color.Pink,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                HighlightTexture = TrailRegistry.CrystalTrail.Value,
                WindTexture = TrailRegistry.CrystalTrail.Value,
                Tiling = Vector2.One * 0.3f
                //  WindTexture = TrailRegistry.CausticTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return MathHelper.Lerp(10, 4, EasingFunction.InOutSine(interpolant));
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Violet, Color.Blue, interpolant);
                lerp1 = Color.Lerp(lerp1, Color.Pink, ExtraMath.Osc(0f, 1f, speed: 13));
                return Color.Lerp(Color.Transparent, lerp1, EasingFunction.QuadraticBump(interpolant)) * (1f - interpolant);
            }

        };
    }
}
