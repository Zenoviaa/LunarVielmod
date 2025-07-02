using System;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    internal static class AssetHelper
    {
        public static string RootName => "Stellamod";
        public static string EmptyTexture => RootName + "/Assets/Textures/Empty";
        public static string DirectoryHere(this Type type)
        {
            string Texture = (type.Namespace).Replace('.', '/');
            return Texture;
        }

        public static string MyDirectory(this object obj)
        {
            string path = (obj.GetType().Namespace).Replace('.', '/');
            return path;
        }

        public static string PathHere(this Type type)
        {
            string Texture = (type.Namespace + "." + type.Name).Replace('.', '/');
            return Texture;
        }
    }
}
