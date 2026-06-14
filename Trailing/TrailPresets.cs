using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Effects;
using Stellamod.Core.Effects.Trails;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Trailing;

public struct SlashTrailBuilder
{
    public SlashTrailBuilder()
    {
        //Set Defaults
        baseColor = Color.DarkGray;
        windColor = Color.LightGray;
        lightColor = Color.White;
        rimHighlightColor = Color.LightGray;
        widthFunction = GetTrailWidthDefault;
        colorFunction = GetTrailColorDefault;
    }

    public Color baseColor;
    public Color windColor;
    public Color lightColor;
    public Color rimHighlightColor;
    public ITrailer.GetTrailWidth widthFunction;
    public ITrailer.GetTrailColor colorFunction;
    public SlashTrailer Instantiate()
    {
        return new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = baseColor,
                WindColor = windColor,
                LightColor = lightColor,
                RimHighlightColor = rimHighlightColor,
                BlendState = BlendState.Additive
            },

            TrailColorFunction = colorFunction,
            TrailWidthFunction = widthFunction
        };
    }

    private float GetTrailWidthDefault(float completionRatio)
    {
        return MathHelper.SmoothStep(10, 0, completionRatio);
    }

    private Color GetTrailColorDefault(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.OutCirc(completionRatio));
    }
    //public static implicit operator ITrailer(SlashTrailBuilder builder) => builder.Instantiate();
    public static implicit operator SlashTrailer(SlashTrailBuilder builder) => builder.Instantiate();
}

public static class TrailPresets
{
    public static SlashTrailer CreateIvynSlashTrail()
    {
        var slashEffect = new SlashEffect()
        {
            BaseColor = Color.DarkGreen,
            WindColor = Color.DarkOliveGreen,
            LightColor = Color.SpringGreen,
            RimHighlightColor = Color.White,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive
        };
        var slashTrailer = new SlashTrailer();
        slashTrailer.Shader = slashEffect;
        return slashTrailer;
    }
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

    public static SlashTrailer Chillrend => new SlashTrailer
    {
        Shader = new SlashEffect()
        {
            BaseColor = Color.LightGray,
            HighlightColor = Color.White,
            RimHighlightColor = Color.White,
            WindColor = Color.LightGray,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
            WindTexture = TrailRegistry.StarTrail.Value
        },
        TrailWidthFunction = (float interpolant) =>
        {
            return EasingFunction.QuadraticBump(interpolant) * 16;
        },
        TrailColorFunction = (float interpolant) =>
        {
            Color lerp1 = Color.Lerp(Color.White, Color.LightGray, interpolant);
            return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
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

    public static SlashTrailer Miracle => new SlashTrailer
    {
        Shader = new SlashEffect()
        {
            BaseColor = Color.Blue,
            HighlightColor = Color.Purple,
            RimHighlightColor = Color.DarkViolet,
            WindColor = Color.DarkViolet,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
            WindTexture = TrailRegistry.WhispyTrail.Value
        },
        TrailWidthFunction = (float interpolant) =>
        {
            return EasingFunction.QuadraticBump(interpolant) * 7;
        },
        TrailColorFunction = (float interpolant) =>
        {
            Color lerp1 = Color.Lerp(Color.Blue, Color.Purple, interpolant);
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
            return EasingFunction.QuadraticBump(interpolant) * 14;
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

    public static SlashTrailer WarriorsGraceTrailing()
    {
        SlashTrailer slashTrailer = new SlashTrailer();
        slashTrailer.Shader = new SlashEffect()
        {
            BaseColor = Color.DarkGray,
            HighlightColor = Color.White,
            RimHighlightColor = Color.LightGray,
            WindColor = Color.LightSlateGray,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
            WindTexture = TrailRegistry.CrystalTrail.Value
        };
        slashTrailer.TrailWidthFunction = (float interpolant) =>
        {
            return EasingFunction.QuadraticBump(interpolant) * 16;
        };
        slashTrailer.TrailColorFunction = (float interpolant) =>
        {
            Color lerp1 = Color.Lerp(Color.White, Color.Black, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant);
        };
        return slashTrailer;
    }

    public static SlashTrailer Auroran => new SlashTrailer
    {
        Shader = new SlashEffect()
        {
            BaseColor = Color.LightGreen,
            HighlightColor = Color.White,
            RimHighlightColor = Color.LightPink,
            WindColor = Color.SkyBlue,
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
            return Color.Lerp(Color.Transparent, lerp1, interpolant);
        }

    }; 
    public static SlashTrailer Starvast => new SlashTrailer
    {
        Shader = new SlashEffect()
        {
            BaseColor = Color.Pink,
            HighlightColor = Color.LightGoldenrodYellow,
            RimHighlightColor = Color.Pink,
            WindColor = Color.Blue,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
            WindTexture = TrailRegistry.StarTrail.Value
        },
        TrailWidthFunction = (float interpolant) =>
        {
            return EasingFunction.QuadraticBump(interpolant) * 8;
        },
        TrailColorFunction = (float interpolant) =>
        {
            Color lerp1 = Color.Lerp(Color.LightGoldenrodYellow, Color.Cyan, interpolant);
            return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
        }

    };

    public static SlashTrailer XScissor => new SlashTrailer
    {
        Shader = new SlashEffect()
        {
            BaseColor = Color.LightGreen,
            HighlightColor = Color.White,
            RimHighlightColor = Color.LightPink,
            WindColor = Color.SkyBlue,
            BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
            WindTexture = TrailRegistry.CrystalTrail.Value
        },
        TrailWidthFunction = (float interpolant) =>
        {
            return MathHelper.SmoothStep(16, 0, interpolant);
        },
        TrailColorFunction = (float interpolant) =>
        {
            Color lerp1 = Color.Lerp(Color.LightCyan, Color.Purple, interpolant);
            return Color.Lerp(Color.Transparent, lerp1, interpolant);
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
