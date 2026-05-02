using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Common.Shaders;

public static class CustomBlendStates
{

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