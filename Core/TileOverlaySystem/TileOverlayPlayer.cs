using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.TileOverlaySystem;

public class TileOverlayPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        if (Main.netMode == NetmodeID.Server)
            return;
 
        TileOverlayUtility.RequestTileOverlayData();
    }
}
