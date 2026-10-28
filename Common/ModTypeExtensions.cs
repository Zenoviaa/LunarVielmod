using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Common;

public static class ModTypeExtensions
{
    extension(ModType)
    {
        public static string NoTexture => TextureRegistry.EmptyTexture;
    }
}
