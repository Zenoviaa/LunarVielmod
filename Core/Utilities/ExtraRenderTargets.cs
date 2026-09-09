using Stellamod.Core.Rendering;
using Terraria;

namespace Stellamod.Core.Utilities;

public class ExtraRenderTargets
{
    public static readonly RenderTargetProvider TileTargetSwap = new RenderTargetProvider(() =>
    {
        return new RenderTargetParameters
        {
            Width = Main.instance.tileTarget.Width,
            Height = Main.instance.tileTarget.Height,
        };
    });
}
