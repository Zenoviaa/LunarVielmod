using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Stellamod.Core.Utilities;

public static class ContentHelper
{
    public static T[] CreateAllContentOfType<T>(Mod mod) where T : class
    {
        List<T> types = new();
        Type[] loadablesTypes = AssemblyManager.GetLoadableTypes(mod.Code);

        foreach (Type type in loadablesTypes.Where(t => t.IsClass && t.IsSubclassOf(typeof(T)) && !t.IsAbstract))
        {
            T overlayType = Activator.CreateInstance(type) as T;
            types.Add(overlayType);
        }

        return types.ToArray();
    }
}
