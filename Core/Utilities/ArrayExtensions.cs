using System.Runtime.CompilerServices;
using Terraria;

namespace Stellamod.Core.Utilities;

public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int InterpolantToIndex(in float interpolant, in int length)
    {
        int index = (int)(interpolant * length);
        return index;
    }
    public static int InterpolantToIndexClamped(in float interpolant, in int length)
    {
        int index = (int)(interpolant * length);
        index = MathHelper.Clamp(index, 0, length - 1);
        return index;
    }

    public static T InterpolateArrayWrapped<T>(this T[] arr, float interpolant)
    {
        int index = InterpolantToIndex(interpolant, arr.Length) % arr.Length;
        T spawnPos = arr[index];
        return spawnPos;
    }

    public static T InterpolateArrayClamped<T>(this T[] arr, float interpolant)
    {
        int index = InterpolantToIndex(interpolant, arr.Length);
        index = MathHelper.Clamp(index, 0, arr.Length - 1);
        T spawnPos = arr[index];
        return spawnPos;
    }
    
    public static Vector2 DirectionToNextPoint(this Vector2[] arr, float interpolant, in int increment = 4)
    {
        int index = InterpolantToIndexClamped(interpolant, arr.Length);
        return DirectionToNextPoint(arr, index);
    }

    public static Vector2 DirectionToNextPoint(this Vector2[] arr, int index, in int increment = 4)
    {
        int nextIndex = index  + increment;
        nextIndex = MathHelper.Clamp(nextIndex, 0, arr.Length - 1);
        Vector2 current = arr[index];
        Vector2 next = arr[nextIndex];
        Vector2 diff = next - current;
        diff = diff.SafeNormalize(Vector2.Zero);
        return diff;
    }
}
