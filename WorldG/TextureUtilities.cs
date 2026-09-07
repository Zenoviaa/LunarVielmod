namespace Stellamod.WorldG;

/// <summary>
/// Collection of helper functions for manipulating textures.
/// </summary>
public static class TextureUtilities
{
    public static int GetPixelIndex(Texture2D texture, int x, int y)
    {
        return x + y * texture.Width;
    }

    public static Color GetPixelColor(Texture2D texture, int x, int y, Color[] pixels)
    {
        return pixels[GetPixelIndex(texture, x, y)];
    }
}
