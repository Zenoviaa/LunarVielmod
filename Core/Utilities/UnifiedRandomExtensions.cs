using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Utilities;

namespace Stellamod.Core.Utilities;

public static class UnifiedRandomExtensions
{
    public static T NextElement<T>(this IList<T> collection, UnifiedRandom rand)
    {
        return collection[rand.Next(0, collection.Count)];
    }
}
