using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Stellamod.WorldG;

public static class BitMaskUtilities
{

    public static void SetBitMask(ref ulong[] _bitMask, int chunkIndex, byte bitIndex, bool state)
    {
        byte indexToSetTo = state ? (byte)1 : (byte)0;
        ulong chunk = _bitMask[chunkIndex];
        ulong mask = (ulong)(indexToSetTo << bitIndex);
        chunk |= mask;
        _bitMask[chunkIndex] = chunk;
    }

    public static bool GetBitMask(ref ulong[] _bitMask, int chunkIndex, byte bitIndex)
    {
        ulong chunk = _bitMask[chunkIndex];
        return (chunk & (1U << bitIndex)) == 1;
    }

}

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
