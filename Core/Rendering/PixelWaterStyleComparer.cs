using System.Collections.Generic;

namespace Stellamod.Core.Rendering;

public class PixelWaterStyleComparer : IComparer<PixelWaterStyle>
{
    public int Compare(PixelWaterStyle x, PixelWaterStyle y)
    {
        return y.priority.CompareTo(x.priority);
    }
}
