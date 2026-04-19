using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Common.UI;

public static class UIHelpers
{
    public static Vector2 ScreenOffset(Vector2 dimensions, Vector2 normalizedOrigin, Vector2 offset, Vector2? parentDimensions = null)
    {
        if(parentDimensions == null)
        {
            parentDimensions = new Vector2(Main.screenWidth, Main.screenHeight);
        }
        float relativeLeft = parentDimensions.Value.X * normalizedOrigin.X - (dimensions.X / 2) + offset.X;
        float relativeTop = parentDimensions.Value.Y * normalizedOrigin.Y - (dimensions.Y / 2) + offset.Y;
        return new Vector2(relativeLeft, relativeTop);
    }
}
