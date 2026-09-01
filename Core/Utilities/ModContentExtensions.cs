using Stellamod.Core.ZTileSystem;
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
