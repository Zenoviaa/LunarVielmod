using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public partial class VeilGen
{
    public static Random _random => MoonUtils.Rand;
    public static int Width => Main.maxTilesX;
    public static int Height => Main.maxTilesY;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle GetCenteredArea(int x, int y, int width, int height)
    {
        int centerX = x;
        int centerY = y;

        int left = centerX - width / 2;
        int right = centerX + width / 2;
        int top = centerY - height / 2;
        int bottom = centerY + height / 2;

        if (left < 0)
            left = 0;
        if (right >= Width)
            right = Width - 1;
        if (top < 0)
            top = 0;
        if (bottom >= Height)
            bottom = Height - 1;
        return new(left, top, (right - left), (bottom - top));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolid(int x, int y)
    {
        return MoonUtils.QuickSolid(x, y);
    }

    public static void ClearTile(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        tile.ClearTile();
        //Tiles[x, y] = 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Density(int x, int y, int size) => Density(x, y, size, size);
    public static float Density(int x, int y, int width, int height)
    {
        var area = GetCenteredArea(x, y, width, height);
        var maxTiles = (float)(area.Width * area.Height);
        var countingTiles = 0F;
        for (var tx = area.Left; tx < area.Right; tx++)
        {
            for (var ty = area.Top; ty < area.Bottom; ty++)
            {
                //var t = Tiles[tx, ty];
                if (IsSolid(tx, ty))
                    countingTiles++;
            }
        }
        return countingTiles / maxTiles;
    }

    public static void CreateSpike(int startingX, int startingY, Vector2 startingDirection, int spikeStartWidth, int spikeEndWidth, ushort tileType, int steps)
    {
        Vector2 inBetweenTile = new Vector2(startingX, startingY);
        var stepSize = 0.5F;

        Vector2 currentPosition = inBetweenTile;
        Vector2 perpDirection = startingDirection;
        perpDirection = perpDirection.RotatedBy(MathHelper.PiOver2);
        for (var s = 0; s < steps; s++)
        {
            //sample fnl for 
            var width = MathHelper.Lerp(spikeStartWidth, spikeEndWidth, (float)(s) / ((float)steps));

            for (var w = -width / 2; w < width / 2; w++)
            {
                Vector2 pos = currentPosition + perpDirection * w;
                var tile = TileUtilities.Clamp(pos.ToPoint());
                if (tile.X <= 0 || tile.X >= Width || tile.Y <= 0 || tile.Y >= Height)
                    continue;

                QuickPlaceTile(tile.X, tile.Y, tileType);
            }

            currentPosition += startingDirection * stepSize;
        }
    }

    public static void CreateSpikeyTunnel(int startingX, int startingY, Vector2 startingDirection, int steps, int size, float densityThreshold = 0.4F)
    {
        Vector2 inBetweenTile = new Vector2(startingX, startingY);
        var stepSize = 4;

        Vector2 currentPosition = inBetweenTile;
        for (var s = 0; s < steps; s++)
        {
            //sample fnl for 
            var density = Density((int)inBetweenTile.X, (int)inBetweenTile.Y, 30);
            if (density < densityThreshold)
            {
                break;
            }
            Carve((int)inBetweenTile.X, (int)inBetweenTile.Y, 64, size);
            startingDirection = startingDirection.RotatedBy(0.1f);
            inBetweenTile += startingDirection * stepSize;
        }
    }


    public static void Carve(int x, int y, int walkerSteps, int walkerWidth)
    {
        Point walkerPoint = new Point(x, y);
        var originalPoint = walkerPoint;
        for (int s = 0; s < walkerSteps; s++)
        {
            switch (Random.Shared.Next(4))
            {
                case 0:
                    walkerPoint.X--;
                    break;
                case 1:
                    walkerPoint.X++;
                    break;
                case 2:
                    walkerPoint.Y++;
                    break;
                case 3:
                    walkerPoint.Y--;
                    break;
            }
            walkerPoint = TileUtilities.Clamp(walkerPoint);
            ClearTile(walkerPoint.X, walkerPoint.Y);
            //Tiles[walkerPoint.X, walkerPoint.Y] = 1;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > walkerWidth || dy > walkerWidth)
            {
                walkerPoint = originalPoint;
            }
        }
    }
    public static void WalkerSpike(int x, int y, int walkerSteps, int walkerWidth, ushort tile)
    {
        Point walkerPoint = new Point(x, y);
        var originalPoint = walkerPoint;
        for (int s = 0; s < walkerSteps; s++)
        {
            switch (Random.Shared.Next(5))
            {
                case 0:
                    walkerPoint.X--;
                    break;
                case 1:
                    walkerPoint.X++;
                    break;
                default:
                case 2:
                    walkerPoint.Y++;
                    break;
                case 3:
                    walkerPoint.Y--;
                    break;
            }
            walkerPoint = TileUtilities.Clamp(walkerPoint);
            QuickPlaceTile(walkerPoint.X, walkerPoint.Y, tile);
            //Tiles[walkerPoint.X, walkerPoint.Y] = tile;
            //Tiles[walkerPoint.X, walkerPoint.Y] = 1;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > walkerWidth || dy > walkerWidth)
            {
                walkerPoint = originalPoint;
            }
        }
    }
    public static Vector2 NextRandomTunnelStartingDirection(Random random)
    {
        var direction = Vector2.UnitX;
        if (random.Next(0, 2) == 0)
            direction.X *= -1;
        direction= direction.RotatedBy(random.NextSingle() * 2.8F);
        return direction;
    }

    public static void DarkspaceBridges()
    {

        bool IsShimmer(int x, int y)
        {
            var tile = Main.tile[x, y];
            return tile.LiquidType == LiquidID.Shimmer && tile.LiquidAmount > 0;
            //return Main.tile[x, y].LiquidType == LiquidID.shim
            //return Tiles[x, y] == 4;
        }
        //TODO Generate bridges
        bool CanPlaceBridgeHere(int x, int y)
        {
            /*
            var tooClose = 100 * 100;
            //Do structure chec
            Rectangle myRect = RectangleFromBottomLeft(x, y, 64, 32);
            foreach (var rect in _structureRectangles)
            {
                if (rect.Intersects(myRect))
                    return false;

                var dq = Vector2.DistanceSquared(myRect.Center.ToVector2(), rect.Center.ToVector2());
                if (dq < tooClose)
                    return false;
            }
            */
            return true && IsShimmer(x, y);
        }

        void PlaceBridge(int x, int y)
        {
            //this will just be a structure load
            //but for now put a rectangle to simulate it
            //PlaceRectangleFromBottomLeft(x, y, 64, 32, 5);

        }

        //Get a random point in the darkspace
        Rectangle darkspaceRectangle = new Rectangle(100, Height / 2, Width - 200, Height / 2);
        var attempts = 0;
        var numBridges = 8;
        while (attempts < 10000 && numBridges > 0)
        {
            attempts++;
            var randX = _random.Next(darkspaceRectangle.Left, darkspaceRectangle.Right);
            var randY = _random.Next(darkspaceRectangle.Top, darkspaceRectangle.Bottom - 128);
            if (!CanPlaceBridgeHere(randX, randY))
                continue;

            var stepsDown = 100;
            while (stepsDown > 0)
            {
                if (IsSolid(randX, randY + 1))
                    break;
                else
                {
                    stepsDown--;
                    randY++;
                }
            }
            randY += 8;
            numBridges--;
            PlaceBridge(randX, randY);
        }
    }
    public static Vector2 PerpendicularDirectionToGround(int centerX, int centerY)
    {
        Vector2 center = new Vector2(centerX, centerY);
        Vector2 directionSum = Vector2.Zero;
        var count = 0F;
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                var dx = centerX + i;
                var dy = centerY + j;
                if (dx < 0 || dx >= Width || dy <= 0 || dy >= Height)
                    continue;
                //if(centerX + i < 0)
                if (!IsSolid(centerX + i, centerY + j))
                {
                    Vector2 otherPoint = new Vector2(centerX + i, centerY + j);
                    Vector2 direction = (otherPoint - center);
                    directionSum += direction;
                    count++;
                }
            }
        }


        directionSum.Normalize();
        return directionSum;
    }


    public static void CreateDarkspaceWalls(int darkspaceTop, int darkspaceBottom)
    {
        int padding = 1800;
        var genRand = WorldGen.genRand;
        ushort silkTileType = (ushort)ModContent.TileType<MiracleSilkTile>();
        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = true;
        for (int x = padding; x < Main.maxTilesX - padding; x++)
        {
            for (int y = darkspaceTop - 100; y < darkspaceBottom + 100; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                //tile type is more likely to fail than being exposed to the air, and is a cheaper check
                if (tile.TileType != TileID.Granite)
                    continue;

                //no need to do a bounds check here the darkspace can't possibly be that close to the edge.
                //WorldGen solid or slope checks if it's not a platform, we don't care if it is a platform or not
                //Removing that for a speed up, moonutils purely checks tilesolid.
                bool hasRight = !MoonUtils.QuickSolid(x + 1, y);
                bool hasLeft = !MoonUtils.QuickSolid(x - 1, y);
                bool hasTop = !MoonUtils.QuickSolid(x, y + 1);
                bool hasBottom = !MoonUtils.QuickSolid(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;


                if (WorldGen.TileIsExposedToAir(x, y))
                {
                    float proximity = MoonUtils.ProximityTo(x, Main.maxTilesX / 2, 2000);
                    int denom = (int)MathHelper.Lerp(500, 50, proximity);
                    if (genRand.NextBool(denom))
                    {
                        float strength = genRand.Next(7, 11);
                        int steps = genRand.Next(12, 20);
                        ushort tileType = (ushort)ModContent.TileType<SilkTile>();


                        WorldGen.OreRunner(x, y,
                           strength,
                            steps, tileType);


                        //We can get away with place tile here since we're only calling it once
                        //But maybe we should replace it anyway
                        //WorldGen.PlaceTile(x, y, ModContent.TileType<MiracleSilkTile>(), mute: true, forced: true);
                        tile.TileType = silkTileType;
                        tile.HasTile = true;
                        tile.TileFrameX = -1;
                        tile.TileFrameY = -1;
                    }
                }

                if (hasAny)
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new Point(x, y);
                    int steps = genRand.Next(1, 4);
                    Vector2 baseDirection = -Vector2.UnitY;
                    int wallCaveWidth = 3;

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(wallCaveWidth, wallCaveWidth),
                                new Actions.PlaceWall(WallID.GraniteUnsafe));
                        }

                        point += (baseDirection * wallCaveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }
        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = false;

    }

    public static void CreateDarkspaceNLake(int darkspaceTop, int darkspaceBottom, int darkspaceLeft, int darkspaceRight, int crystalLakeLeft, int crystalLakeRight)
    {
        ushort crystalLakeTile = TileID.Granite;
        ushort darkspaceStone = TileID.Granite;
        bool IsNotSolid(int x, int y)
        {
            return !MoonUtils.QuickSolid(x, y);
        }

        void SetDarkspaceStone(int x, int y)
        {
            QuickPlaceTile(x, y, darkspaceStone);
   //         Tiles[x, y] = 2;
        }

        void SetCrystalLake(int x, int y)
        {
            QuickPlaceTile(x, y, crystalLakeTile);
        }

        void SetShimmer(int x, int y)
        {
            var tile = Main.tile[x, y];
            tile.LiquidAmount = 255;
            tile.LiquidType = LiquidID.Shimmer;
            tile.HasTile = false;
         
            //Tiles[x, y] = 4;
        }

        //Move left to right,
        var caveHeight = 150;
        var caveSlant = 450;


        var darkspaceTopLow = darkspaceTop + 128;
        var darkspaceTopHigh = darkspaceTopLow - caveSlant;

        var darkspaceRockTopLow = darkspaceTopLow + 100;
        var darkspaceRockTopHigh = darkspaceTopHigh - 100;

        var darkspaceOscRange = 7;
        var darkspaceOscFrequency = 24;

        var darkspaceOscRangeSmall = 3;
        var darkspaceOscFrequencySmall = 57;

        var darkspaceOscRangeTiny = 2;
        var darkspaceOscFrequencyTiny = 135;

        var darkspaceTileRect = new Rectangle(darkspaceLeft, darkspaceTop, darkspaceRight - darkspaceLeft, darkspaceBottom - darkspaceTop);



        var randOffset = _random.NextSingle() * 6;
        var tops = new int[darkspaceRight - darkspaceLeft];

        FastNoiseLite fnl = new();
        fnl.SetSeed(_random.Next(1337236));
        fnl.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);


        var off4 = _random.Next(-8, 8);

        for (var x = darkspaceLeft; x < darkspaceRight; x++)
        {
            var localX = x - darkspaceLeft;
            var ratio = (localX) / (float)(darkspaceRight - darkspaceLeft);
            var off = MathF.Sin(ratio * darkspaceOscFrequency + randOffset) * darkspaceOscRange * 0.5f + darkspaceOscRange * 0.5f;
            var off2 = MathF.Cos(ratio * darkspaceOscFrequencySmall + randOffset) * darkspaceOscRangeSmall;
            var off3 = MathF.Sin(ratio * darkspaceOscFrequencyTiny + 0.3f + randOffset) * darkspaceOscRangeTiny;
            if (_random.Next(15) == 0)
                off4 = _random.Next(-8, 8);
            var n2 = fnl.GetNoise(x, 0);
            var n3 = fnl.GetNoise(x * 1.5F, 2);
            var off5 = (int)(n2 * 8);
            var off6 = (int)(n3 * 16);
            tops[localX] =
                (int)MathHelper.Lerp(darkspaceTopLow, darkspaceTopHigh, ratio) +
                (int)off +
                (int)off2 +
                (int)off3 +
                off4 +
                off5 +
                off6;


        }

        Point GetCenter(int x)
        {
            var top = tops[x];
            var bottom = top + caveHeight;
            var middle = (top + bottom) / 2;
            return new Point(darkspaceLeft + x, middle);
        }

        SavedGenerationParameters.DarkspaceBottomLeftCenter = GetCenter(0);
        SavedGenerationParameters.DarkspaceTopRightCenter = GetCenter(darkspaceRight - darkspaceLeft - 1);





        var size = (float)(darkspaceRight - darkspaceLeft);
        var sprayPaintCountPerTile = 6;
        var halfCaveHeight = caveHeight / 2;
        var tileManipulator = VeilGen.RedSetTileManipulator(SetDarkspaceStone);
        //Fill the cave with the spray paint textures
        for (var x = darkspaceLeft; x < darkspaceRight; x++)
        {
            var localX = x - darkspaceLeft;
            var top = tops[localX];
            var bottom = top + caveHeight;
            var middle = (top + bottom) / 2;
            VeilGen.SprayPaint(x, middle, tileManipulator);
            for (var n = 0; n < sprayPaintCountPerTile; n++)
            {
                var sprayMiddle = middle + _random.Next(-caveHeight * 2, caveHeight * 2);
                VeilGen.SprayPaint(x, sprayMiddle, tileManipulator);
            }
        }

        //Make the tentacles that randomly go around the biome
        for (var x = darkspaceLeft; x < darkspaceRight; x++)
        {
            var localX = x - darkspaceLeft;
            var top = tops[localX];
            var bottom = top + caveHeight;
            var middle = (top + bottom) / 2;
            var randVelocity = _random.NextVector2CircularEdge(1, 1);
            VeilGen.SetTentacle(fnl, SetDarkspaceStone, new Point(x, _random.Next(top, bottom)), randVelocity, 128, 16, 4);
        }

        for (var x = darkspaceLeft; x < darkspaceRight; x++)
        {
            var localX = x - darkspaceLeft;
            var top = tops[localX];
            var bottom = top + caveHeight;
            var ratio = (float)localX / size;
            var denom = (int)MathHelper.Lerp(0, 32, ratio);
            for (var y = top; y < bottom; y++)
            {
                ClearTile(x, y);
            }
        }


        //create the edges by doing a bump
        void MakeCap(int middleX, int capOffset, int capWidth, int top, int bottom)
        {
            middleX += darkspaceLeft;

            var columnSize = (float)(bottom - top);
            for (var y = top; y < bottom; y++)
            {
                var localY = y - top;
                var progress = (float)localY / columnSize;
                var xOffset = (int)MathHelper.Lerp(0, -capOffset, EasingFunction.QuadraticBump(progress));
                var edge = middleX + xOffset;
                var edgeLeft = edge - capWidth;
                for (var x = edgeLeft; x < edge; x++)
                {
                    if (x < 0 || x >= Width)
                        continue;
                    SetDarkspaceStone(x, y);
                }

            }

        }

        var middleX = 32;
        var leftTop = tops[middleX] - 32;
        var leftBottom = leftTop + caveHeight + 64;
        MakeCap(middleX, 32, 48, leftTop, leftBottom);


        middleX = tops.Length - 4;
        leftTop = tops[middleX] - 32;
        leftBottom = leftTop + caveHeight + 64;

        MakeCap(middleX, -32, 48, leftTop, leftBottom);


        //Place shimmer lakes

        #region Place Shimmer Lakes
        void PlaceShimmerLakes()
        {
            var numDarkspaceShimmerLakes = 64;
            var darkspaceShimmerLakeMinWidth = 32;
            var darkspaceShimmerLakeMaxWidth = 64;
            var darkspaceShimmerDepth = 48;

            var darkspaceShimmerOscRange = 12;
            var darkspaceShimmerOscFrequency = 7;

            for (var ns = 0; ns < numDarkspaceShimmerLakes; ns++)
            {
                //get a random point in the darkspace
                var topIndex = _random.Next(64, tops.Length - 64);
                var top = tops[topIndex];
                top += 2;

                var x = darkspaceLeft + topIndex;

                var failsafe = 0;
                while (failsafe < 1000 && IsNotSolid(x, top))
                {
                    failsafe++;
                    top++;
                }

                var shimmerLakeWidth = _random.Next(darkspaceShimmerLakeMinWidth, darkspaceShimmerLakeMaxWidth);
                var halfShimmerLakeWidth = (int)(shimmerLakeWidth / 2f);
                var darkspaceShimmerLakeLeft = x - halfShimmerLakeWidth;
                var darkspaceShimmerLakeRight = x + halfShimmerLakeWidth;
                var t = top;
                t -= 16;
                for (var dx = darkspaceShimmerLakeLeft; dx < darkspaceShimmerLakeRight; dx++)
                {
                    var localX = dx - darkspaceShimmerLakeLeft;
                    var ratio = localX / (float)shimmerLakeWidth;
                    var depth = darkspaceShimmerDepth * EasingFunction.QuadraticBump(ratio);
                    var osc = MathF.Sin(ratio * darkspaceShimmerOscFrequency) * darkspaceShimmerOscRange;
                    depth += osc;
                    for (var dy = t; dy < t + depth; dy++)
                    {
                        if (dx <= 0 || dx >= Width)
                            continue;
                        SetShimmer(dx, dy);
                    }
                }
            }
        }

        #endregion

        PlaceShimmerLakes();


        #region Generate Crystal Lake


        void GenerateCrystalLake()
        {
            var crystalLakeOscFrequency = 25;
            var crystalLakeOscRange = 15;

            var crystalLakeOscRangeSmall = 4;
            var crystalLakeOscFrequencySmall = 70;


            var crystalLakeBottomOscRangeTiny = 4;
            var crystalLakeBottomOscFrequencyTiny = 70;

            var crystalLakeOscSpikeFrequency = 75;
            var crystalLakeOscSpikeRange = 35;

            var crystalLakeHeight = 256;
            var crystalLakeOffset = 48;
            var crystalLakeDivetSize = 64;

            var crystalLakeSize = (float)(crystalLakeRight - crystalLakeLeft);

            var crystalLakeTop = darkspaceTopHigh - crystalLakeHeight - crystalLakeOffset;
            var crystalLakeBottom = crystalLakeTop + crystalLakeHeight;

            var crystalLakeRockTop = crystalLakeTop - 42;
            var crystalLakeRockBottom = crystalLakeBottom + 80;

            var crystalLakeShimmerHeight = 64;
            var crystalLakeShimmerLine = crystalLakeRockBottom - crystalLakeShimmerHeight;

            var crystalLakeTops = new int[(int)crystalLakeSize];
            var numBlotches = 1000;

            var setCrystalLakeManipulator = VeilGen.RedSetTileManipulator(SetCrystalLake);
            for(var n = 0; n < numBlotches; n++)
            {
                var randPoint = new Point();
                randPoint.X = _random.Next(crystalLakeLeft - 100, crystalLakeRight + 100);
                randPoint.Y = _random.Next(crystalLakeTop - 100, crystalLakeBottom + 100);
                VeilGen.SprayPaint(randPoint.X, randPoint.Y, setCrystalLakeManipulator);
            }
            


            for (var x = crystalLakeLeft; x < crystalLakeRight; x++)
            {
                var localX = x - crystalLakeLeft; ;
                var ratio = (float)localX / crystalLakeSize;
                var top = (int)MathHelper.Lerp(crystalLakeBottom, crystalLakeTop, EasingFunction.QuadraticBump(ratio));
                var bottom = crystalLakeBottom;
                var off = MathF.Sin(ratio * crystalLakeOscFrequency) * crystalLakeOscRange * 0.5f + crystalLakeOscRange * 0.5f;
                var off2 = MathF.Cos(ratio * crystalLakeOscFrequencySmall) * crystalLakeOscRangeSmall;
                var off5 = MathF.Cos(ratio * crystalLakeOscSpikeFrequency) * crystalLakeOscSpikeRange;
                top += (int)off;
                top += (int)off2;
                top += (int)off5;


                if (_random.Next(15) == 0)
                    off4 = _random.Next(-4, 4);
                top += off4;

                var n = fnl.GetNoise(x, 0);
                var n2 = fnl.GetNoise(x * 2, 0);
                var n3 = fnl.GetNoise(x * 3, 0);
                top += (int)(n * 64);
                top += (int)(n2 * 32);
                top += (int)(n3 * 8);
                for (var y = top; y < bottom; y++)
                {
                    if (y <= 0)
                        continue;

                    ClearTile(x, y);
                }

                crystalLakeTops[localX] = top;
                top = bottom;
                bottom = top + (int)MathHelper.Lerp(0, crystalLakeDivetSize, EasingFunction.QuadraticBump(ratio));
                var off3 = MathF.Sin(ratio * crystalLakeBottomOscFrequencyTiny) * crystalLakeBottomOscRangeTiny;
                bottom += (int)off3;

                if (_random.Next(15) == 0)
                    off4 = _random.Next(-4, 4);
                bottom += (int)off4;
                for (var y = top; y < bottom; y++)
                {
                    if (y <= 0)
                        continue;

                    ClearTile(x, y);
                }

            }

            for (var x = crystalLakeLeft; x < crystalLakeRight; x++)
            {
                var top = crystalLakeShimmerLine;
                var bottom = top + crystalLakeShimmerHeight;
                for (var y = top; y < bottom; y++)
                {
                    if (IsSolid(x, y))
                        continue;
                    SetShimmer(x, y);
                }
            }

            var numCrystalSpikes = 512;
            var attempts = 0;

            while (attempts < 20000 && numCrystalSpikes > 0)
            {
                attempts++;
                var randX = _random.Next(crystalLakeLeft + 1, crystalLakeRight - 1);

                var localX = randX - crystalLakeLeft;
                var top = crystalLakeTops[localX];

                WalkerSpike(randX, top, 64, 7, 6);
                var perpDirection = PerpendicularDirectionToGround(randX, top);
                CreateSpike(randX, top, perpDirection, 4, 1, 3, 46);
                numCrystalSpikes--;
            }


            numCrystalSpikes = 256;
            attempts = 0;
            while (attempts < 10000 && numCrystalSpikes > 0)
            {
                attempts++;
                var randX = _random.Next(crystalLakeLeft + 1, crystalLakeRight - 1);

                var localX = randX - crystalLakeLeft;
                var top = crystalLakeTops[localX];

                WalkerSpike(randX, top, 64, 7, 6);
                CreateSpike(randX, top, Vector2.UnitY, 4, 1, 6, 46);
                numCrystalSpikes--;
            }


        }

        VeilGen.SettleLiquids();
        GenerateCrystalLake();
        DarkspaceBridges();
        #endregion

        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 7, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        AutomataSmoothErase(new Rectangle(darkspaceLeft, darkspaceTop, darkspaceRight - darkspaceLeft, darkspaceBottom - darkspaceTop), in @params);
    }
}
