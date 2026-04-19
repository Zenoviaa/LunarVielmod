using Stellamod.Core.Foreground;
using Terraria;

namespace Stellamod.Backgrounds;

public class UndergroundSandCity : ForegroundLayer
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        tilingInBothAxes = true;
    }
    public override bool IsActive()
    {
        return Main.LocalPlayer.ZoneUndergroundDesert && !Main.LocalPlayer.ZoneOverworldHeight;
    }
    public override void SetLayering(ref float zLayer, ref Vector2 parallax)
    {
        base.SetLayering(ref zLayer, ref parallax);
        parallax.X = 1.2f;
        parallax.Y = 1.2f;
    }
}
