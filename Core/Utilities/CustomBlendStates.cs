using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Utilities;

public static class CustomBlendStates
{
    public static readonly BlendState Brightest = new BlendState
    {
        AlphaSourceBlend = Blend.DestinationAlpha,
        ColorSourceBlend = Blend.DestinationColor,
        AlphaDestinationBlend = Blend.One,
        ColorDestinationBlend = Blend.One,
        AlphaBlendFunction = BlendFunction.Max,
        ColorBlendFunction = BlendFunction.Max
    };

    public static readonly BlendState Multiply = new BlendState
    {
        AlphaSourceBlend = Blend.DestinationAlpha,
        AlphaDestinationBlend = Blend.Zero,
        AlphaBlendFunction = BlendFunction.Add,
        ColorSourceBlend = Blend.DestinationColor,
        ColorDestinationBlend = Blend.Zero,
        ColorBlendFunction = BlendFunction.Add
    };

    public static readonly BlendState Subtract = new BlendState
    {
        AlphaSourceBlend = Blend.DestinationAlpha,
        AlphaDestinationBlend = Blend.Zero,
        AlphaBlendFunction = BlendFunction.Add,
        ColorSourceBlend = Blend.DestinationColor,
        ColorDestinationBlend = Blend.Zero,
        ColorBlendFunction = BlendFunction.Subtract
    }; 
    
    public static readonly BlendState MaskSubtract = new BlendState
    {
        AlphaSourceBlend = Blend.DestinationAlpha,
        AlphaDestinationBlend = Blend.Zero,
        AlphaBlendFunction = BlendFunction.ReverseSubtract,
        ColorSourceBlend = Blend.DestinationColor,
        ColorDestinationBlend = Blend.Zero,
        ColorBlendFunction = BlendFunction.Max
    };
}