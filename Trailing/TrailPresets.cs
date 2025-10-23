using Microsoft.Xna.Framework;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Trailing
{
    public static class TrailPresets
    {
        public static SlashTrailer FlamingTrail => new SlashTrailer
        {
            Shader = new FlamingTrailShader()
            {
                
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                return Color.Lerp(Color.White, Color.Transparent, interpolant);
            }

        };
        public static SlashTrailer HypnoticScythe => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Pink,
                HighlightColor = Color.Cyan,
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
        public static SlashTrailer CinderBreaker => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Yellow,
                HighlightColor = Color.Orange,
                RimHighlightColor = Color.Red,
                WindColor = Color.DarkRed,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.WhispyTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 7;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Yellow, Color.Red, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };
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

        public static SlashTrailer MooningSlicer => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.LightBlue,
                HighlightColor = Color.White,
                RimHighlightColor = Color.LightCyan,
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
                Color lerp1 = Color.Lerp(Color.LightCyan, Color.Cyan, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };
        public static SlashTrailer Auroran => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.LightBlue,
                HighlightColor = Color.White,
                RimHighlightColor = Color.Purple,
                WindColor = Color.Blue,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.CrystalTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.LightCyan, Color.Purple, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        public static SlashTrailer LightSpand => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Orange,
                HighlightColor = Color.White,
                RimHighlightColor = Color.Red,
                WindColor = Color.DarkOrange,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.LightningTrail2.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Yellow, Color.Orange, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        public static SlashTrailer Assassin => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Red,
                HighlightColor = Color.White,
                RimHighlightColor = Color.Red,
                WindColor = Color.DarkRed,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend,
                WindTexture = TrailRegistry.LightningTrail2.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 5;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.Red, Color.Black, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };
        public static SlashTrailer Sirius => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.White,
                HighlightColor = Color.Cyan,
                RimHighlightColor = Color.DarkBlue,
                WindColor = Color.DarkViolet,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.LightningTrail2.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 5;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Blue, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        public static SlashTrailer GladiatorSpear => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Orange,
                HighlightColor = Color.White,
                RimHighlightColor = Color.DarkRed,
                WindColor = Color.DarkGray,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.LightningTrail2.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 5;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Color.White, Color.Orange, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        public static SlashTrailer InkingSpire => new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.Yellow,
                HighlightColor = Color.Red,
                RimHighlightColor = Color.Orange,
                WindColor = Color.Blue,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.CrystalTrail.Value
            },
            TrailWidthFunction = (float interpolant) =>
            {
                return EasingFunction.QuadraticBump(interpolant) * 16;
            },
            TrailColorFunction = (float interpolant) =>
            {
                Color lerp1 = Color.Lerp(Main.DiscoColor, Main.DiscoColor, interpolant);
                return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
            }

        };

        public static SlashTrailer HypnotizedSoul => new SlashTrailer
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
                return MathHelper.Lerp(16, 8, EasingFunction.InOutSine(interpolant));
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
