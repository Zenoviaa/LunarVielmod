using System;

namespace Stellamod.Core.Utilities
{
    public static class FileUtils
    {
        public static string GetTypeDirectory(this object obj)
        {
            Type type = obj.GetType();
            return (type.Namespace).Replace('.', '/');
        }
        public static string GetTypeDirectoryWithSlash(this object obj)
        {
            Type type = obj.GetType();
            return (type.Namespace + ".").Replace('.', '/');
        }
        public static string GetTypeFileName(this object obj)
        {
            Type type = obj.GetType();
            return (type.Namespace + "." + type.Name).Replace('.', '/');
        }
        /*
        extension(object obj)
        {
            public string Directory
            {
                get
                {
                    Type type = obj.GetType();
                    return (type.Namespace).Replace('.', '/');
                }
            }

            public string DirectoryWithSlash
            {
                get
                {
                    Type type = obj.GetType();
                    return (type.Namespace + ".").Replace('.', '/');
                }
            }


            public string FileName
            {
                get
                {
                    Type type = obj.GetType();
                    return (type.Namespace + "." + type.Name).Replace('.', '/');
                }
            }
        }*/
    }
}
