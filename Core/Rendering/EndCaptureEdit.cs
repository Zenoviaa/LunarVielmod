using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class EndCaptureEdit : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_FilterManager.CanCapture += ForceCapture;
    }

    private bool ForceCapture(On_FilterManager.orig_CanCapture orig, FilterManager self)
    {
        if (Filters.Scene._activeFilterCount <= 0)
            Filters.Scene._activeFilterCount = 1;

        return true;
    }
}
