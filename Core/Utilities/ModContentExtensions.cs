using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public static class ModContentExtensions
{
    extension(ModContent)
    {
        public static ushort ZTileType<T1>() where T1 : ZTile
        {
            return ModContent.GetInstance<T1>().type;
        }
    }
}
