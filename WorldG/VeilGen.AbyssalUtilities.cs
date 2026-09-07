using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;


namespace Stellamod.WorldG;

public partial class VeilGen
{
    /// <summary>
    /// Generates the entire abyss biome
    /// </summary>
    public static void GenerateAbyss()
    {
        int left = SavedGenerationParameters.SnowLeft;
        int right = SavedGenerationParameters.SnowRight;
        int top = SavedGenerationParameters.SnowTop;
        int bottom = ModContent.GetInstance<StellaWorld>().DarkspaceStart;

        //Calculate center of the abyss
        Point AbyssCenter = new Point();
        AbyssCenter.X = left + right;
        AbyssCenter.X /= 2;
        AbyssCenter.Y = (int)(SavedGenerationParameters.RockLayerHigh + Main.maxTilesY * 0.15);
        AbyssCenter.Y -= 20;
        //Place the center like a circle

        ushort abyssTile = (ushort)ModContent.TileType<AbyssalDirt>();

        int abyssHigh = AbyssCenter.Y - 500;

        int abyssLow = bottom;

        Rectangle rect = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        VeilGen.ClearWallsArea(rect);

        //Fill the entire area with abyss dirt tiles
        for (int x = left; x < right; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                tile.HasTile = true;
                tile.TileType = abyssTile;
            }
        }
        //var genRand = WorldGen.genRand;
        FastRandom fastRandom = new FastRandom(WorldGen.genRand.Next(0, 3000));
        for (int x = left; x < right; x++)
        {
            if (x > left && x < right - 1)
                continue;

            for (int y = abyssHigh; y < abyssLow; y += 8)
            {
                WorldGen.TileRunner(x, y,
                    strength: 48,
                    125, abyssTile, addTile: true);
            }
        }

        for (int x = left; x < right; x += 8)
        {
            int y = abyssHigh;
            WorldGen.TileRunner(x, y,
                strength: 48,
                125, abyssTile, addTile: true);
            y = abyssLow;
            WorldGen.TileRunner(x, y,
                strength: 48,
                125, abyssTile, addTile: true);
        }

        TileID.Sets.CanBeClearedDuringGeneration[abyssTile] = true;
        TileID.Sets.CanBeClearedDuringOreRunner[abyssTile] = true;

        Span<ushort> pool = new ushort[1].AsSpan();
        pool[0] = (ushort)ModContent.TileType<AbyssalCoarseDirt>();

