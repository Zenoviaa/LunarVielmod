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
    public static bool[] ThickSnow;
    public static bool[] AegisMisty;

    /// <summary>
    /// When set to 0, does nothing
    /// When set to 1, renders white fog if the tile is solid
    /// When set to 2, renders red fog if the tile is solid
    /// </summary>
    public static int[] BarrierFog;
    public override void ResizeArrays()
    {
        base.ResizeArrays();
        AegisMisty = TileID.Sets.Factory.CreateBoolSet();
        ThickSnow = TileID.Sets.Factory.CreateBoolSet();
        BarrierFog = TileID.Sets.Factory.CreateIntSet();
    }
}

public static class TileSetsExtensions
{
    extension(TileID.Sets)
    {
        public static int[] BarrierFog => TileSets.BarrierFog;
    }
}