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
                BaseColor = Color.White,
                HighlightColor = Color.Cyan,
                RimHighlightColor = Color.Blue,
                WindColor = Color.Violet,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
              //  WindTexture = TrailRegistry.CausticTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 26;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Blue, Color.Violet, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, interpolant);
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
    }
}
