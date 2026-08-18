using Terraria;

namespace Stellamod.Core.Utilities;

public static class TileUtilities
{
    /// <summary>
    /// Attempts to find the center of a closed spaced by averaging the nearest tiles on the left, right, top and bottom. Not guaranteed to work with complex shapes
    /// </summary>
    /// <param name="centerOfClosedSpace"></param>
    /// <returns></returns>
    public static Vector2 GuessArenaCenter(Vector2 centerOfClosedSpace)
    {
        Point playerPoint = centerOfClosedSpace.ToTileCoordinates();
        Point right = playerPoint;
        Point left = playerPoint;
        for (int x = 0; x < 100; x++)
        {
            right = playerPoint + new Point(x, 0);
            if (!WorldGen.InWorld(right.X, right.Y))
                break;
            Tile tile = Main.tile[right];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
        }

        for (int x = 0; x < 100; x++)
        {
            left = playerPoint - new Point(x, 0);
            if (!WorldGen.InWorld(left.X, left.Y))
                break;
            Tile tile = Main.tile[left];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
        }
        Point up = playerPoint;
        Point down = playerPoint;
        for (int y = 0; y < 100; y++)
        {
            down = playerPoint + new Point(0, y);
            if (!WorldGen.InWorld(down.X, down.Y))
                break;
            Tile tile = Main.tile[down];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
        }
        for (int x = 0; x < 100; x++)
        {
            up = playerPoint - new Point(0, x);
            if (!WorldGen.InWorld(up.X, up.Y))
                break;
            Tile tile = Main.tile[up];
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                break;
        }


        Point midHorizontal = left + right;
        midHorizontal.X /= 2;

        Point midVertical = up + down;
        midVertical.Y /= 2;

        return new Point(midHorizontal.X, midVertical.Y).ToWorldCoordinates();

    }
    public static void FastPlaceTile(int x, int y, ushort tileType)
    {
        Tile tile = Main.tile[x, y];
        tile.ClearTile();
        tile.TileType = tileType;
        tile.HasTile = true;

        //-1 means it'll get framed later I think.
        tile.TileFrameX = -1;
        tile.TileFrameY = -1;

        //Unsure if this is needed?
        //WorldGen.SquareTileFrame(x, y);
    }
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

    public static Rectangle Clamp(Rectangle rectangle)
    {
        rectangle.X = (int)MathHelper.Clamp(rectangle.X, 0, Main.maxTilesX - 1);
        rectangle.Y = (int)MathHelper.Clamp(rectangle.Y, 0, Main.maxTilesY - 1);

        int maxRight = (int)MathHelper.Clamp(rectangle.X + rectangle.Width, 0, Main.maxTilesX - 1);
        int maxWidth = maxRight - rectangle.Left;
        rectangle.Width = (int)MathHelper.Min(rectangle.Width, maxWidth);

        int maxBottom = (int)MathHelper.Clamp(rectangle.Y + rectangle.Height, 0, Main.maxTilesY - 1);
        int maxHeight = maxBottom - rectangle.Top;
        rectangle.Height = (int)MathHelper.Min(rectangle.Height, maxHeight);
        return rectangle;
    }

    public static Point Clamp(Point tilePoint)
    {
        if (tilePoint.X < 0)
            tilePoint.X = 0;
        if (tilePoint.Y < 0)
            tilePoint.Y = 0;
        if (tilePoint.X >= Main.maxTilesX)
            tilePoint.X = Main.maxTilesX - 1;
        if (tilePoint.Y >= Main.maxTilesY)
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
    public static (Point topLeft, Point bottomRight) CenterTileBounds(Vector2 centerWorld, int width, int height)
    {
        Vector2 cameraTopLeft = centerWorld - new Vector2(width, height) / 2;
        Vector2 cameraBottomRight = centerWorld + new Vector2(width, height) / 2;

        
        Point topLeftTile = cameraTopLeft.ToTileCoordinates();
        Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

        topLeftTile = Clamp(topLeftTile);
        bottomRightTile = Clamp(bottomRightTile);
        return (topLeftTile, bottomRightTile);
    }
    public static (Point topLeft, Point bottomRight) CenterTileBoundsTileSpace(Vector2 centerWorld, int width, int height)
    {
        Point p = centerWorld.ToTileCoordinates();
        Point topLeftTile = p - new Point(width / 2, height/ 2);
        Point bottomRightTile = p + new Point(width / 2, height / 2);

        topLeftTile = Clamp(topLeftTile);
        bottomRightTile = Clamp(bottomRightTile);
        return (topLeftTile, bottomRightTile);
    }
}
