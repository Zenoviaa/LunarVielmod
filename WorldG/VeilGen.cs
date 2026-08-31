using Humanizer;
using Microsoft.VisualBasic;
using ReLogic.Content;
using ReLogic.Utilities;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Areas.Tundra.Snow.TilesSN;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.ZTileSystem;
using Stellamod.TilesNew.RainforestTiles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

/// <summary>
/// Collection of helper functions for manipulating textures.
/// </summary>
public static class TextureUtilities
{
    public static int GetPixelIndex(Texture2D texture, int x, int y)
    {
        return x + y * texture.Width;
    }

    public static Color GetPixelColor(Texture2D texture, int x, int y, Color[] pixels)
    {
        return pixels[GetPixelIndex(texture, x, y)];
    }
}

public enum PrefabPlacementType : byte
{
    FromTopLeft,
    FromTopCenter,
    FromCenter,
    FromTopRight
}

/// <summary>
/// Encapsulates a texture for world generation purposes, in most cases we're just going to use the texture as a mask for erasing tiles.
/// </summary>
public class GenerationPrefab : IDisposable
{
    public GenerationPrefab(string name, Asset<Texture2D> textureAsset)
    {
        Name = name;
        TextureAsset = textureAsset;
        Pixels = new Color[Width * Height];
        TextureAsset.Value.GetData(Pixels);
    }

    public string Name { get; private set; }
    public Color[] Pixels { get; private set; }
    public Asset<Texture2D> TextureAsset { get; private set; }
    public int Width => TextureAsset.Width();
    public int Height => TextureAsset.Height();

    public void Dispose()
    {
        TextureAsset = null;
    }

    public Color Sample(int localX, int localY)
    {
        return TextureUtilities.GetPixelColor(TextureAsset.Value, localX, localY, Pixels);
    }


    private void PasteEraseInner(in int originX, in int originY)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int tileX = originX + x;
                int tileY = originY + y;
                if (!WorldGen.InWorld(tileX, tileY))
                    continue;

                Color c = Sample(x, y);
                if (c.R > 125)
                {
                    Tile t = Main.tile[tileX, tileY];
                    t.ClearEverything();
                }
            }
        }
    }
    public void PasteErase(int originX, int originY, Point pixelOrigin)
    {
        originX -= pixelOrigin.X;
        originY -= pixelOrigin.Y;
        PasteEraseInner(originX, originY);
    }
    public void PasteErase(Point origin, PrefabPlacementType placementType)
    {
        PasteErase(origin.X, origin.Y, placementType);
    }
    public Rectangle GetBounds(int originX, int originY, PrefabPlacementType placementType)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        //Clamp to world bounds to prevent index out of bounds exceptions
        Rectangle rectangle = new Rectangle(originX, originY, Width, Height);
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
    public void PasteErase(int originX, int originY, PrefabPlacementType placementType)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        PasteEraseInner(originX, originY);
    }


}


[Autoload(Side = ModSide.Client)]
public class GenerationTextureManager : ModSystem
{
    public Dictionary<string, GenerationPrefab> Prefabs { get; private set; }
    public override void Load()
    {
        base.Load();
        Main.QueueMainThreadAction(LoadPrefabAssets);
    }
    public override void Unload()
    {
        base.Unload();
        Main.QueueMainThreadAction(UnloadPrefabAssets);
    }

    private void UnloadPrefabAssets()
    {

    }
    private void LoadPrefabAssets()
    {
        Prefabs = new Dictionary<string, GenerationPrefab>();
        Mod mod = Stellamod.Instance;
        foreach (var file in mod.GetFileNames())
        {
            if (file.Contains("WorldGenTextures/"))
            {
                string path = "Stellamod/" + file;
                path = path.Replace(".rawimg", "");
                Asset<Texture2D> worldGenTexture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
                GenerationPrefab prefab = new GenerationPrefab(Path.GetFileNameWithoutExtension(file), worldGenTexture);
                Console.WriteLine($"Prefab {prefab.Name}");
                Prefabs.Add(prefab.Name, prefab);
            }
        }
    }

    public GenerationPrefab GetPrefab(string name) => Prefabs[name];
}


public static class DungeonLayouter
{

    public static (int, int)[] GenerateLayout(int roomCount, UnifiedRandom rand)
    {
        int size = 16;
        bool[,] map = new bool[size, size];
        int[,] costs = new int[size, size];

        int halfSize = size / 2;
        int x = halfSize;
        int y = halfSize;
        int placedRooms = 0;
        int adjacentIndex = 0;

        (int, int)[] adjacent = new (int, int)[4];
        (int, int)[] roomsOnMap = new (int, int)[roomCount];


        void PlaceRoom(int x, int y)
        {
            if (map[x, y])
                return;

            //Increase costs of neighbouring nodes
            for (int a = 0; a < adjacentIndex; a++)
            {
                (int ax, int ay) = adjacent[a];
                costs[ax, ay] += 1;
            }

            map[x, y] = true;
            roomsOnMap[placedRooms] = (x, y);
            placedRooms++;
        }

        void PushAdjacent(int ax, int ay)
        {
            if (ax < 0 || ax >= size || ay <= 0 || ay >= size)
                return;

            if (costs[ax, ay] >= 2)
                return;

            if (map[ax, ay])
                return;

            adjacent[adjacentIndex++] = (ax, ay);
        }

        void FindAdjacents()
        {
            adjacentIndex = 0;
            PushAdjacent(x - 1, y);
            PushAdjacent(x + 1, y);
            PushAdjacent(x, y - 1);
            PushAdjacent(x, y + 1);
        }

        int snakeLength = 4;

        while (placedRooms < roomCount)
        {
            //Get Adjacent Points to current node
            FindAdjacents();

            //Place at the current position if possible
            PlaceRoom(x, y);

            //Recalculate the adjacent nodes since the costs have changed
            FindAdjacents();

            snakeLength--;

            //We've come to a dead end if the adjacent index = 0
            //In this case we should go to a different room and keep moving around
            if (adjacentIndex <= 0 || snakeLength <= 0)
            {
                snakeLength = rand.Next(4);
                //Just go to a random room we placed
                int positionToMoveTo = rand.Next(placedRooms);
                (x, y) = roomsOnMap[positionToMoveTo];
            }
            else
            {
                int positionToMoveTo = rand.Next(adjacentIndex);
                (int ax, int ay) = adjacent[positionToMoveTo];
                x = ax;
                y = ay;
            }
            //We now have all open spots next to this room
        }
        return roomsOnMap;
    }
}
public record struct CellularAutomataParams(int Steps, float RandomFill, int BirthLimit, int DeathLimit);

