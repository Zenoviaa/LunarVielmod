using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WitchySystem;

[Autoload(Side = ModSide.Client)]
public class OffsetMinimapWithDisplaySet : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Main.GetInfoAccIconPosition += GetOffsetPosition;
        On_Main.UpdateMinimapAnchors += OffsetMinimapDown;
    }

    private void GetOffsetPosition(On_Main.orig_GetInfoAccIconPosition orig, int drawnCount, int StartX, out int X, out int Y)
    {
        orig(drawnCount, StartX, out X, out Y);
        Y += 160;
    }

    private void OffsetMinimapDown(On_Main.orig_UpdateMinimapAnchors orig)
    {
        orig();
        Main._minimapTopRightAnchorOffsetTowardsBottom += 160;
    }
}
