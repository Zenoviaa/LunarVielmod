using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class TileSets : ModSystem
{
    public static bool[] BlockMineshafts = TileID.Sets.Factory.CreateBoolSet();
    public static bool[] Collectible = TileID.Sets.Factory.CreateBoolSet();
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BlockMineshafts[TileID.SnowBlock] = true;
        BlockMineshafts[TileID.Sand] = true;
        BlockMineshafts[TileID.Mud] = true;
        BlockMineshafts[TileID.IceBlock] = true;
    }
}