public partial class VeilGen
{
    public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;

    public static readonly Room[] MineshaftPrefabs = DungeonSaveUtility.GetDungeonPrefabs("Mineshafts");


    public static void QuickOrePatch(int x, int y, int tileType)
    {
        VeilGen.Walker(x, y, WorldGen.genRand.Next(50, 90), tileType, maxDist: 3);
    }
    public static void Walker(int x, int y, int steps, int tileType, int maxDist)
    {
        Point walkerPoint = new Point(x, y);
        Point originalPoint = walkerPoint;
        var genRand = WorldGen.genRand;
        for (int s = 0; s < steps; s++)
        {
            switch (genRand.Next(4))
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
            Tile tile = Main.tile[walkerPoint];
            tile.ClearTile();
            tile.HasTile = true;
            tile.TileFrameX = -1;
            tile.TileFrameY = -1;
            tile.TileType = (ushort)tileType;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > maxDist || dy > maxDist)
            {
                walkerPoint = originalPoint;
            }
        }

    }


    public static void PruneLonelyTiles(Rectangle areaRectangle)
    {
        for (int x = areaRectangle.Left; x < areaRectangle.Right; x++)
        {
            for (int y = areaRectangle.Top; y < areaRectangle.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileAbove = Main.tile[x, y - 1];
                Tile tileBelow = Main.tile[x, y + 1];
                Tile tileLeft = Main.tile[x - 1, y];
                Tile tileRight = Main.tile[x + 1, y];

                int count = 0;
                if (tileAbove.HasTile)
                    count++;
                if (tileBelow.HasTile)
                    count++;
                if (tileLeft.HasTile)
                    count++;
                if (tileRight.HasTile)
                    count++;


                if (count <= 1 && tile.HasTile)
                    tile.ClearTile();
            }
        }
    }


    /// <summary>
    /// Checks if a tile is exposed to air only on cardinal directions, it will not check diagonals
    /// This function assumes that it will not have an out of bounds exception, clamp boundaries before using it
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool IsTileExposedToAirCardinal(int x, int y)
    {
        return !Main.tile[x - 1, y].HasTile ||
                !Main.tile[x + 1, y].HasTile ||
                !Main.tile[x, y - 1].HasTile ||
                !Main.tile[x, y + 1].HasTile;
    }

    public static void WallWalker(int x, int y, int steps, int wallType, int maxDist, byte paint = 0)
    {
        Point walkerPoint = new Point(x, y);
        Point originalPoint = walkerPoint;
        var genRand = WorldGen.genRand;
        for (int s = 0; s < steps; s++)
        {
            switch (genRand.Next(4))
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
            Tile tile = Main.tile[walkerPoint];
            //not sure if i have to do framing manually
            //we'll find out
            tile.WallType = (ushort)wallType;
            tile.WallFrameX = -1;
            tile.WallFrameY = -1;
            tile.WallColor = paint;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > maxDist || dy > maxDist)
            {
                walkerPoint = originalPoint;
            }
        }
    }


    public static bool IsTileNearby(int x, int y, int distance, bool[] tileSet)
    {
        int left = x - distance;
        int top = y - distance;
        Rectangle rect = new Rectangle(left, top, distance * 2, distance * 2);
        rect = TileUtilities.Clamp(rect);
        for (int i = rect.Left; i < rect.Right; i++)
        {
            for (int j = rect.Top; j < rect.Bottom; j++)
            {
                Tile tile = Main.tile[i, j];
                if (!tile.HasTile)
                    continue;
                if (tileSet[tile.TileType])
                    return true;
            }
        }


        return false;
    }

    public static int CountAliveNeighbours(int x, int y, bool[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int dx = x + i;
                int dy = y + j;
                if (dx < 0 || dy < 0 || dx >= width || dy >= height)
                {
                    count++;
                }
                else if (map[dx, dy])
                {
                    count++;
                }
            }
        }
        return count;
    }
    public static bool[,] Step(bool[,] oldMap, in CellularAutomataParams @params)
    {
        int width = oldMap.GetLength(0);
        int height = oldMap.GetLength(1);
        bool[,] newMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbours = CountAliveNeighbours(x, y, oldMap);
                if (neighbours > @params.BirthLimit)
                {
                    newMap[x, y] = true;
                }
                else if (neighbours <= @params.DeathLimit)
                {
                    newMap[x, y] = false;

                }
                else
                {
                    newMap[x, y] = oldMap[x, y];
                }
            }
        }
        return newMap;
    }

    public static bool PlaceCavePrefab(int x, int y, UnifiedRandom genRand)
    {
        if (VeilGen.IsTileNearby(x, y, 50, TileSets.BlockMineshafts))
            return false;

        int maxCaveCount = 9;
        string caveToPlace = $"CavernCave_{genRand.Next(maxCaveCount) + 1}";
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab(caveToPlace);
        prefab.PasteErase(x, y, PrefabPlacementType.FromCenter);

        //Basically we're just sprinkling blotches everywhere and then smoothing it out with automata to create variation within the same room type
        //Honestly it's genius
        int left = x - prefab.Width / 2;
        int top = y - prefab.Height / 2;
        Rectangle rect = new Rectangle(left, top, prefab.Width, prefab.Height);
        rect = TileUtilities.Clamp(rect);
        int numBlotches = prefab.Width / 3;
        for (int n = 0; n < numBlotches; n++)
        {
            int randX = genRand.Next(rect.Left, rect.Right);
            int randY = genRand.Next(rect.Top, rect.Bottom);
            Walker(randX, randY, genRand.Next(60, 120), TileID.Stone, 5);
        }
        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 3, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        AutomataSmoothErase(rect, in @params);
        return true;
    }
    public static void PlaceDeepCuttingCave(Vector2 position, Vector2 initialDirection, int caveSteps, int walkerSteps, int walkerWidth, UnifiedRandom genRand, FastNoiseLite fnl)
    {
        void Carve(int x, int y)
        {
            Point walkerPoint = new Point(x, y);
            Point originalPoint = walkerPoint;
            for (int s = 0; s < walkerSteps; s++)
            {
                switch (genRand.Next(4))
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
                Tile tile = Main.tile[walkerPoint];
                tile.ClearTile();

                //Reset if walking too far
                int dx = Math.Abs(walkerPoint.X - originalPoint.X);
                int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
                if (dx > walkerWidth || dy > walkerWidth)
                {
                    walkerPoint = originalPoint;
                }
            }
        }
        //ALGO:
        //Pick a random point on the world
        //Use that as a starting coordinate
        //Move the tunnel in an initial direction, for us likely diagonally down
        //After each step, the tunnel turns its direction by a  small amount based on noise
        //At each step, do a walker algorithm to cut away at the terrain
        bool placedCave = false;
        for (int s = 0; s < caveSteps; s++)
        {
            Point tile = position.ToTileCoordinates();
            Carve(tile.X, tile.Y);
            position += initialDirection * walkerWidth * 2;
            float noise = fnl.GetNoise(s, 0);
            initialDirection = initialDirection.RotatedBy(noise * 0.1D);
            if (genRand.NextBool(caveSteps) && !placedCave)
            {
                placedCave = PlaceCavePrefab(tile.X, tile.Y, genRand);
            }
        }
    }

    public static void AutomataSmoothErase(Rectangle rectangle, in CellularAutomataParams @params)
    {
        bool[,] map = new bool[rectangle.Width, rectangle.Height];
        for (int x = rectangle.Left; x < rectangle.Right; x++)
        {
            for (int y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                int lx = x - rectangle.Left;
                int ly = y - rectangle.Top;
                map[lx, ly] = Main.tile[x, y].HasTile;
            }
        }
        map = AutomataSmooth(map, in @params);
        Erase(new Point(rectangle.X, rectangle.Y), map);
    }

    public static bool[,] AutomataSmooth(bool[,] map, in CellularAutomataParams @params)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        for (int s = 0; s < @params.Steps; s++)
        {
            map = Step(map, in @params);
        }

        //Remove tiles with only 1 neighbour
        bool[,] lessLonelyMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourCount = 0;
                for (int dx = -1; dx < 2; dx++)
                {
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (dx != 0 && dy != 0)
                            continue;
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX < 0 || newY < 0 || newX >= width || newY >= height)
                            neighbourCount++;
                        else if (map[newX, newY])
                            neighbourCount++;
                    }
                }

                if (neighbourCount <= 1)
                {
                    lessLonelyMap[x, y] = false;
                }
                else
                {
                    lessLonelyMap[x, y] = map[x, y];
                }
            }
        }

        return lessLonelyMap;
    }
    public static bool[,] CellularAutomataMap(int width, int height, in CellularAutomataParams @params, UnifiedRandom genRand)
    {
        bool[,] map = new bool[width, height];

        //First initialize the map with random values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = genRand.Next(0, 100) < @params.RandomFill;
            }
        }

        for (int s = 0; s < @params.Steps; s++)
        {
            map = Step(map, in @params);
        }


        //Remove tiles with only 1 neighbour
        bool[,] lessLonelyMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourCount = 0;
                for (int dx = -1; dx < 2; dx++)
                {
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (dx != 0 && dy != 0)
                            continue;
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX < 0 || newY < 0 || newX >= width || newY >= height)
                            neighbourCount++;
                        else if (map[newX, newY])
                            neighbourCount++;
                    }
                }

                if (neighbourCount <= 1)
                {
                    lessLonelyMap[x, y] = false;
                }
                else
                {
                    lessLonelyMap[x, y] = map[x, y];
                }
            }
        }

        return lessLonelyMap;
    }

    public static void Erase(Point topLeft, bool[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        Rectangle rect = new Rectangle(topLeft.X, topLeft.Y, width, height);
        rect = TileUtilities.Clamp(rect);
        for (int x = rect.Left; x < rect.Right; x++)
        {
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!map[x - rect.Left, y - rect.Top])
                {
                    tile.ClearTile();
                }

            }
        }
    }

    public static bool PlaceMineshaft(int x, int y) => PlaceMineshaft(new Point(x, y));
    public static (Rectangle rect, Room[] map) GenerateMineshaft(UnifiedRandom genRand)
    {
        (int, int)[] layout = DungeonLayouter.GenerateLayout(40, genRand);
        Point[] vertices = new Point[layout.Length];
        for (int v = 0; v < vertices.Length; v++)
        {
            vertices[v] = new Point(layout[v].Item1, layout[v].Item2);
        }

        DungeonChart simpleChart = DungeonChart.FromMap(layout);
        Room[] map = Dungeonizer.CreateDungeonFromChart(MineshaftPrefabs, simpleChart, genRand);
        Rectangle rectangle = Dungeonizer.GetDungeonBounds(map);
        return (rectangle, map);
    }

    public static void SettleLiquids()
    {
        Liquid.QuickWater(3);
        WorldGen.WaterCheck();
        int num = 0;
        Liquid.quickSettle = true;
        int num2 = 10;
        while (num < num2)
        {
            int num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
            num++;
            double num4 = 0.0;
            int num5 = num3 * 5;
            while (Liquid.numLiquid > 0)
            {
                num5--;
                if (num5 < 0)
                {
                    break;
                }

                double num6 = (num3 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (double)num3;
                if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num3)
                {
                    num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                }

                if (num6 > num4)
                {
                    num4 = num6;
                }
                else
                {
                    num6 = num4;
                }

                int num7 = 10;
                if (num > num7)
                {
                    num7 = num;
                }

                Liquid.UpdateLiquid();
            }

            WorldGen.WaterCheck();
        }

        Liquid.quickSettle = false;
    }
    public static bool PlaceMineshaft(Point startTile, Rectangle rectangle, Room[] map)
    {
        if (Structurizer.CanPlaceStructureHere(rectangle))
            return false;

        Point point = startTile;
        Point vectorToOrigin = (point - rectangle.Top().ToPoint());
        rectangle.Location += vectorToOrigin;
        //Main.NewText(map.Length);
        //Just a failsafe
        while (rectangle.Right().X >= Main.maxTilesX)
            rectangle.Location -= new Point(32, 0);

        int width = rectangle.Width;
        width -= 150;
        int height = rectangle.Height;

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


            /*
            if (VeilGen.IsTileNearby(bottomLeft.X, bottomLeft.Y, 25, TileSets.BlockMineshafts))
                continue;
            */
            Structurizer.ReadStruct(bottomLeft, room.prefab, Structurizer.DefaultTileBlend);
            Structurizer.ProtectStructure(bottomLeft, room.prefab);
        }
        return true;
    }
    public static bool PlaceMineshaft(Point startTile)
    {
        (int, int)[] layout = DungeonLayouter.GenerateLayout(42, Main.rand);
        Point[] vertices = new Point[layout.Length];
        for (int v = 0; v < vertices.Length; v++)
        {
            vertices[v] = new Point(layout[v].Item1, layout[v].Item2);
        }
        //      DungeonGenerationHelper
        // DungeonGenerationHelper.vertices = vertices;
        DungeonChart simpleChart = DungeonChart.FromMap(layout);
        Room[] map = Dungeonizer.CreateDungeonFromChart(MineshaftPrefabs, simpleChart, Main.rand);
        Rectangle rectangle = Dungeonizer.GetDungeonBounds(map);
        return PlaceMineshaft(startTile, rectangle, map);
    }
    public static float GetMarshHeight(float x)
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
  
    public static void GenerateMarshFoliage(Point startTile, int length)
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
            int height = (int)(GetMarshHeight(ratio) * mountainHeight);
            heights[x - startTile.X] = height;
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
        int numWaterBlotches = Main.rand.Next(10, 15);
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

        //Grass up the holes we just made
        for (int x = startTile.X; x < endTile.X; x++)
        {
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];
            for (int y = startTile.Y - height + 7; y < Main.maxTilesY / 2; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                bool touchingAir = WorldGen.TileIsExposedToAir(x, y);
                if (touchingAir && (tile.TileType == ModContent.TileType<RainforestGrass>()) && genRand.NextBool(2))
                {
                    Point point = new Point(x, y);
                    int steps = genRand.Next(1, 4);
                    Vector2 baseDirection = -Vector2.UnitY;
                    int caveWidth = 3;

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - caveWidth > 0 && point.X + caveWidth < Main.maxTilesX && point.Y + caveWidth < Main.maxTilesY && point.Y - caveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(caveWidth, caveWidth),
                                new Actions.PlaceWall(WallID.JungleUnsafe));
                        }

                        point += (baseDirection * caveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }
    }
    public static void GenerateMarsh(Point startTile, int length)
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
            int height = (int)(GetMarshHeight(ratio) * mountainHeight);
            heights[x - startTile.X] = height;
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startTile.Y - y, grassTileType);
            }
        }
    }
    public static bool IsAir(int x, int y, int w)
    {
        for (int k = 0; k < w; k++)
        {
            Tile tile = Framing.GetTileSafely(x + k, y);
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                return false;
        }

        return true;
    }

    public static bool IsRainforestTreeGround(int x, int y, int w)
    {
        for (int k = 0; k < w; k++)
        {
            Tile tile = Framing.GetTileSafely(x + k, y);
            if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && (tile.TileType == ModContent.TileType<RainforestGrass>())))
                return false;

            Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
            if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                return false;
        }

        return true;
    }
    public static void PlaceMultitile(Point16 position, int type, int style = 0)
    {
        var data = TileObjectData.GetTileData(type, style); //magic numbers and uneccisary params begone!

        if (position.X + data.Width > Main.maxTilesX || position.X < 0)
            return; //make sure we dont spawn outside of the world!

        if (position.Y + data.Height > Main.maxTilesY || position.Y < 0)
            return;

        int xVariants = 0;
        int yVariants = 0;

        if (data.StyleHorizontal)
            xVariants = Main.rand.Next(data.RandomStyleRange);
        else
            yVariants = Main.rand.Next(data.RandomStyleRange);

        for (int x = 0; x < data.Width; x++) //generate each column
        {
            for (int y = 0; y < data.Height; y++) //generate each row
            {
                Tile tile = Framing.GetTileSafely(position.X + x, position.Y + y); //get the targeted tile
                tile.TileType = (ushort)type; //set the type of the tile to our multitile

                int yHeight = 0;
                for (int k = 0; k < data.CoordinateHeights.Length; k++)
                {
                    yHeight += data.CoordinateHeights[k] + data.CoordinatePadding;
                }

                tile.TileFrameX = (short)((x + data.Width * xVariants) * (data.CoordinateWidth + data.CoordinatePadding)); //set the X frame appropriately
                tile.TileFrameY = (short)(y * (data.CoordinateHeights[y > 0 ? y - 1 : y] + data.CoordinatePadding) + yVariants * yHeight); //set the Y frame appropriately
                tile.HasTile = true; //activate the tile
            }
        }
    }
    public static void PlaceMangroveTrees(int treex, int treey, int height)
    {

        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<MangroveTree>(), true, true);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y == height - 1 && x == 1)
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<MangroveTreeTop>(), true, true);
                }
                else
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<MangroveTree>(), true, true);
                }
            }
        }

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.TileFrame(treex + x, treey + y);
            }
        }
    }
    public static void PlaceBigTrees<TreeTrunk, TreeTop>(int treex, int treey, int height)
        where TreeTrunk : ModTile
        where TreeTop : ModTile
    {

        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<TreeTrunk>(), true, true);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y == height - 1 && x == 1)
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<TreeTop>(), true, true);

                }
                else
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<TreeTrunk>(), true, true);
                }
            }
        }

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.TileFrame(treex + x, treey + y);
            }
        }
    }
    public static void PlaceTrees<TreeTrunk, TreeTop>(int treex, int treey, int height)
        where TreeTrunk : ModTile
        where TreeTop : ModTile
    {
        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<TreeTrunk>(), true, true);
        for (int y = 0; y < height; y++)
        {
            if (y == height - 1)
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<TreeTop>(), true, true);
            }
            else
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<TreeTrunk>(), true, true);

            }

        }

        for (int y = 0; y < (height + 2); y++)
        {
            WorldGen.TileFrame(treex, treey + y);
        }
    }
    public static void PlaceAcaciaTrees(int treex, int treey, int height)
    {
        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<AcaciaTree>(), true, true);
        for (int y = 0; y < height; y++)
        {
            if (y == height - 1)
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<AcaciaTreeTop>(), true, true);
            }
            else
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<AcaciaTree>(), true, true);

            }

        }

        for (int y = 0; y < (height + 2); y++)
        {
            WorldGen.TileFrame(treex, treey + y);
        }
    }
    public static void PlaceRaintrees(int treex, int treey, int height)
    {
        treey -= 1;

        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        PlaceMultitile(new Point16(treex, treey - 1), ModContent.TileType<RainforestTreeBase>());

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(treex + x, treey - (y + 2), ModContent.TileType<RainforestTree>(), true, true);
            }
        }

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.TileFrame(treex + x, treey + y);
            }
        }
    }

    public static void GenerateIceSpike(Vector2 cavePosition, double width, Vector2D endOffset, ushort tileId = TileID.IceBlock)
    {
        WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Tail(width, endOffset), Actions.Chain(new GenAction[]
        {
                new Actions.SetTile(tileId),
        }));
    }

    public static void GenerateFallingIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 pullDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        Vector2 caveVelocity = baseCaveDirection;
        ushort[] wallTypes = new ushort[]
        {
            WallID.SnowWallUnsafe,
            WallID.IceUnsafe
        };

        Vector2 pullVelocity = pullDirection;
        Vector2 startVelocity = baseCaveDirection;
        float sharpness = 1f;
        int ignoreTile = ModContent.TileType<AbyssalDirt>();
        for (int s = 0; s < caveSteps; s++)
        {
            float radiansOffset = MathF.Sin(s * 0.5f) * MathHelper.ToRadians(45);
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
            }

            //Place Walls
            for (int w = 0; w < 5; w++)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                if (genRand.NextBool(2))
                {
                    wallType = WallID.IceUnsafe;
                }

                Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                Vector2 wallPosition = cavePosition + wallVelocity;
                WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }

    public static void GenerateIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        Vector2 caveVelocity = baseCaveDirection;
        ushort[] wallTypes = new ushort[]
        {
            WallID.SnowWallUnsafe,
            WallID.IceUnsafe
        };

        int ignoreTile = ModContent.TileType<AbyssalDirt>();
        for (int s = 0; s < caveSteps; s++)
        {
            float radiansOffset = MathF.Sin(s * 0.5f) * MathHelper.ToRadians(45);
            Vector2 shiftedVelocity = baseCaveDirection.RotatedBy(radiansOffset);
            caveVelocity = shiftedVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
            }

            //Make Stalagtites
            if (genRand.NextBool(2))
            {
                Vector2D endOffset = new Vector2D(
                    genRand.Next(-10, 10),
                    genRand.Next(-20, -3));
                Vector2 spikePosition = cavePosition;
                spikePosition += new Vector2(0, -10);
                GenerateIceSpike(spikePosition, width: 25, endOffset);
            }

            //Make Stalagmites
            if (genRand.NextBool(4))
            {
                Vector2D endOffset = new Vector2D(
                    genRand.Next(-10, 10),
                    genRand.Next(3, 7));
                Vector2 spikePosition = cavePosition;
                spikePosition += new Vector2(0, 15);
                GenerateIceSpike(spikePosition, width: 15, endOffset);
            }

            //Place Walls
            for (int w = 0; w < 5; w++)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                if (genRand.NextBool(2))
                {
                    wallType = WallID.IceUnsafe;
                }

                Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                Vector2 wallPosition = cavePosition + wallVelocity;
                WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }

    public static void PlaceMarble(Point granitePoint, Vector2 radiusSize, int caveWidth = 5)
    {
        var genRand = WorldGen.genRand;
        int maxRadius = (int)radiusSize.Y;
        int radius = genRand.Next((int)radiusSize.X, (int)radiusSize.Y);
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

        Vector2 cavePosition = new Vector2(granitePoint.X, granitePoint.Y) - new Vector2(radius, radius / 4);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitX;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);

        //Chance to open up
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

    public static void PlaceGranite(Point granitePoint, Vector2 radiusSize, int caveWidth = 5)
    {
        var genRand = WorldGen.genRand;


        int radius = genRand.Next((int)radiusSize.X, (int)radiusSize.Y);
        float sizeMultiplier = radius / radiusSize.Y;
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

        Vector2 cavePosition = new Vector2(granitePoint.X, granitePoint.Y) - new Vector2(radius / 4, radius);

        //Starting cave direction
        Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

        //How much the tile runner is gonna carve out
        Vector2 caveStrength = new Vector2(12, 14);

        //Chance to open up
        int caveSteps = (int)(50f * sizeMultiplier);
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
                if (genRand.NextBool(3))
                {
                    WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(4, 4),
                        new Actions.SetLiquid(type: LiquidID.Water));
                }

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += newVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateFigure8Cave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection;
        pullDirection.X = -baseCaveDirection.X;
        pullDirection.Y = 1;

        Vector2 targetPosition = caveVelocity + pullDirection;
        Vector2 startPullDirection = pullDirection;
        float sharpness = 3f;
        float counter = 0;
        float target = 100;
        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - cavePosition).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;
            if (counter < 20f)
            {
                pullDirection = Vector2.Lerp(startPullDirection, Vector2.Zero, counter / 20f);
                targetPosition = cavePosition + pullDirection;
                counter++;
            }

            if (counter > target)
            {
                target = genRand.Next(100, 150);
                targetPosition.X = -targetPosition.X;
                startPullDirection = pullDirection;
                counter = 0;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                */

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateSimpleCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (shouldBreak)
            {
                break;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                Point wallPoint = cavePosition.ToPoint();
                WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(tileToPlace),
                    new Actions.Smooth(true)
                }));
            }

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateDuneHole(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        ushort t = (ushort)tileToPlace;
        for (int j = 0; j < caveSteps; j++)
        {
            counter++;
            breakStrength *= 0.9995f;


            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldUtils.Gen(new Point((int)cavePosition.X, (int)cavePosition.Y),
                                    new Shapes.Circle(8, 8), new Actions.ClearTile());
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
    public static void GenerateDuneHoleEdges(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        ushort t = (ushort)tileToPlace;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;


            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                if (j > 6)
                {


                    WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                                          genRand.NextFloat(breakStrength.X, breakStrength.Y),
                                          genRand.Next(4, 5), tileToPlace, addTile);
                }


            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
    public static void GenerateDuneCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        ushort t = (ushort)tileToPlace;
        for (int j = 0; j < caveSteps; j++)
        {
            if (t == TileID.Sandstone && j < 16)
                continue;
            counter++;
            breakStrength *= 0.9995f;


            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {

                if (tileToPlace == -1)
                {
                    WorldUtils.Gen(new Point((int)cavePosition.X, (int)cavePosition.Y),
                        new Shapes.Circle(8, 8), new Actions.ClearTile());
                }
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace, addTile);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public record struct EdgeDecorationParameters
    {
        public required Rectangle tileBounds;
        public required List<int> targetTileTypes;
        public required int denom;
        public required ushort[] zTileTypes;
        public required int zLayer;
        public required ZRenderLayer renderLayer;
    }

    public static void KillZTilesInArea(Rectangle tileBounds)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        zTileMap.KillAnyArea(tileBounds);
    }

  

    public static void QuickPlaceTile(int x, int y, ushort tileType)
    {
        Tile tile = Main.tile[x, y];
        tile.HasTile = true;
        tile.TileFrameX = -1;
        tile.TileFrameY = -1;
        tile.TileType = tileType;
    }


    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace, addTile);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
    public static void GenerateDuneCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

            if (shouldBreak)
                break;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;


            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
        }
    }

    public static void GenerateStraightCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;


            if (shouldBreak)
            {
                break;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                Point wallPoint = cavePosition.ToPoint();
                WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(tileToPlace),
                    new Actions.Smooth(true)
                }));
            }

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateStraightCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

            if (shouldBreak)
                break;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;


            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
        }
    }

    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

            if (shouldBreak)
                break;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;


            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
        }
    }

    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
    public static void GenerateSquiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;

    }

    public static void GenerateCavernousCave1(Vector2 caveOrigin, Vector2 caveInitialDirection, int caveWidth, int caveSteps)
    {
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        float maxRadianOffset = MathHelper.ToRadians(40);
        Vector2 cavePosition = caveOrigin;

        var genRand = WorldGen.genRand;
        Vector2 breakStrength = new Vector2(10, 25);
        float strength = genRand.NextFloat(breakStrength.X, breakStrength.Y);
        float offset = genRand.NextFloat(0, 1000);
        for (int n = 0; n < caveSteps; n++)
        {
            float noiseSample = fastNoiseLite.GetNoise(n * 4f + offset, 0);
            float rotation = noiseSample * maxRadianOffset;
            Vector2 caveDirection = caveInitialDirection.RotatedBy(rotation);
            cavePosition += caveDirection * 4;


            float ratio = n / (float)caveSteps;
            float extraWidth = MathHelper.Lerp(0, caveWidth, EasingFunction.QuadraticBump(ratio));
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    strength + genRand.NextFloat(-2f, 2f) + extraWidth,
                    genRand.Next(4, 5), -1);*/
            }

            //      Main.NewText(noiseSample);
        }

        cavePosition = caveOrigin;
        for (int n = 0; n < caveSteps; n++)
        {
            float noiseSample = fastNoiseLite.GetNoise(n * 4f + offset, 0);
            float rotation = noiseSample * maxRadianOffset;
            Vector2 caveDirection = caveInitialDirection.RotatedBy(rotation);
            cavePosition += caveDirection * 3;

            float ratio = n / (float)caveSteps;
            float extraWidth = MathHelper.Lerp(0, caveWidth, EasingFunction.QuadraticBump(ratio));
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                 extraWidth,
                genRand.Next(4, 5), TileID.Stone);
            }
        }
    }

    public static void GenerateLongCurveCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;
        Vector2 pullDirection;
        pullDirection.X = -baseCaveDirection.X;
        pullDirection.Y = 1;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = caveVelocity;

        float sharpness = 10;
        float counter = 0;
        float target = genRand.Next(50, 200);
        float direction = 1;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (counter > target)
            {
                target = genRand.Next(50, 200);
                float mult = direction % 2 == 0 ? 1 : 0;
                pullVelocity = startVelocity.RotatedBy(MathHelper.ToRadians(-180 * mult));
                direction++;
                counter = 0;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                */

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateFishCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        float i = cavePosition.X;
        for (int j = 0; j < caveSteps; j++)
        {

            //1. Have Position

            //The default direction

            Vector2 caveDirection = baseCaveDirection;


            //Sample the noise
            float sample = fastNoiseLite.GetNoise(cavePosition.X, j / 50f);
            float caveOffsetAngleAtStep = sample * MathHelper.ToRadians(90);


            //Rotate based on the noise
            caveDirection = caveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);*/
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateVirulentCave(Vector2 cavePosition,
        Vector2 seedPosition,
        Vector2 baseCaveDirection,
        Vector2 caveStrength,
        int caveWidth,
        int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();
        Vector2 caveVelocity = baseCaveDirection;
        float sharpness = 1f;
        for (int j = 0; j < caveSteps; j++)
        {
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (seedPosition - cavePosition).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }


    public static void GenerateJungleTreeCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps,
        int splitSteps,
        int splitDenominator)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 1;
        int counter = 1;

        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                targetPosition = targetPosition.RotatedByRandom(MathHelper.ToRadians(15));
            }

            if (genRand.NextBool(splitDenominator) && j > 4)
            {
                int clearingCaveWidth = caveWidth / 2;
                int clearingCaveSteps = splitSteps;

                //Cave position in tiles
                Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                //Starting cave direction
                float dir = counter % 2 == 0 ? 1 : -1;
                counter++;
                Vector2 clearingCaveDirection = baseCaveDirection.RotatedBy(dir * MathHelper.PiOver2);

                //How much the tile runner is gonna carve out
                Vector2 clearingCaveStrength = caveStrength * 0.5f;

                VeilGen.GenerateJungleTreeCaves(clearingPosition,
                    clearingCaveDirection,
                    clearingCaveStrength,
                    clearingCaveWidth,
                    clearingCaveSteps,
                    genRand.Next(splitSteps / 2, splitSteps),
                    splitDenominator * 640);
            }

            /*
            Point cavePoint = cavePosition.ToPoint();
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(cavePoint, new Shapes.Rectangle(20, 10), new Actions.TileScanner(TileID.Mud, TileID.Stone).Output(dictionary));
            int mudCount = dictionary[TileID.Mud];
            int stoneCount = dictionary[TileID.Stone];
            if(stoneCount > mudCount)
            {
                return;
            }
            */
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }



            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateTreeCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps,
        int splitDenominator)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 1;
        int counter = 1;
        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                targetPosition = targetPosition.RotatedByRandom(MathHelper.ToRadians(30));
            }

            if (genRand.NextBool(splitDenominator) && j > 4)
            {
                int clearingCaveWidth = caveWidth / 2;
                int clearingCaveSteps = caveSteps;

                //Cave position in tiles
                Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                //Starting cave direction
                float dir = counter % 2 == 0 ? 1 : -1;
                counter++;
                Vector2 clearingCaveDirection = baseCaveDirection.RotatedBy(dir * MathHelper.PiOver2);

                //How much the tile runner is gonna carve out
                Vector2 clearingCaveStrength = caveStrength * 0.5f;

                VeilGen.GenerateTreeCaves(clearingPosition,
                    clearingCaveDirection,
                    clearingCaveStrength,
                    clearingCaveWidth,
                    clearingCaveSteps,
                    splitDenominator * 640);
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                */

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateStraightCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 1;
        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
            
                */
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }

    public static void GenerateHighCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps,
        int clearingDenominator)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 9;

        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (genRand.NextBool(clearingDenominator) && j > caveSteps / 2)
            {
                int clearingCaveWidth = 15;
                int clearingCaveSteps = 500;

                //Cave position in tiles
                Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                //Starting cave direction
                Vector2 clearingCaveDirection = caveVelocity;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                //How much the tile runner is gonna carve out
                Vector2 clearingCaveStrength = new Vector2(20, 25);

                VeilGen.GenerateOpenCaveClearing(clearingPosition,
                    clearingCaveDirection,
                    clearingCaveStrength,
                    clearingCaveWidth,
                    clearingCaveSteps);
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
            
                */
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);

            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }


    public static void GenerateOpenCaveClearing(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        Vector2 caveVelocity = baseCaveDirection;
        Vector2 baseCavePosition = cavePosition;
        for (int j = 0; j < caveSteps; j++)
        {
            if (genRand.NextBool(4))
            {
                caveVelocity = Main.rand.NextVector2Circular(1, 1);
            }
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                // ShapeData shapeData = new ShapeData();
                // Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                // WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(8, 9), -1);
            }

            // Update the cave position.
            cavePosition = baseCavePosition + caveVelocity * caveWidth;
        }
    }

    public static void GenerateLongNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 9;
        for (int j = 0; j < caveSteps; j++)
        {

            //1. Have Position
            //  caveDirection = Vector2.Lerp(caveDirection, pullDirection, 0.05f);


            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);*/
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }


    public static void GenerateVeinyCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y, MathF.Sin(j * 0.05f) * 10 +
                    genRand.NextFloat(2, 5),
                    genRand.Next(5, 10), -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }
    public static void GenerateLinearCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 50f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next((int)caveStrength.X, (int)caveStrength.Y),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }
    public static float TilePercentNoAir(Point tilePoint, Rectangle size, params ushort[] tileIDs)
    {
        int count = 0;
        int width = size.Width;
        int height = size.Height;
        for (int x = tilePoint.X; x < tilePoint.X + width; x++)
        {
            if (x < 0)
                continue;
            if (x >= Main.maxTilesX)
                continue;

            for (int y = tilePoint.Y; y > tilePoint.Y - height; y--)
            {

                if (y < 0)
                    continue;
                if (y >= Main.maxTilesY)
                    continue;

                Tile tile = Main.tile[x, y];
                for (int t = 0; t < tileIDs.Length; t++)
                {
                    int tileID = tileIDs[t];
                    if (tile.HasTile)
                    {
                        count++;
                    }
                }
            }
        }

        int tileM = width * height;
        float tilePercent = count / (float)tileM;
        return tilePercent;
    }

    public static float TilePercent(Point tilePoint, Rectangle size, params ushort[] tileIDs)
    {
        int count = 0;
        int width = size.Width;
        int height = size.Height;
        for (int x = tilePoint.X; x < tilePoint.X + width; x++)
        {
            if (x < 0)
                continue;
            if (x >= Main.maxTilesX)
                continue;

            for (int y = tilePoint.Y; y > tilePoint.Y - height; y--)
            {

                if (y < 0)
                    continue;
                if (y >= Main.maxTilesY)
                    continue;

                Tile tile = Main.tile[x, y];
                for (int t = 0; t < tileIDs.Length; t++)
                {
                    int tileID = tileIDs[t];
                    if (!WorldGen.SolidTile(x, y))
                    {
                        count++;
                    }

                    if (tile.HasTile && tile.TileType == tileID)
                    {
                        count++;
                    }
                }
            }
        }

        int tileM = width * height;
        float tilePercent = count / (float)tileM;
        return tilePercent;
    }
    public static void GenerateColosseum(Point tilePoint, StructureMap structureMap = null)
    {
        var genRand = WorldGen.genRand;
        string GetMiniStructurePath()
        {
            int num = genRand.Next(1, 3);
            string baseStructurePath = $"Struct/Colosseum/SquareHouse{num}";
            return baseStructurePath;
        }

        string GetStructurePath()
        {
            int num = genRand.Next(1, 5);
            string baseStructurePath = $"Struct/Colosseum/House{num}";
            return baseStructurePath;
        }

        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };

        void Arena(Point tilePoint)
        {
            var structure = "Struct/Colosseum/TheColosseum";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
            for (int beamX = rectangle.Location.X;
             beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;

                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
        }
        void PlaceAir(Point tilePoint)
        {
            string structure = "Struct/Colosseum/Elevator";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
        }

        void PlaceBigStructure(Point tilePoint)
        {
            string structure = GetStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

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

            for (int beamX = rectangle.Location.X;
                beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
        }
        void PlaceSmallStructure(Point tilePoint)
        {
            string structure = GetMiniStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

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
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);

            for (int beamX = rectangle.Location.X;
                beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
        }
        PlaceAir(tilePoint + new Point(48, 100));
        PlaceAir(tilePoint + new Point(50, 100));
        int upOffset = 18;
        PlaceBigStructure(tilePoint);
        PlaceBigStructure(tilePoint + new Point(24, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));

        tilePoint.Y -= upOffset;
        PlaceBigStructure(tilePoint);
        PlaceBigStructure(tilePoint + new Point(24, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));


        tilePoint.Y -= upOffset;
        PlaceBigStructure(tilePoint + new Point(4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 - 4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24 - 4, 0));

        tilePoint.Y -= upOffset;
        PlaceSmallStructure(tilePoint + new Point(34, 0));
        PlaceSmallStructure(tilePoint + new Point(52, 0));

        tilePoint.Y -= upOffset;
        PlaceSmallStructure(tilePoint + new Point(16, 1));
        PlaceSmallStructure(tilePoint + new Point(34, 1));
        PlaceSmallStructure(tilePoint + new Point(52, 1));
        PlaceSmallStructure(tilePoint + new Point(70, 1));

        tilePoint.Y -= upOffset;
        Arena(tilePoint + new Point(-21, -1));

        /*
        //Layer 6
      
        */
    }
    public static void GenerateMineshaftTunnel(Point tilePoint, Point tileDirection, int tunnelLength)
    {
        var genRand = WorldGen.genRand;
        string GetStructurePath()
        {
            int num = genRand.Next(1, 15);
            string baseStructurePath = $"Struct/Catacombs/CaRoom{num}";
            return baseStructurePath;
        }

        int[] tileBlend = new int[]
        {

        };

        for (int t = 0; t < tunnelLength; t++)
        {
            string structure = GetStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            if (TilePercent(tilePoint, rectangle, TileID.Dirt, TileID.Stone) < 0.7f)
            {
                break;
            }

            int[] chestIndices = Structurizer.ReadStruct(tilePoint, structure, null);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();
                    if (genRand.NextBool(2))
                    {
                        switch (genRand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.MagicMirror, 1));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.HermesBoots, 1));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FlareGun, 1));
                                itemsToAdd.Add((ItemID.Flare, genRand.Next(20, 30)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.Mace, 1));
                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.LavaCharm, 1));
                                break;
                            case 5:
                                itemsToAdd.Add((ItemID.Aglet, 1));
                                break;
                        }
                    }

                    itemsToAdd.Add((ModContent.ItemType<MinersGold>(), genRand.Next(3, 5)));
                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.Bomb, genRand.Next(3, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.Torch, genRand.Next(3, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.SpelunkerGlowstick, genRand.Next(5, 10)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.LesserHealingPotion, genRand.Next(2, 4)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.LesserManaPotion, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.SpelunkerPotion, genRand.Next(2, 4)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.PotionOfReturn, genRand.Next(1, 3)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.HunterPotion, genRand.Next(1, 3)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.MiningPotion, genRand.Next(1, 3)));
                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.TrapsightPotion, genRand.Next(1, 3)));
                                break;
                            case 5:
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, genRand.Next(1, 3)));
                                break;
                        }
                    }
                    for (int n = 0; n < 4; n++)
                    {
                        if (genRand.NextBool(4))
                        {
                            switch (genRand.Next(0, 7))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.Amethyst, genRand.Next(3, 10)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.Emerald, genRand.Next(3, 10)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.Sapphire, genRand.Next(3, 10)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.Topaz, genRand.Next(3, 10)));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.Ruby, genRand.Next(3, 10)));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ItemID.Diamond, genRand.Next(3, 10)));
                                    break;
                                case 6:
                                    itemsToAdd.Add((ItemID.Amber, genRand.Next(3, 10)));
                                    break;
                            }
                        }
                    }

                    for (int n = 0; n < 4; n++)
                    {
                        if (genRand.NextBool(4))
                        {
                            switch (genRand.Next(0, 8))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.CopperOre, genRand.Next(3, 10)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.TinOre, genRand.Next(3, 10)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.IronOre, genRand.Next(3, 10)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.LeadOre, genRand.Next(3, 10)));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.SilverOre, genRand.Next(3, 10)));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ItemID.TungstenOre, genRand.Next(3, 10)));
                                    break;
                                case 6:
                                    itemsToAdd.Add((ItemID.GoldOre, genRand.Next(3, 10)));
                                    break;
                                case 7:
                                    itemsToAdd.Add((ItemID.PlatinumOre, genRand.Next(3, 10)));
                                    break;
                            }
                        }
                    }

                    if (genRand.NextBool(1))
                    {
                        switch (genRand.Next(3))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.CopperCoin, genRand.Next(45, 100)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.SilverCoin, genRand.Next(45, 100)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.GoldCoin, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(100))
                    {
                        itemsToAdd.Add((ItemID.MiningHelmet, 1));
                        itemsToAdd.Add((ItemID.MiningPants, 1));
                        itemsToAdd.Add((ItemID.MiningShirt, 1));
                    }

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


            Structurizer.ProtectStructure(tilePoint, structure);

            if (tileDirection.X != 0)
            {
                tilePoint.X += tileDirection.X * rectangle.Width;
            }
            else if (tileDirection.Y != 0)
            {
                tilePoint.Y += tileDirection.Y * (rectangle.Height + 1);
            }

            if (genRand.NextBool(4) && tileDirection != new Point(0, -1))
            {
                GenerateMineshaftTunnel(tilePoint, new Point(0, -1), tunnelLength / 2);
            }
            else if (genRand.NextBool(2) && tileDirection != new Point(1, 0))
            {
                GenerateMineshaftTunnel(tilePoint, new Point(1, 0), tunnelLength / 2);
            }
        }
    }

    public static void GenerateWiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            sample = MathF.Sin(sample * 8);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            sample = MathF.Sin(sample * 4);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateWormCave(Vector2 cavePosition,
        Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        //Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
        //Vector2 cavePosition = new Vector2(Main.maxTilesX / 2, (int)Main.worldSurface);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 1f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);

            float angleOffset = sample * MathHelper.Pi;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(angleOffset);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0f)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }
}