        FastNoiseLite fnl = new FastNoiseLite();
        for (int i = 0; i < 1; i++)
        {
            fnl.SetSeed(fastRandom.Next(0, 20000));
            fnl.SetFrequency(0.05f);
            fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            fnl.SetDomainWarpAmp(65);
            for (int x = left; x < right; x++)
            {
                for (int y = abyssHigh; y < abyssLow; y++)
                {
                    float noise = fnl.GetNoise(x, y);
                    if (noise > 0.1f)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.TileType = pool[i];
                    }
                }
            }
        }

        Dictionary<int, List<Vector2>> caveConnectPoints = new Dictionary<int, List<Vector2>>();
        bool CreateAbyssCavernCave(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = fastRandom.Next(12, 18);
            float cavingSteps = fastRandom.Next(24, 64);
            float down = fastRandom.Next(-64, -12);
            int connectPointCounter = 5;
            bool success = false;
            while (scanArea.Contains(cavernPoint.ToPoint()) && failSafe < 500)
            {
                connectPointCounter--;
                if (cavingSteps > 0)
                {
                    if (connectPointCounter <= 0)
                    {
                        caveConnectPoints[index].Add(cavernPoint);
                    }
                    WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                          strength: strength,
                          fastRandom.Next(7, 27), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = fastRandom.Next(-64, -12);
                    strength = fastRandom.Next(12, 20);
                    cavingSteps = fastRandom.Next(24, 96);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }

        bool CreateAbyssClearing(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = fastRandom.Next(12, 18);
            float cavingSteps = fastRandom.Next(24, 64);
            float down = fastRandom.Next(-64, -12);
            int connectPointCounter = 5;
            bool success = false;
            while (scanArea.Contains(cavernPoint.ToPoint()) && failSafe < 500)
            {
                connectPointCounter--;
                if (cavingSteps > 0)
                {
                    if (connectPointCounter <= 0)
                    {
                        caveConnectPoints[index].Add(cavernPoint);
                    }
                    WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                          strength: strength,
                          fastRandom.Next(27, 32), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = fastRandom.Next(-64, -12);
                    strength = fastRandom.Next(22, 30);
                    cavingSteps = fastRandom.Next(48, 96);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }

        List<Vector2> FindPointsICanConnectTo(int index, Vector2 referencePoint)
        {
            float connectRadius = 150;
            float maxConnectionRadiusSquared = connectRadius * connectRadius;
            List<Vector2> otherPoints = new List<Vector2>(16);
            foreach (var kvp in caveConnectPoints)
            {
                if (kvp.Key == index)
                    continue;
                foreach (Vector2 cavePoint in kvp.Value)
                {
                    float distanceSquared = Vector2.DistanceSquared(referencePoint, cavePoint);
                    if (distanceSquared <= maxConnectionRadiusSquared)
                    {
                        otherPoints.Add(cavePoint);
                    }
                }
            }
            return otherPoints;
        }
        List<Vector2> FindAnyPointsICanConnectTo(Vector2 referencePoint, float connectRadius = 150)
        {
            float maxConnectionRadiusSquared = connectRadius * connectRadius;
            List<Vector2> otherPoints = new List<Vector2>(16);
            foreach (var kvp in caveConnectPoints)
            {
                foreach (Vector2 cavePoint in kvp.Value)
                {
                    float distanceSquared = Vector2.DistanceSquared(referencePoint, cavePoint);
                    if (distanceSquared <= maxConnectionRadiusSquared)
                    {
                        otherPoints.Add(cavePoint);
                    }
                }
            }
            return otherPoints;
        }


        //Sprinkle several long caves throughout the biome
        int numCaves = 18;
        Rectangle operationRectangle = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        operationRectangle = operationRectangle.CenterPad(25);

        for (int n = 0; n < numCaves; n++)
        {
            caveConnectPoints.TryAdd(n, new List<Vector2>());
            int dir = 1;
            if (fastRandom.Next(2) == 0)
                dir = -1;
            Vector2 p = new Vector2();
            p.X = fastRandom.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = fastRandom.Next(right - 25, right);
            p.X += fastRandom.Next(-250, 250);
            p.Y = (int)MathHelper.Lerp(abyssHigh, abyssLow, n / (float)numCaves);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            if (dir == -1)
                initialDirection *= -1;

            bool success = CreateAbyssCavernCave(n, p, initialDirection, operationRectangle);
            if (!success)
            {
                n--;
            }
        }


        //Create numerous clearings in the abyss
        int numClearings = 8;

        /*
        for (int n = 0; n < numClearings; n++)
        {
            int dir = 1;
            if (fastRandom.Next(2) == 0)
                dir = -1;
            Vector2 p = new Vector2();
            p.X = fastRandom.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = fastRandom.Next(right - 25, right);
            p.X += fastRandom.Next(-250, 250);
            p.Y = (int)MathHelper.Lerp(abyssHigh, abyssLow, n / (float)numClearings);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            if (dir == -1)
                initialDirection *= -1;

            bool success = CreateAbyssClearing(n, p, initialDirection, operationRectangle);
            if (!success)
            {
                n--;
            }
        }
        */
        //NOW WE CONNECT CAVES
        //Let's make two connections per layer
        //or atleast try to

        for (int n = 0; n < numCaves; n++)
        {
            int attempts = 0;
            for (int k = 0; k < 3; k++)
            {
                if (attempts >= 100)
                {
                    break;
                }
                List<Vector2> points = caveConnectPoints[n];
                if (points.Count <= 0)
                    break;

                Vector2 referencePoint = points[fastRandom.Next(0, points.Count)];
                List<Vector2> pointsICanConnectTo = FindPointsICanConnectTo(n, referencePoint);
                //So by distance to point
                pointsICanConnectTo = pointsICanConnectTo.OrderBy(x => Vector2.Distance(referencePoint, x)).ToList();

                if (pointsICanConnectTo.Count <= 0)
                {
                    k--;
                    attempts++;
                    continue;
                }
                int min = (int)MathF.Min(6, pointsICanConnectTo.Count);
                VeilGen.CreateAbyssConnectionCave(referencePoint, pointsICanConnectTo[fastRandom.Next(0, min)]);
            }
        }

        List<Point> validPointsForFlowers = new List<Point>();
        int skip = 8;
        int xPadding = 50;
        int innerLeft = left + xPadding;
        int innerRight = right - xPadding;
        int innerHigh = abyssHigh + 150;
        int innerLow = abyssLow - 125;
        for(int x = innerLeft; x < innerRight; x+= skip)
        {
            for(int y = innerHigh; y < innerLow; y+= skip)
            {
                Rectangle tileBounds = TileUtilities.CenterTileRectangle(new Point(x, y), 100, 100);
                if (!VeilGen.IsFilledEnough(tileBounds, 0.95f))
                {
                    continue;
                }
                validPointsForFlowers.Add(new Point(x, y));
            }
        }

        //Place Clearings
        //How do we palce these uhhhh
        //Yeahs
        int numBellFlowers = 5;
        List<Point> placedFlowers = new List<Point>();
        List<Vector2> allPoints = new List<Vector2>();
        foreach (var kvp in caveConnectPoints)
            allPoints.AddRange(kvp.Value);
        BellFlowerSystem.ClearBellFlowers();
        for (int n = 0; n < numBellFlowers; n++)
        {

            bool TooCloseToAnotherPlacedFlower(Point p)
            {
                foreach(Point placed in placedFlowers)
                {
                    if (TileUtilities.TooCloseToTilePoint(p, placed, proximity: 200))
                        return true;
                }
                return false;
            }
   
            Vector2 GetRandomConnectionPoint(Vector2 referencePoint, float checkDistance)
            {
                float checkDistanceSquared = checkDistance * checkDistance;
                for(int a = 0; a < 50; a++)
                {
                    Vector2 p = allPoints[fastRandom.Next(0, allPoints.Count)];
                    float distanceSquare = Vector2.DistanceSquared(referencePoint, p);
                    if (distanceSquare <= checkDistanceSquared)
                        return p;
                }
                return allPoints[fastRandom.Next(0, allPoints.Count)];
            }
            int maxAttempts = 100;
            int a = 0;
            
            
            while(a < maxAttempts)
            {
                Point randPoint = validPointsForFlowers[fastRandom.Next(0, validPointsForFlowers.Count)];
                if (TooCloseToAnotherPlacedFlower(randPoint))
                {
                    a++;
                    continue;
                }
         

                //List<Vector2> pointsICanConnectTo = FindAnyPointsICanConnectTo(randPoint.ToVector2(), connectRadius: 250);
                //pointsICanConnectTo = pointsICanConnectTo.OrderBy(x => Vector2.Distance(randPoint.ToVector2(), x)).ToList();
                Vector2 pointToConnectTo = GetRandomConnectionPoint(randPoint.ToVector2(), checkDistance: 250);
                if (pointToConnectTo.Y > randPoint.Y)
                    continue;


                VeilGen.CreateBellFlowerClearing(randPoint.ToVector2());

                CreateAbyssConnectionCaveMini(randPoint.ToVector2(), pointToConnectTo);
                placedFlowers.Add(randPoint);
                break;
            }
            if(a >= maxAttempts)
            {
                Main.NewText("FAIL TO PLACED BELL FLOWER", Color.Red);
            }
        }


        VeilGen.PruneLonelyTiles(rect);
        VeilGen.GenerateWaterBowls(rect, 512, new Point(5, 12), new Point(5, 12));
        VeilGen.GenerateWaterBlobs(rect, 4, new Point(64, 100));  
        var types = new ushort[]
        {
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalWhiteFlower>()
        };
        var types2 = new ushort[]
        {
            ModContent.ZTileType<AbyssalOrbFlower>()
        };
        var wetTypes = new ushort[]
        {
            ModContent.ZTileType<AbyssalReed>()
        };

        VeilGen.KillZTilesInArea(rect);

        int[] multiTileFlowers = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        var groundTiles = new List<int>
        {
            ModContent.TileType<AbyssalDirt>(),
            ModContent.TileType<AbyssalCoarseDirt>()
        };

        //No need to settle liquids anymore, water just places in the correct spot
        VeilGen.SettleLiquids();
        VeilGen.DecorateSurfaceEdgesWithMultiTile(rect, denom: 8, groundTiles, multiTileFlowers);
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 8,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types,
            value = 175
        });
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 128,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types2,
            value = 175
        });
        VeilGen.DecorateWetAreasWithZTile(new()
        {
            denom = 24,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = wetTypes,
            value = 175
        });


        VeilGen.DecorateEdgeTilesWithWalls(rect, groundTiles,
            (ushort)ModContent.WallType<AbyssalDirtWall>(), 1);

        VeilGen.DecorateEdgeTilesWithWalls(rect, groundTiles,
             (ushort)ModContent.WallType<AbyssalGrassWallDark>(), 1);
        VeilGen.GrowKelpArea<AbyssalKelp>(rect, minHeight: 5, maxHeight: 9, denom: 7);
        if (WorldGen.SkipFramingBecauseOfGen)
            return;
        for (int x = left; x < right; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                WorldGen.SquareTileFrame(x, y, resetFrame: true);
            }
        }

        TileUtilities.UpdateMap(rect, 255);
    }

    public static void CreateBellFlowerClearing(Vector2 pointToPlaceOn)
    {
        int tileRadius = 30;
        Point tilePoint = pointToPlaceOn.ToPoint();
        ushort abyssDirtTile = (ushort)ModContent.TileType<AbyssalDirt>();


        Rectangle originalBounds = TileUtilities.CenterTileRectangle(tilePoint, tileRadius * 2, tileRadius * 2);
        for (int x = originalBounds.Left; x < originalBounds.Right; x++)
        {
            for (int y = originalBounds.Top; y < originalBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = true;
                tile.TileType = abyssDirtTile;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }
        }

        ClearCircle(tilePoint, tileRadius);


        int islandRadius = 16;
        Point islandPoint = tilePoint;
        islandPoint.Y -= 8;
        WorldUtils.Gen(islandPoint, new Shapes.Circle(islandRadius),
            Actions.Chain(new Actions.SetTile(abyssDirtTile, false, false)));
        WorldUtils.Gen(islandPoint, new Shapes.HalfCircle(islandRadius),
            Actions.Chain(new Actions.ClearTile()));

        tileRadius += 6;
        int left = tilePoint.X - tileRadius / 2;
        int right = tilePoint.X + tileRadius / 2;
        int top = tilePoint.Y - tileRadius / 2;
        int bottom = tilePoint.Y + tileRadius / 2;

        for(int x = left; x < right; x++)
        {
            for(int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.LiquidAmount = 255;
                tile.LiquidType = LiquidID.Water;
            }
        }

        Rectangle bounds = TileUtilities.CenterTileRectangle(tilePoint, tileRadius, tileRadius);
        var targetTileTypes = new List<int> { abyssDirtTile };
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes,
            WallID.HallowedGrassUnsafe, 1);
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes,
            WallID.GrassUnsafe, 1);
        BellFlowerSystem.CreateBellFlower(islandPoint + new Point(0, -5));
    }

    public static void ClearCircle(Point tilePoint, int tileRadius)
    {
        WorldUtils.Gen(tilePoint, new Shapes.Circle(tileRadius, tileRadius),
           Actions.Chain(new Actions.ClearTile()));
        /*
        for (int x = tilePoint.X - tileRadius; x <= tilePoint.X + tileRadius; x++)
        {
            for (int y = tilePoint.Y - tileRadius; y <= tilePoint.Y + tileRadius; y++)
            {
                Point point = new Point(x, y);
                int dx = Math.Abs(point.X - tilePoint.X);
                int dy = Math.Abs(point.Y - tilePoint.Y);
                int diff = dx + dy;
                if (diff > tileRadius)
                    continue;
                Tile tile = Main.tile[point];
                tile.ClearTile();
            }
        }*/
    }
    public static void GrowKelpArea<KelpTile>(Rectangle tileBounds, int minHeight, int maxHeight, int denom)
        where KelpTile : ModTile
    {
        var genRand = WorldGen.genRand;
        for (int x = tileBounds.Left; x < tileBounds.Right; x++)
        {
            for(int y= tileBounds.Top; y < tileBounds.Bottom; y++)
            {
                Tile tileBelow = Main.tile[x, y + 1];
                Tile tile = Main.tile[x, y];
                if (!tileBelow.HasTile)
                    continue;
                if (tile.LiquidAmount <= 0)
                    continue;
                if (!genRand.NextBool(denom))
                    continue;
                GrowKelp<KelpTile>(x, y, minHeight, maxHeight);
            }
        }
    }
    /// <summary>
    /// Places a line of tiles
    /// </summary>
    /// <typeparam name="KelpTile"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxHeight"></param>
    public static void GrowKelp<KelpTile>(int x, int y, int minHeight, int maxHeight) 
        where KelpTile : ModTile
    {
        var genRand = WorldGen.genRand;
        int height = genRand.Next(minHeight, maxHeight);
        int endHeight = y - height;
        int startHeight = y;
        for(int j = endHeight; j <= startHeight; j++)
        {
            Tile tile = Main.tile[x, j];
            if (tile.HasTile)
                break;
            WorldGen.PlaceTile(x, j, ModContent.TileType<KelpTile>());
        }
    }

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
    public static void CreateAbyssConnectionCaveMini(Vector2 start, Vector2 end)
    {
        var genRand = WorldGen.genRand;
        float strength = genRand.NextFloat(6, 9);
        float steps = Vector2.Distance(start, end) / 2f;
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
                Tile tileAbove = Main.tile[x, y - 1];
                if (tileAbove.LiquidAmount > 0)
                    continue;
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
                        instanceData.value = parameters.value;
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
                        instanceData.value = parameters.value;
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
                    int steps = genRand.Next(0, 2);


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
                    //Fall down until hitting a solid tile or another water source
                    int newY = TileUtilities.FallToSolidOrWaterTile(x, y, maxSteps: 255);

                    //We're placing at 1 tile above the actual thing
                    newY--;
                    tile = Main.tile[x, newY];
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
