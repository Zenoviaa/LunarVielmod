using Microsoft.CodeAnalysis;
using Stellamod.Content.Areas.Tundra.Abyss;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.ZTileSystem;
using Stellamod.Items.Ores;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static tModPorter.ProgressUpdate;


namespace Stellamod.WorldG;

public partial class VeilGen
{
    private static readonly List<Point> _placedAbyssFlowers = new();
    public static Point AbyssCenterTile
    {
        get
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
            return AbyssCenter;
        }
    }

    public static Rectangle AbyssRectangle
    {
        get
        {
            int left = SavedGenerationParameters.SnowLeft;
            int right = SavedGenerationParameters.SnowRight;
            int top = SavedGenerationParameters.SnowTop;
            int bottom = ModContent.GetInstance<StellaWorld>().DarkspaceStart;

            Point AbyssCenter = new Point();
            AbyssCenter.X = left + right;
            AbyssCenter.X /= 2;
            AbyssCenter.Y = (int)(SavedGenerationParameters.RockLayerHigh + Main.maxTilesY * 0.15);
            AbyssCenter.Y -= 20;

            int abyssHigh = AbyssCenter.Y - 500;
            int abyssLow = bottom;

            Rectangle rect = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
            return rect;
        }
    }

    /// <summary>
    /// Sets the saved generation parameters in genvars for the snow origin points
    /// </summary>
    public static void SetAbyssGenerationParameters()
    {
        SavedGenerationParameters.SnowLeft = GenVars.snowOriginLeft;
        SavedGenerationParameters.SnowRight = GenVars.snowOriginRight;
        SavedGenerationParameters.SnowBottom = GenVars.snowBottom;
        SavedGenerationParameters.SnowTop = GenVars.snowTop;
        SavedGenerationParameters.RockLayerHigh = GenVars.rockLayerHigh;
        SavedGenerationParameters.DarkspaceTop = DarkspaceTop;
    }
    /// <summary>
    /// Generates the entire abyss biome
    /// </summary>
    public static void GenerateAbyss()
    {
        AbyssEffectsRenderer.rebuildWaterfalls = true;
        int left = SavedGenerationParameters.SnowLeft;
        int right = SavedGenerationParameters.SnowRight;
        int top = SavedGenerationParameters.SnowTop;
        int bottom = SavedGenerationParameters.DarkspaceTop;

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
        Rectangle wallRect = rect.CenterPad(64);

        VeilGen.ClearWallsArea(wallRect);

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
                tile.IsHalfBlock = false;
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
                    strength = fastRandom.Next(35, 45);
                    cavingSteps = fastRandom.Next(56, 100);
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

            bool success = false;
            if(n % 3 == 0)
            {
                success = CreateAbyssClearing(n, p, initialDirection, operationRectangle); 
            }
            else
            {
                success = CreateAbyssCavernCave(n, p, initialDirection, operationRectangle);
            }
          
            if (!success)
            {
                n--;
            }
        }


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
        int skip = 6;
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
        _placedAbyssFlowers.Clear();
        List<Vector2> allPoints = new List<Vector2>();
        foreach (var kvp in caveConnectPoints)
            allPoints.AddRange(kvp.Value);
        BellFlowerSystem.ClearBellFlowers();
        void GenerateBellFlowers()
        {
            for (int n = 0; n < numBellFlowers; n++)
            {

                bool TooCloseToAnotherPlacedFlower(Point p)
                {
                    int dx = Math.Abs(p.X - AbyssCenter.X);
                    if (dx < 165)
                        return true;
                    foreach (Point placed in _placedAbyssFlowers)
                    {
                        if (TileUtilities.TooCloseToTilePoint(p, placed, proximity: 200))
                            return true;
                    }
                    return false;
                }

                bool IsValidToConnectTo(Vector2 p, Vector2 referencePoint, float checkDistance)
                {
                    float checkDistanceSquared = checkDistance * checkDistance;
                    float distanceSquare = Vector2.DistanceSquared(referencePoint, p);
                    if (distanceSquare <= checkDistanceSquared)
                        return true;
                    return false;
                }


                void Fail()
                {
                    Main.NewText("Fail to PLACED BELL FLOWER", Color.Red);
                    Stellamod.Instance.Logger.Info($"Failed to place Bell Flower");
                }


                //Find all points that would would not be too close to another flower
                //Previously we were doing this randomly, but just iterating over every connection point and checking it's validity is definitely faster
                //And more reliable, that reliability is especially important.
                List<Point> pointsNotTooCloseToFlowers = validPointsForFlowers.Where(x => !TooCloseToAnotherPlacedFlower(x)).ToList();
                if(pointsNotTooCloseToFlowers.Count == 0)
                {
                    Fail();
                    continue;
                }

                Point randPoint = pointsNotTooCloseToFlowers[fastRandom.Next(0, pointsNotTooCloseToFlowers.Count)];

                //Find all points that are close enough to connect to for creating path ways
                List<Vector2> validConnectionPoints = allPoints.Where(x =>
                 x.Y <= randPoint.Y && IsValidToConnectTo(x, randPoint.ToVector2(), checkDistance: 250)).ToList();
                if (validConnectionPoints.Count <= 0)
                {
                    Fail();
                    continue;
                }

                Vector2 pointToConnectTo = validConnectionPoints[fastRandom.Next(0, validConnectionPoints.Count)];
                VeilGen.CreateBellFlowerClearing(randPoint.ToVector2());
                CreateAbyssConnectionCaveMini(randPoint.ToVector2() + new Vector2(0, -16), pointToConnectTo);
                _placedAbyssFlowers.Add(randPoint);
            }
        }

        for(int i = 0; i < 3; i++)
            VeilGen.PruneLonelyTiles(rect);
        VeilGen.GenerateWaterBowls(rect, 512, new Point(5, 12), new Point(5, 12));
        VeilGen.GenerateWaterBlobs(rect, 4, new Point(64, 100));
        GenerateBellFlowers();

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
        VeilGen.CreateHalfBlocksOnEdges(rect);
        VeilGen.CreateHalfBlocksOnEdges(rect);
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

        TileID.Sets.CanBeClearedDuringGeneration[abyssTile] = false;
        TileID.Sets.CanBeClearedDuringOreRunner[abyssTile] = false;
        /*
        if (!WorldGen.SkipFramingBecauseOfGen)
        {
            for (int x = left; x < right; x++)
            {
                for (int y = abyssHigh; y < abyssLow; y++)
                {
                    WorldGen.SquareTileFrame(x, y, resetFrame: true);
                    WorldGen.SquareWallFrame(x, y, resetFrame: true);
                }
            }
        }

        if (!WorldGen.SkipFramingBecauseOfGen)
        {
            TileUtilities.UpdateMap(rect, 255);
        }*/
    }

    public static void GrowKelpInAbyss()
    {
        VeilGen.GrowKelpArea<AbyssalKelp>(AbyssRectangle, minHeight: 5, maxHeight: 9, denom: 7);

        //Extra kelp around flowers
        foreach (Point p in _placedAbyssFlowers)
        {
            Rectangle kelpRect = TileUtilities.CenterTileRectangle(p, 50, 50);
            VeilGen.GrowKelpArea<AbyssalKelp>(kelpRect, minHeight: 20, maxHeight: 35, denom: 4);
        }
    }
    public static void PlaceAbysmTemple()
    {
        VeilGen.PlaceAbysmTemple(AbyssCenterTile + new Point(0, 256));
    }

    public static Point FindSurfaceOfWater(Point tilePoint, int maxSteps)
    {     
        Tile tile = Main.tile[tilePoint];
        for(int s = 0; s < maxSteps; s++)
        {
            if (tilePoint.Y <= 0)
            {
                tilePoint.Y = 0;
                return tilePoint;
            }
            Tile wetTile = Main.tile[tilePoint];
            if ((wetTile.HasTile && WorldGen.SolidTile(tilePoint)) || wetTile.LiquidAmount <= 0)
                return tilePoint;
            tilePoint.Y--;
        }
        return tilePoint;
    }

    public static bool HasNumAirTilesAbove(Point tilePoint, int steps)
    {
        Tile tile = Main.tile[tilePoint];
      
        for(int y = 0; y < steps; y++)
        {
            Point nextPoint = tilePoint + new Point(0, -y);
            if (nextPoint.Y < 0)
                return true;

            Tile tileAbove = Main.tile[nextPoint];
            if (tileAbove.HasTile && WorldGen.SolidTile(nextPoint))
                return false;
        }
        return true;
    }

    public static void PlaceAbysmTemple(Point abyssCenter)
    {
        StructureMap structures = GenVars.structures;
        Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Aurelus/AurelusTemple2");
        Point Loc = abyssCenter;
        Loc.X -= rectangle.Width / 2;
        Loc.Y += rectangle.Height / 2;
        rectangle.Location = Loc;

        Rectangle templeRectangle = rectangle;
        templeRectangle.Y -= rectangle.Height;
        /*
        for(int x = templeRectangle.Left; x< templeRectangle.Right; x++)
        {
            for(int y = templeRectangle.Top; y < templeRectangle.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = false;
            }
        }
        */
        VeilGen.KillZTilesInArea(templeRectangle);
        SavedGenerationParameters.AbyssTempleRectangle = templeRectangle;
        StructureLoader.ProtectStructure(Loc, "Struct/Aurelus/AurelusTemple2");
        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/AurelusTemple2");
        //Should prob make a chest loot apie
        foreach (int chestIndex in ChestIndexs)
        {
            var chest = Main.chest[chestIndex];
            // etc

            // itemsToAdd will hold type and stack data for each item we want to add to the chest
            var itemsToAdd = new List<(int type, int stack)>();

            // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
            // Using a switch statement and a random choice to add sets of items.
            switch (Main.rand.Next(7))
            {
                case 0:
                    itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                    itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                    break;
                case 1:
                    itemsToAdd.Add((ModContent.ItemType<Venatici>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                    itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                    itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                    break;
                case 2:
                    itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                    itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                    break;
                case 3:
                    //     itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                    // itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                    itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                    break;
                case 4:
                    itemsToAdd.Add((ModContent.ItemType<HolmbergScythe>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                    itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                    break;

                case 5:
                    itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                    itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                    break;

                case 6:

                    itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                    itemsToAdd.Add((ItemID.Shiverthorn, Main.rand.Next(2, 15)));
                    itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                    itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
                    break;
            }

            // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
            int chestItemIndex = 0;
            foreach (var itemToAdd in itemsToAdd)
            {
                Item item = new Item();
                item.SetDefaults(itemToAdd.type);
                item.stack = itemToAdd.stack;
                chest.item[chestItemIndex] = item;
                chestItemIndex++;
                if (chestItemIndex >= 40)
                    break; // Make sure not to exceed the capacity of the chest
            }
        }
    }

    /// <summary>
    /// Returns the number of tiles that have any liquid within a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static int CountLiquids(Rectangle tileBounds)
    {
        int count = 0;
        for (int x = tileBounds.Left; x <= tileBounds.Right; x++)
        {
            for (int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.LiquidAmount > 0)
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns the number of solids that are filled with any solid tile in a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static int CountSolids(Rectangle tileBounds)
    {
        int count = 0;
        for (int x = tileBounds.Left; x <= tileBounds.Right; x++)
        {
            for (int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile && WorldGen.SolidTile(x, y))
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Returns the percentage of tiles that are filled with any solid in a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static float CountSolidsPercent(Rectangle tileBounds)
    {
        int solidCount = CountSolids(tileBounds);
        int maxCount = tileBounds.Width * tileBounds.Height;
        float pct = (float)solidCount / (float)maxCount;
        return pct;
    }

    /// <summary>
    /// Returns the percentage of tiles that are filled with any liquid in a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static float CountLiquidsPercent(Rectangle tileBounds)
    {
        int liquidCount = CountLiquids(tileBounds);
        int maxLiquidCount = tileBounds.Width * tileBounds.Height;


        float pct = (float)liquidCount / (float)maxLiquidCount;
        return pct;
    }

    /// <summary>
    /// Creates half blocks on the edges of water ponds within the given area
    /// </summary>
    /// <param name="tileBounds"></param>
    public static void CreateHalfBlocksOnEdges(Rectangle tileBounds)
    {
        for(int x = tileBounds.Left; x <= tileBounds.Right; x++)
        {
            for(int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileRight = Main.tile[x + 1, y];
                Tile tileAbove = Main.tile[x, y - 1];
                Tile tileLeft = Main.tile[x - 1, y];

                //Right Half Block
                if(tile.HasTile &&
                    tileLeft.LiquidAmount > 0 &&
                    !tileAbove.HasTile)
                {
                    tile.IsHalfBlock = true;
                }

                //Left Half block
                if(tile.HasTile &&
                    tileRight.LiquidAmount > 0 &&
                    !tileAbove.HasTile)
                {
                    tile.IsHalfBlock = true;
                }
            }
        }
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
                tile.LiquidAmount = 0;
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

        int pillarLeft = islandPoint.X - 6;
        int pillarRight = islandPoint.X + 6;
        int pillarTop = islandPoint.Y + 8;
        int pillarBottom = tilePoint.Y + tileRadius + 2;

        for (int x = pillarLeft; x <= pillarRight; x++)
        {
            for (int y = pillarTop; y < pillarBottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = true;
                tile.TileType = abyssDirtTile;
                tile.LiquidAmount = 0;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }
        }
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
                if (tile.HasTile)
                    continue;
                tile.LiquidAmount = 255;
                tile.LiquidType = LiquidID.Water;
            }
        }

        Rectangle bounds = TileUtilities.CenterTileRectangle(tilePoint, tileRadius * 2, tileRadius * 2);
        var targetTileTypes = new List<int> { abyssDirtTile };
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes, 2,
            (ushort)ModContent.WallType<AbyssalGrassWallDark>(), 7);
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes, 2,
            (ushort)ModContent.WallType<AbyssalDirtWall>(), 7);
        VeilGen.DecorateWallEdgesWithWallPatches(bounds, new List<int>
        { 
            ModContent.WallType<AbyssalDirtWall>(),
            ModContent.WallType<AbyssalGrassWallDark>() 
        }, 32, (ushort)ModContent.WallType<AbyssalGrassWall>());
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
        ushort type = (ushort)ModContent.TileType<KelpTile>();
        for(int j = endHeight; j <= startHeight; j++)
        {
            Tile tile = Main.tile[x, j];
            if (tile.HasTile)
                break;
            tile.HasTile = true;
            tile.TileFrameX = -1;
            tile.TileFrameY = -1;
            tile.TileType = type;
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
                tile.WallType = WallID.None;
            
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
    public static void DecorateEdgeTilesWithWalls(Rectangle tileBounds, List<int> targetTileTypes, int steps, ushort wallType, int maxWallCaveWidth = 2)
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

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(1, 1),
                                new Actions.PlaceWall(wallType));
                        }

                        point += (baseDirection * 4).RotatedByRandom(MathHelper.ToRadians(360)).ToPoint();
                    }
                }
            }
        }
    }
    public static void DecorateWallEdgesWithWallPatches(Rectangle tileBounds, List<int> targetTileTypes, int steps, ushort wallType, int maxWallCaveWidth = 2)
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
                if (!targetTileTypes.Contains(tile.WallType))
                    continue;
                Tile tileLeft = Main.tile[x - 1, y];
                Tile tileRight = Main.tile[x + 1, y];
                Tile tileBottom = Main.tile[x, y + 1];
                Tile tileTop = Main.tile[x, y - 1];
                bool hasAny = 
                    tileLeft.WallType == WallID.None ||
                    tileRight.WallType == WallID.None || 
                    tileTop.WallType == WallID.None || 
                    tileBottom.WallType == WallID.None;
                
                if (hasAny && genRand.NextBool(16))
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
      
                    Vector2 worldPos = new Vector2(x, y).ToWorldCoordinates();
                    for (int s = 0; s < steps; s++)
                    {
                        Point tilePoint = worldPos.ToTileCoordinates();
                        Tile placeTile = Main.tile[tilePoint];
                        placeTile.WallType = wallType;
                        placeTile.WallFrameX = -1;
                        placeTile.WallFrameY = -1;
                     
                        worldPos += (baseDirection * 8).RotatedByRandom(MathHelper.ToRadians(360));
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
