using Terraria;

namespace Stellamod.Core.Utilities;

public static class VectorExtensions
{
    public static void ClearForTrailing(this Vector2[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = new Vector2(-9999);
        }
    }
    public static void PushAndPopOffEnd<T>(this T[] arr, T newElement)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            arr[i] = arr[i - 1];
        }
        arr[0] = newElement;
    }
    public static Vector2 Resize(this Vector2 vector, float newLength)
    {
        return vector.SafeNormalize(Vector2.Zero) * newLength;
    }
}
