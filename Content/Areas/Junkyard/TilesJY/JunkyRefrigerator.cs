using Stellamod.Core.ZTileSystem;

namespace Stellamod.Content.Areas.Junkyard.TilesJY;

public class JunkyRefrigerator : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}