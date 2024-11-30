using System;
using System.Collections.Generic;

namespace Stellamod.Helpers
{
    public static class GeneralHelpers
    {
        public static List<T> GetEnumList<T>()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            List<T> list = new List<T>(array);
            return list;
        }
    }
}
