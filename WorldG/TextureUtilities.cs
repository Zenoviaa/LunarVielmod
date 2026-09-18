using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stellamod.WorldG;

/// <summary>
/// Collection of helper functions for manipulating textures.
/// </summary>
public static class TextureUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPixelIndex(Texture2D texture, int x, int y)
    {
        return x + y * texture.Width;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPixelIndex(int width, int x, int y)
    {
        return x + y * width;
    }

    public static Color GetPixelColor(Texture2D texture, int x, int y, Color[] pixels)
    {
        return pixels[GetPixelIndex(texture, x, y)];
    }

}
