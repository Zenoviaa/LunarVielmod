using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.Drawing;

namespace Stellamod.Core.Rendering;

public static class WaterHelpers
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "DrawWaters")]
    public static extern void DrawWaters(Main instance, bool isBackground);


    //GetScreenDrawArea(unscaledPosition, vector + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetScreenDrawArea")]
    public static extern void GetScreenDrawArea(TileDrawing tileDrawing, Vector2 unscaledPosition, Vector2 offset, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY);


}
