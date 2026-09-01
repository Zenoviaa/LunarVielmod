using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Core.ZTileSystem;
using Stellamod.Tiles;
using Stellamod.Tiles.Veil;
using Stellamod.TilesNew.MothlightTiles;
using Stellamod.TilesNew.RainforestTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public class VeilGenTester : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 1;
    }

    public override bool? UseItem(Player player)
    {
        AbyssTest();
        // LayoutTest();
        //  CaveTest();
        // AegislavTest();
        //   CaveTest2();
        return true;
    }

    private static void AbyssTest()
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
        var genRand = WorldGen.genRand;
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
            fnl.SetSeed(genRand.Next(0, 20000));
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
            float strength = genRand.NextFloat(12, 18);
            float cavingSteps = genRand.Next(24, 64);
            float down = genRand.Next(-64, -12);
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
                          genRand.Next(7, 27), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = genRand.Next(-64, -12);
                    strength = genRand.NextFloat(12, 20);
                    cavingSteps = genRand.Next(24, 96);
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
            if (genRand.NextBool(2))
                dir = -1;
            Vector2 p = new Vector2();
            p.X = genRand.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = genRand.Next(right - 25, right);
            p.X += genRand.Next(-250, 250);
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

                Vector2 referencePoint = points[genRand.Next(0, points.Count)];
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
                VeilGen.CreateAbyssConnectionCave(referencePoint, pointsICanConnectTo[genRand.Next(0, min)]);
            }
        }

        Rectangle rect = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
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
        VeilGen.ClearWallsArea(rect);
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

        VeilGen.SettleLiquids();
        VeilGen.DecorateSurfaceEdgesWithMultiTile(rect, denom: 8, groundTiles, multiTileFlowers);
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 8,
            renderLayer = ZRenderLayer.Midground,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types
        }); 
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 128,
            renderLayer = ZRenderLayer.Midground,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types2
        });
        VeilGen.DecorateWetAreasWithZTile(new()
        {
            denom = 24,
            renderLayer = ZRenderLayer.Midground,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = wetTypes
        });

        VeilGen.DecorateEdgeTilesWithWalls(rect, groundTiles, 
            (ushort)ModContent.WallType<AbyssalDirtWall>());
        VeilGen.GrowKelpArea<AbyssalKelp>(rect, minHeight: 5, maxHeight: 9, denom: 7);

        for (int x = left; x < right; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                WorldGen.SquareTileFrame(x, y, resetFrame: true);
            }
        }
        if (WorldGen.SkipFramingBecauseOfGen)
            return;
        TileUtilities.UpdateMap(rect, 255);

    }
    private static void CaveTest2()
    {
        Point mousePoint = Main.MouseWorld.ToTileCoordinates();
        WorldGen.CaveOpenater(mousePoint.X, mousePoint.Y);
    }

    private static void CaveTest()
    {
        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 5, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        bool[,] map = VeilGen.CellularAutomataMap(4000, 256, in @params, Main.rand);
        int width = map.GetLength(0);
        int height = map.GetLength(1);



        VeilGen.Erase(new Point(100, 2500), map);
    }

    private static void AegislavTest()
    {
        Point aegislavCastlePoint = Main.MouseWorld.ToTileCoordinates() + new Point(0, -100);
        aegislavCastlePoint = TileUtilities.FallToSolidTile(aegislavCastlePoint);

        string path = "Structures/BloodletCastle";
        aegislavCastlePoint.Y += 15;
        Structurizer.ReadStruct(aegislavCastlePoint, path, Structurizer.DefaultTileBlend);
        Structurizer.ProtectStructure(aegislavCastlePoint, path);
    }
    private static void LayoutTest()
    {
        (int, int)[] layout = DungeonLayouter.GenerateLayout(16, Main.rand);
        for (int r = 0; r < layout.Length; r++)
        {
            Main.NewText(layout[r]);
        }


        Point[] vertices = new Point[layout.Length];
        for (int v = 0; v < vertices.Length; v++)
        {
            vertices[v] = new Point(layout[v].Item1, layout[v].Item2);
        }
        DungeonGenerationHelper.vertices = vertices;
    }
    private static void MineshaftTest()
    {
        Point startTile = Main.MouseWorld.ToTileCoordinates();
        VeilGen.PlaceMineshaft(startTile);
    }
    private static void MistyDungeonTest()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        Point startTile = mouseWorld.ToTileCoordinates();
        Room[] prefabs = DungeonSaveUtility.ReadDungeonPrefabsFromFiles();
        GenerationPrefab generationPrefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("MistyDungeon_1");
        DungeonChart chart = DungeonChart.FromPrefab(generationPrefab);
        Room[] map = Dungeonizer.GenerateFromChart(prefabs, chart, Main.rand);
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };


        Point topLeft = Point.Zero;
        Point bottomRight = Point.Zero;
        for (int r = 0; r < map.Length; r++)
        {
            Room room = map[r];
            if (topLeft.X > room.bounds.Left)
                topLeft.X = room.bounds.Left;
            if (topLeft.Y > room.bounds.Top)
                topLeft.Y = room.bounds.Top;

            if (bottomRight.X < room.bounds.Right)
                bottomRight.X = room.bounds.Right;
            if (bottomRight.Y < room.bounds.Bottom)
                bottomRight.Y = room.bounds.Bottom;
        }
        Rectangle rectangle = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

        Point point = startTile;
        Point vectorToOrigin = (point - rectangle.Top().ToPoint());
        rectangle.Location += vectorToOrigin;

        //Just a failsafe
        while (rectangle.Right().X >= Main.maxTilesX)
            rectangle.Location -= new Point(32, 0);

        int width = rectangle.Width;
        width -= 150;
        int height = rectangle.Height;


        //So we're just gonna start from index 1 to skip it
        for (int r = 1; r < map.Length; r++)
        {
            Room room = map[r];
            int padding = 10;
            Rectangle roomRectangle = Structurizer.ReadRectangle(room.prefab);
            int outlineWidth = roomRectangle.Width + padding;
            int outlineHeight = roomRectangle.Height + padding;

            //This hsould give us an outline of bricks, I think
            Point topLeftRoom = room.bounds.TopLeft().ToPoint() + new Point(-padding / 2, -padding / 2);
            Point offset = rectangle.Top().ToPoint();
            offset.Y -= outlineHeight;
            topLeftRoom += offset;
            WorldUtils.Gen(topLeftRoom, new Shapes.Rectangle(outlineWidth, outlineHeight),
               Actions.Chain(
                    new Actions.ClearWall(),
                    new Actions.SetTile((ushort)ModContent.TileType<MothlightBrick>()))
               );
        }

        for (int r = 0; r < map.Length; r++)
        {
            Room room = map[r];
            Point bottomLeft = room.bounds.BottomLeft().ToPoint();
            Point offset = rectangle.Top().ToPoint();

            int tileX = offset.X;
            int tileY = offset.Y;

            bottomLeft.X += tileX;
            bottomLeft.Y += tileY;
            bottomLeft.Y -= map[0].bounds.Height;
            Structurizer.ReadStruct(bottomLeft, room.prefab, tileBlend);
            Structurizer.ProtectStructure(bottomLeft, room.prefab);
        }
    }
    public static void GenerateDungeon()
    {
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        WorldGen.KillTile(tileX, tileY, noItem: true);
    }

    private float GetMarshHeight(float x)
    {
        float bump = x * (4 - x * 4);
        float mountains = MathF.Sin(x * 2) * 0.5f + 0.5f;
        float mountains2 = MathF.Sin(x * 2) * 0.5f + 0.7f;
        float dips = MathF.Sin(x * 32) * 0.1f;
        float roughness = MathF.Sin(x * 120) * 0.01f;
        float roughness2 = MathF.Sin(x * 200) * 0.005f;
        float y = bump * mountains * mountains2 - dips - roughness - roughness2;
        return y + 0.1f;
    }

    private void GenerateMarsh(Point startTile, int length)
    {
        var genRand = WorldGen.genRand;

        //Generate the terrain
        Point endTile = startTile + new Point(length, 0);
        int mountainHeight = 200;
        int[] heights = new int[length];
        int grassTileType = ModContent.TileType<RainforestGrass>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;

            float ratio = localX / length;
            // Console.WriteLine(ratio);
            int height = (int)(GetMarshHeight(ratio) * mountainHeight);
            heights[x - startTile.X] = height;
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startTile.Y - y, grassTileType);
            }
        }

        ushort uGrassTileType = (ushort)grassTileType;
        //Generate big trees, mangrove trees
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = startTile.Y - height;
            Tile tile = Main.tile[x, startTile.Y - height];

            Rectangle scanArea = new Rectangle(x, y, 5, 2);
            Point point = new Point(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height), new Actions.TileScanner(uGrassTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];

            if (tileCount >= 5)
            {
                if (genRand.NextBool(16))
                {
                    int treeHeight = genRand.Next(20, 48);
                    VeilGen.PlaceMangroveTrees(x, y, treeHeight);
                }
            }
        }

        //Now we're going to place acacia trees
        ushort bigTreeTileType = (ushort)ModContent.TileType<MangroveTree>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = startTile.Y - height;
            Tile tile = Main.tile[x, startTile.Y - height];

            Rectangle scanArea = new Rectangle(x, y, 5, 2);
            Point point = new Point(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height), new Actions.TileScanner(uGrassTileType, bigTreeTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];
            int mangroveTreeCount = dictionary[bigTreeTileType];

            if (tileCount >= 5 && mangroveTreeCount <= 0)
            {
                if (genRand.NextBool(8))
                {
                    int treeHeight = genRand.Next(12, 20);
                    VeilGen.PlaceAcaciaTrees(x, y, treeHeight);
                }
            }
        }

        //Spawn surface waters
        int numWaterBlotches = Main.rand.Next(5, 10);
        for (int n = 0; n < numWaterBlotches; n++)
        {
            int randX = genRand.Next(startTile.X, endTile.X);

            int heightIndex = randX - startTile.X;
            int height = heights[heightIndex];

            int randY = startTile.Y - height - 20;

            int radius = 12;
            Point point = new Point(randX, randY);
            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 2, radius / 2),
                new Actions.SetLiquid(type: LiquidID.Water));
        }

        //Spawn underground waters
        numWaterBlotches = genRand.Next(60, 80);
        for (int n = 0; n < numWaterBlotches; n++)
        {
            int randX = genRand.Next(startTile.X, endTile.X);

            int heightIndex = randX - startTile.X;
            int height = heights[heightIndex];

            int randY = startTile.Y - height + 10 + genRand.Next(0, 100);
            randY = (int)MathHelper.Clamp(randY, startTile.Y - height, startTile.Y);

            int radius = genRand.Next(8, 20);
            Point point = new Point(randX, randY);

            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 2, radius / 2),
                new Actions.ClearTile(true));

            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 3, radius / 3),
                new Actions.SetLiquid(type: LiquidID.Water));
        }
    }
    private void GenerateSkullrunnerCircle()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int size = 80;
        int tileType = ModContent.TileType<ManorBlock>();
        Point point = new Point(tileX, tileY);
        WorldUtils.Gen(point, new Shapes.Circle(size, size), Actions.Chain(
            new GenAction[] { new Actions.SetTile((ushort)tileType, true, true) }));

        int hollowSize = 60;
        WorldUtils.Gen(point, new Shapes.Circle(hollowSize, hollowSize), Actions.Chain(
            new GenAction[] { new Actions.ClearTile(true) }));


        WorldUtils.Gen(point, new ShapeUtilities.HalfCircle(hollowSize), Actions.Chain(
            new GenAction[] { new Actions.SetLiquid(LiquidID.Lava) }));


        int rectSize = size - 10;
        int rectHeight = rectSize - 60;
        Point rectanglePoint = new Point(tileX - rectSize / 2, tileY);
        WorldUtils.Gen(rectanglePoint, new Shapes.Rectangle(rectSize, rectHeight), Actions.Chain(
            new GenAction[] { new Actions.SetTile((ushort)tileType, true, true) }));

        int rectSize2 = size - 14;
        Point rectanglePoint2 = new Point(tileX - rectSize2 / 2, tileY + 2);
        WorldUtils.Gen(rectanglePoint2, new Shapes.Rectangle(rectSize2, rectHeight), Actions.Chain(
          new GenAction[] { new Actions.SetTile((ushort)tileType, true, true) }));


        int rectSize3 = size - 18;
        Point rectanglePoint3 = new Point(tileX - rectSize3 / 2, tileY + 4);
        WorldUtils.Gen(rectanglePoint3, new Shapes.Rectangle(rectSize3, rectHeight), Actions.Chain(
          new GenAction[] { new Actions.SetTile((ushort)tileType, true, true) }));

    }


    private void GenerateColosseum()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Vector2 colosseumPosition = new Vector2(tileX, tileY);
        Point colosseumPoint = colosseumPosition.ToPoint();
        Vector2 caveStrength = new Vector2(12, 15);
        int fallSteps = 150;
        int caveWidth = 5;
        VeilGen.GenerateDuneHole((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.Zero, caveWidth,
          caveSteps: fallSteps,
          tileToPlace: TileID.Sandstone,
          addTile: true);
        VeilGen.GenerateDuneHoleEdges((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.Zero, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: TileID.Sandstone,
            addTile: true);
        VeilGen.GenerateDuneHoleEdges((colosseumPoint + new Point(51, 0)).ToVector2(), Vector2.UnitY, caveStrength, Vector2.Zero, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: -1,
            addTile: false);
        VeilGen.GenerateColosseum(colosseumPosition.ToPoint());

    }

    private void GenerateCavernToAbyss()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 caveVelocity = Vector2.UnitY;
        Vector2 caveStrength = new Vector2(15, 20);
        Vector2 pullDirection = -Vector2.UnitX * 0.2f;
        int caveWidth = 7;
        int caveSteps = 100;
        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateSmallFallingIceCavern()
    {

        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 caveVelocity = Vector2.UnitX;
        if (genRand.NextBool(2))
        {
            caveVelocity = -Vector2.UnitX;
        }
        Vector2 caveStrength = new Vector2(5, 10);
        Vector2 pullDirection = Vector2.UnitY;
        int caveWidth = 7;
        int caveSteps = 25;
        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateFallingIceCavern()
    {

        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 caveVelocity = Vector2.UnitX;
        Vector2 caveStrength = new Vector2(20, 30);
        Vector2 pullDirection = Vector2.UnitY;
        int caveWidth = 7;
        int caveSteps = 100;
        VeilGen.GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
    }


    private void GenerateIceCavern()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 caveVelocity = Vector2.UnitX;
        Vector2 caveStrength = new Vector2(20, 30);
        int caveWidth = 7;
        int caveSteps = 100;
        VeilGen.GenerateIceCavern(cavePosition, caveVelocity, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateMarble()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Point granitePoint = new Point(tileX, tileY);

        int maxRadius = 64;
        int radius = genRand.Next(24, 64);
        float sizeMultiplier = radius / (float)maxRadius;
        WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
            new Actions.SetTile(TileID.Marble));
        for (int n = 0; n < 150; n++)
        {
            int r = genRand.Next(radius - 30, radius + 16);
            Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
            Vector2 strength = new Vector2(3, 4);
            WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), -1);
        }
        for (int n = 0; n < 450; n++)
        {
            int r = genRand.Next(radius - 30, radius + 16);
            Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
            Vector2 strength = new Vector2(8, 10);
            WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), TileID.Marble);
        }

        Vector2 cavePosition = new Vector2(tileX, tileY) - new Vector2(radius, radius / 4);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);

        //Chance to open up
        int caveWidth = 5;
        int caveSteps = (int)(50f * sizeMultiplier);
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;

        ushort[] wallTypes = new ushort[]
        {
            WallID.MarbleUnsafe,
            WallID.MarbleBlock
        };

        for (int w = 0; w < 800; w++)
        {
            Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
            ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceWall(wallType),
                new Actions.Smooth(true)
            }));
        }

        for (int j = 0; j < caveSteps; j++)
        {

            Vector2 newVelocity = caveVelocity;
            newVelocity.Y += MathF.Sin(j * 2f) * 8;
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 20), -1);
            }

            // Update the cave position.
            cavePosition += newVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    private void GenerateGranite()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        Point granitePoint = new Point(tileX, tileY);

        int radius = genRand.Next(24, 64);
        WorldUtils.Gen(granitePoint, new Shapes.Circle(radius, radius),
            new Actions.SetTile(TileID.Granite));
        for (int n = 0; n < 150; n++)
        {
            int r = genRand.Next(radius - 30, radius + 16);
            Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
            Vector2 strength = new Vector2(3, 4);
            WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), -1);
        }
        for (int n = 0; n < 450; n++)
        {
            int r = genRand.Next(radius - 30, radius + 16);
            Point tileRunnePoint = granitePoint + genRand.NextVector2CircularEdge(r, r).ToPoint();
            Vector2 strength = new Vector2(8, 10);
            WorldGen.TileRunner(tileRunnePoint.X, tileRunnePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), TileID.Granite);
        }

        Vector2 cavePosition = new Vector2(tileX, tileY) - new Vector2(radius / 4, radius);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);

        //Chance to open up
        int caveWidth = 5;
        int caveSteps = 50;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;

        ushort[] wallTypes = new ushort[]
        {
            WallID.GraniteUnsafe,
            WallID.GraniteBlock
        };

        for (int w = 0; w < 800; w++)
        {
            Point shadowOrbPoint = granitePoint + genRand.NextVector2Circular(radius, radius).ToPoint();
            ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceWall(wallType),
                new Actions.Smooth(true)
            }));
        }


        for (int j = 0; j < caveSteps; j++)
        {

            Vector2 newVelocity = caveVelocity;
            newVelocity.X += MathF.Sin(j) * 8;
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(6, 6), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += newVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    private void GenerateEvil()
    {
        var genRand = WorldGen.genRand;
        Vector2 mouseWorld = Main.MouseWorld;
        int mx = (int)Main.MouseWorld.X / 16;
        int my = (int)Main.MouseWorld.Y / 16;
        Point tilePoint = new Point(mx, my);
        Point evilPoint = tilePoint;

        int radius = 96;
        WorldGen.crimson = false;
        ushort blockType = WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
        ushort wallType = WorldGen.crimson ? WallID.CrimsonUnsafe1 : WallID.CorruptionUnsafe1;

        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(blockType));
        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 20, radius - 20), new Actions.ClearTile());
        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 40, radius - 40), new Actions.SetTile(blockType));

        ushort[] corruptWallTypes = new ushort[]
        {
                    WallID.CorruptionUnsafe1,
                    WallID.CorruptionUnsafe2,
                    WallID.EbonstoneUnsafe
        };

        ushort[] crimsonWallTypes = new ushort[]
        {
                    WallID.CrimsonUnsafe1,
                    WallID.CrimsonUnsafe2,
                    WallID.CrimstoneUnsafe
        };

        int decorativeBlock = WorldGen.crimson ? TileID.FleshBlock : TileID.LesionBlock;
        int lampType = WorldGen.crimson ? 14 : 33;
        int lanternType = WorldGen.crimson ? 23 : 39;
        for (int w = 0; w < 800; w++)
        {
            Point shadowOrbPoint = evilPoint + genRand.NextVector2Circular(80, 80).ToPoint();

            ushort wallType2 = WorldGen.crimson ?
                crimsonWallTypes[genRand.Next(0, crimsonWallTypes.Length)] :
                corruptWallTypes[genRand.Next(0, corruptWallTypes.Length)];
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
            {
                        new Actions.PlaceWall(wallType2),
                        new Actions.Smooth(true)
            }));
        }

        for (int w = 0; w < 150; w++)
        {
            int radius2 = genRand.Next(50, 100);
            Point shadowOrbPoint = evilPoint + genRand.NextVector2CircularEdge(radius2, radius2).ToPoint();
            ushort wallType2 = WorldGen.crimson ? WallID.Flesh : WallID.LesionBlock;
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(1, 1), Actions.Chain(new GenAction[]
            {
                        new Actions.PlaceWall(wallType2),
                        new Actions.Smooth(true)
            }));
        }


        float pokey = 12;
        for (int n = 0; n < pokey; n++)
        {
            float progress = n / pokey;
            float rot = progress * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * 66;
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            Vector2 moveVelocity = -velocity.SafeNormalize(Vector2.Zero);
            VeilGen.GenerateSimpleCave(cavePoint.ToVector2(), moveVelocity,
                strength, moveVelocity, 2, caveSteps: 30);
        }

        for (int n = 0; n < 800; n++)
        {
            float progress = n / 800f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), -1);
        }

        for (int n = 0; n < 800; n++)
        {
            float progress = n / 800f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);


            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 800; n++)
        {
            float progress = n / 800f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(60, 100);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new Vector2(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 10; n++)
        {
            float progress = n / 10f;
            float rot = progress * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(30);
            Vector2 velocity = rot.ToRotationVector2() * 10;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float progress = n / 10f;
            float rot = progress * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(60);
            Vector2 velocity = rot.ToRotationVector2() * 30;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float progress = n / 10f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * 50;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 1600; n++)
        {
            float range = genRand.NextFloat(30, 100);
            Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();

            WorldGen.Place1xX(fPoint.X, fPoint.Y, TileID.Lamps, style: lampType);
        }
        for (int n = 0; n < 800; n++)
        {
            float range = genRand.NextFloat(30, 100);
            Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();
            WorldGen.Place1x2Top(fPoint.X, fPoint.Y, TileID.HangingLanterns, style: lanternType);
        }

        //Make Extra
        Vector2 caveStrength = new Vector2(10, 12);
        Vector2 pullDirection = -Vector2.UnitY;
        int caveWidth = 5;
        int steps = 150;

        VeilGen.GenerateSimpleCaveWall((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: wallType);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: blockType);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-16, -32)).ToVector2(), -Vector2.UnitX, caveStrength, pullDirection, caveWidth, caveSteps: steps, tileToPlace: -1);

        int fallSteps = 40;
        VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.UnitY, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: blockType);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength, Vector2.UnitY, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: -1);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength * 2f, Vector2.UnitX, caveWidth,
            caveSteps: fallSteps * 2,
            tileToPlace: blockType,
            addTile: true);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength, Vector2.UnitX, caveWidth,
            caveSteps: fallSteps * 2,
            tileToPlace: -1);

        for (int n = 0; n < 6400; n++)
        {
            int x = genRand.Next(evilPoint.X - 128, evilPoint.X + 128);
            int y = genRand.Next(evilPoint.Y + 90, evilPoint.Y + 150);
            int style = WorldGen.crimson ? 1 : 0;
            WorldGen.Place3x2(x, y, 26, style);
        }

        for (int x = evilPoint.X - 128; x < evilPoint.X + 128; x++)
        {
            int y = evilPoint.Y + 100;
            Point wallPoint = new Point(x, y);
            ushort wallType2 = WorldGen.crimson ? WallID.CrimstoneUnsafe : WallID.EbonstoneUnsafe;
            WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceWall(wallType2),
                new Actions.Smooth(true)
            }));
        }


        //Crimsonfy/Ebonfy surroundings
        int corruptRadius = 500;
        for (int x = evilPoint.X - corruptRadius; x < evilPoint.X + corruptRadius; x++)
        {
            for (int y = evilPoint.Y - corruptRadius; y < evilPoint.Y + corruptRadius; y++)
            {
                if (!WorldGen.SolidTile(x, y))
                    continue;
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Grass)
                {
                    ushort grassType = WorldGen.crimson ? TileID.CrimsonGrass : TileID.CorruptGrass;
                    WorldGen.PlaceTile(x, y, grassType, forced: true);
                }
                if (tile.TileType == TileID.Stone)
                {
                    WorldGen.PlaceTile(x, y, blockType, forced: true);
                }
            }
        }
    }
    private void GenerateAshotiTemple()
    {
        int radius = 80;
        int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
        int desertCenterY = GenVars.desertHiveLow - 200;
        Point arenaPoint = new Point(desertCenterX, desertCenterY);

        //Building the arena
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(TileID.LihzahrdBrick));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 2, radius - 2), new Actions.SetTile((ushort)ModContent.TileType<ChiseledStone>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 4, radius - 4), new Actions.SetTile((ushort)ModContent.TileType<NoxianBlock>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 6, radius - 6), new Actions.ClearTile());
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius / 2, radius / 2), new Actions.SetLiquid(type: LiquidID.Lava));


        //Decorate arena with walls
        for (int w = 0; w < 80; w++)
        {
            float progressOnCircle = w / 80f;
            float rot = progressOnCircle * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * radius;
            Point pointToWall = arenaPoint + vel.ToPoint();
            WorldUtils.Gen(pointToWall, new Shapes.Circle(4, 4), new Actions.PlaceWall(type: WallID.LihzahrdBrickUnsafe));
        }

        //Make Middle of the Temple
        int middleLength = 7;
        for (int m = 0; m < middleLength; m++)
        {
            Point offset = new Point(0, m * -43);
            Point tileToPlaceOn = arenaPoint + offset;

            if (m == middleLength - 1)
            {
                string structure = "Struct/AshotiTemple/TempleEntrance";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                tileToPlaceOn.Y -= 28;
                int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
            else
            {
                string structure = "Struct/AshotiTemple/TempleMiddle";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
        }
    }
    private void GenerateMineshaftTunnel()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int x = (int)Main.MouseWorld.X / 16;
        int y = (int)Main.MouseWorld.Y / 16;
        Point tilePoint = new Point(x, y);
        Point tileDirection = new Point(0, -1);
        int tunnel = Main.rand.Next(7, 25);
        VeilGen.GenerateMineshaftTunnel(tilePoint, tileDirection, tunnel);
    }
    private void GenerateLongCurveCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int x = (int)Main.MouseWorld.X / 16;
        int y = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 5;
        int caveSteps = 1000;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(x, y);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);
        VeilGen.GenerateSquiggleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }
    private void GenerateTreeCaves()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int x = (int)Main.MouseWorld.X / 16;
        int y = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 5;
        int caveSteps = 50;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(x, y);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);

        //Chance to open up
        int splitDenominator = 4;
        VeilGen.GenerateTreeCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps,
            splitDenominator);
    }

    private void GenerateOpenCaveClearing()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 15;
        int caveSteps = 500;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(tileX, tileY);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(20, 25);

        VeilGen.GenerateOpenCaveClearing(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateLongNoodleCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 5;
        int caveSteps = 500;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(tileX, tileY);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(4, 5);

        VeilGen.GenerateLongNoodleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }


    private void GenerateWiggleCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 5;
        int caveSteps = 120;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(tileX, tileY);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(4, 5);

        VeilGen.GenerateWiggleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateLinearCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 5;
        int caveSteps = 25;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(tileX, tileY);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(9, 10);

        VeilGen.GenerateLinearCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateNoodleCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 4;
        int caveSteps = 250;

        //Cave position in tiles
        Vector2 cavePosition = new Vector2(tileX, tileY);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(10, 20);

        VeilGen.GenerateNoodleCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }

    private void GenerateWormCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 4;
        int caveSteps = 64;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 caveStrength = new Vector2(5, 10);
        Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection());
        VeilGen.GenerateWormCave(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }
    private void GenerateVeinyCave()
    {
        Vector2 mouseWorld = Main.MouseWorld;
        int tileX = (int)Main.MouseWorld.X / 16;
        int tileY = (int)Main.MouseWorld.Y / 16;
        int caveWidth = 12;
        int caveSteps = 250;
        Vector2 cavePosition = new Vector2(tileX, tileY);
        Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
        Vector2 caveStrength = new Vector2(4, 5);
        VeilGen.GenerateVeinyCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps);
    }
}
