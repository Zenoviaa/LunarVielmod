using System;
using System.Collections.Generic;
using System.Linq;

namespace Stellamod.Core.Utilities;

public static class ReflectionHelper
{
    public static IEnumerable<T> GetEnumerableOfInterface<T>(params object[] constructorArgs)
    {
        List<T> objects = new List<T>();
        var classTypesImplementingInterface = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
            x.GetTypes())
            .Where(mytype => typeof(T).IsAssignableFrom(mytype) && mytype.GetInterfaces().Contains(typeof(T)))
            .Where(mytype => mytype.IsClass && !mytype.IsAbstract);
        foreach (Type type in classTypesImplementingInterface)
        {
            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
        }

        return objects;
    }
}
