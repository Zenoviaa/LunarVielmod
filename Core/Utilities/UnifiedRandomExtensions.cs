using System;
using System.Collections.Generic;
using Terraria.Utilities;

namespace Stellamod.Core.Utilities;

public static class UnifiedRandomExtensions
{
    public static void Shuffle<T>(this UnifiedRandom random, T[] array)
    {
        for (int i = 0; i < array.Length - 1; ++i)
        {
            int r = random.Next(i, array.Length);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }

    public static void Shuffle<T>(T[] array, UnifiedRandom random)
    {
        for (int i = 0; i < array.Length - 1; ++i)
        {
            int r = random.Next(i, array.Length);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }
    public static T NextElement<T>(this IList<T> collection, UnifiedRandom rand)
    {
        return collection[rand.Next(0, collection.Count)];
    }
}
