using Stellamod.Core.ZTileSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public partial class VeilGen
{
    public static void CreateAbyssConnectionCave(Vector2 start, Vector2 end)
    {
        var genRand = WorldGen.genRand;
        float strength = genRand.NextFloat(12, 18);
        float steps = Vector2.Distance(start, end) / 4f;
        for (float f = 0; f < steps; f++)
        {
            float lerp = f / steps;
            Vector2 pos = Vector2.Lerp(start, end, lerp);
            WorldGen.TileRunner((int)pos.X, (int)pos.Y,
                 strength: strength,
                 genRand.Next(5, 12), -1);
        }
    }
    public static void DecorateSurfaceEdgesWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                if (!hasTop && hasBottom)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        instanceData.frameNumber = (ushort)genRand.Next(0, zTileLoader.GetTile(zTileType).frameCount);
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }
    public static void DecorateWetAreasWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileAbove = Main.tile[x, y - 1];
                if (!tile.HasTile)
                    continue;
                if (tileAbove.LiquidAmount <= 0)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                if (!hasTop && hasBottom)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        instanceData.frameNumber = (ushort)genRand.Next(0, zTileLoader.GetTile(zTileType).frameCount);
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }
    public static void DecorateEdgesWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;
                if (hasAny)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }

    public static void ClearWallsArea(Rectangle tileBounds)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.WallType = 0;
            }
        }
    }
    public static void DecorateEdgeTilesWithWalls(Rectangle tileBounds, List<int> targetTileTypes, ushort wallType, int maxWallCaveWidth = 2)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        int wallCaveWidth = maxWallCaveWidth;
        Vector2 baseDirection = -Vector2.UnitY;

        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!targetTileTypes.Contains(tile.TileType))
                    continue;
                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;
                if (hasAny)
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new Point(x, y);
                    int steps = genRand.Next(1, 4);


                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(wallCaveWidth, wallCaveWidth),
                                new Actions.PlaceWall(wallType));
                        }

                        point += (baseDirection * wallCaveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }
    }

    public static void DecorateSurfaceEdgesWithMultiTile(Rectangle tileBounds, int denom, List<int> targetGroundTileTypes, params int[] tileTypes)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;

        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tileBelow = Main.tile[x, y + 1];
                if (!tileBelow.HasTile)
                    continue;
                if (!targetGroundTileTypes.Contains(tileBelow.TileType))
                    continue;
                if (genRand.NextBool(denom))
                    WorldGen.PlaceObject(x, y, tileTypes.NextElement(genRand));
            }
        }
    }
    public static void GenerateWaterBlobs(Rectangle area, float numWaterBlocks, Point squareRange)
    {
        var genRand = WorldGen.genRand;
        List<Point> validPoints = new();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;
                int sy = y;
                int sx = x;
                while (!tile.HasTile && sy < Main.UnderworldLayer)
                {
                    sy++;
                    tile = Main.tile[sx, sy];
                }
                validPoints.Add(new Point(sx, sy));
            }
        }
        if (validPoints.Count <= 0)
            return;

        for (float f = 0; f < numWaterBlocks; f++)
        {
            //Reset the seed for each cave
            Point p = validPoints.NextElement(genRand);
            //Dimensions of the lava bowl
            int width = genRand.Next(squareRange.X, squareRange.Y);
            int left = p.X - width / 2;
            int right = p.X + width / 2;
            int top = p.Y - width / 2;
            int bottom = p.Y + width / 2;
            for (int x = left; x < right; x++)
            {
                for (int y = top; y < bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                        continue;
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Water;
                }
            }
        }
    }
    public static void GenerateWaterBowls(Rectangle area, float numLavaBowls, Point widthRange, Point depthRange)
    {
        var genRand = WorldGen.genRand;
        List<Point> validPoints = new();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;
                int sy = y;
                int sx = x;
                while (!tile.HasTile && sy < Main.UnderworldLayer)
                {
                    sy++;
                    tile = Main.tile[sx, sy];
                }
                validPoints.Add(new Point(sx, sy));
            }
        }
        if (validPoints.Count <= 0)
            return;

        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            Point p = validPoints.NextElement(genRand);
            Tile startTile = Main.tile[p.X, p.Y];

            //Dimensions of the lava bowl
            int width = genRand.Next(widthRange.X, widthRange.Y);
            int depth = genRand.Next(depthRange.X, depthRange.Y);
            int left = p.X - width / 2;
            int right = p.X + width / 2;
            for (int x = left; x < right; x++)
            {
                float numSteps = right - left;
                int d = (int)MathHelper.Lerp(0, depth, EasingFunction.QuadraticBump((x - left) / numSteps));
                for (int y = p.Y; y < p.Y + d; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Water;
                }
            }
        }
    }
    public static void GenerateLavaBowls(Rectangle area, float numLavaBowls, Point widthRange, Point depthRange)
    {
        var genRand = WorldGen.genRand;
        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            int sx = genRand.Next(area.Left, area.Right);
            int sy = genRand.Next(area.Top, area.Bottom);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the lava is inside of a cave/exposed to air
            if (startTile.HasTile)
                continue;

            //Gotta land on a solid tile
            while (!startTile.HasTile && sy < Main.UnderworldLayer)
            {
                sy++;
                startTile = Main.tile[sx, sy];
            }

            //Dimensions of the lava bowl
            int width = genRand.Next(widthRange.X, widthRange.Y);
            int depth = genRand.Next(depthRange.X, depthRange.Y);
            int left = sx - width / 2;
            int right = sx + width / 2;
            for (int x = left; x < right; x++)
            {
                float numSteps = right - left;
                int d = (int)MathHelper.Lerp(0, depth, EasingFunction.QuadraticBump((x - left) / numSteps));
                for (int y = sy; y < sy + d; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Lava;
                }
            }
        }
    }
}
