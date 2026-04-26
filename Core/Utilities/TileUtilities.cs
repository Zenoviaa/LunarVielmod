using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Utilities;

public static class TileUtilities
{
    public static Point FallToSolidTile(Point tile)
    {
        return FallToSolidTile(tile.X, tile.Y);
    }
    public static Point FallToSolidTile(int x, int y, int direction = 1)
    {
        Point start = new Point(x, y);
        Point current = start;
        for (int i = 0; i < Main.maxTilesY; i++)
        {
            if (WorldGen.InWorld(current.X, current.Y) && WorldGen.SolidTile(current.X, current.Y))
                return current;
            current.Y += direction;
        }
        return Point.Zero;
    }

    public static Point Clamp(Point tilePoint)
    {
        if (tilePoint.X < 0)
            tilePoint.X = 0;
        if (tilePoint.Y < 0)
            tilePoint.Y = 0;
        if (tilePoint.X >= Main.maxTilesX)
            tilePoint.X = Main.maxTilesX - 1;
        if(tilePoint.Y >= Main.maxTilesY)
            tilePoint.Y = Main.maxTilesY - 1;
        return tilePoint;
    }

    public static (Point topLeft, Point bottomRight) CameraTileBounds(float fluff)
    {
        Vector2 cameraCenterWorld = Main.Camera.Center;
        Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
        Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

        cameraTopLeft -= new Vector2(fluff);
        cameraBottomRight += new Vector2(fluff);

        Point topLeftTile = cameraTopLeft.ToTileCoordinates();
        Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

        topLeftTile = Clamp(topLeftTile);
        bottomRightTile = Clamp(bottomRightTile);   
        return (topLeftTile, bottomRightTile);  
    } 
}
