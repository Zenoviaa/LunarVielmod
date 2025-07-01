using System;

namespace Stellamod.Common.Helpers
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
        public static string PathHere(this Type type)
        {
            string Texture = (type.Namespace + "." + type.Name).Replace('.', '/');
            return Texture;
        }
    }
}
