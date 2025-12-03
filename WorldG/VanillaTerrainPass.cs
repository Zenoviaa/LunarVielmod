using System;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG
{
    public class VanillaTerrainPass : GenPass
    {
        private enum TerrainFeatureType
        {
            Plateau,
            Hill,
            Dale,
            Mountain,
            Valley
        }

        private class SurfaceHistory
        {
            private readonly double[] _heights;
            private int _index;

            public double this[int index]
            {
                get
                {
                    return _heights[(index + _index) % _heights.Length];
                }
                set
                {
                    _heights[(index + _index) % _heights.Length] = value;
                }
            }

            public int Length => _heights.Length;

            public SurfaceHistory(int size)
            {
                _heights = new double[size];
            }

            public void Record(double height)
            {
                _heights[_index] = height;
                _index = (_index + 1) % _heights.Length;
            }
        }

        public VanillaTerrainPass()
            : base("Terrain", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int num = 10;
            progress.Message = "Generating Terrain";
            TerrainFeatureType terrainFeatureType = TerrainFeatureType.Plateau;
            int num2 = 0;
            double worldSurface = Main.maxTilesY * 0.65;
            worldSurface *= GenBase._random.Next(90, 110) * 0.005;
            double rockLayer = worldSurface + Main.maxTilesY * 0.13;
            rockLayer *= GenBase._random.Next(90, 110) * 0.01;

            double worldSurfaceLow = worldSurface;
            double worldSurfaceHigh = worldSurface;
            double rockLayerLow = rockLayer;
            double rockLayerHigh = rockLayer;
            double num9 = Main.maxTilesY * 0.23;

            SurfaceHistory surfaceHistory = new SurfaceHistory(500);
            num2 = GenVars.leftBeachEnd + num;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set(i / (double)Main.maxTilesX);
                worldSurfaceLow = Math.Min(worldSurface, worldSurfaceLow);
                worldSurfaceHigh = Math.Max(worldSurface, worldSurfaceHigh);
                rockLayerLow = Math.Min(rockLayer, rockLayerLow);
                rockLayerHigh = Math.Max(rockLayer, rockLayerHigh);
                if (num2 <= 0)
                {
                    terrainFeatureType = TerrainFeatureType.Plateau;
                    num2 = GenBase._random.Next(5, 40);
                    if (terrainFeatureType == TerrainFeatureType.Plateau)
                        num2 *= (int)(GenBase._random.Next(5, 30) * 0.2);
                }

                num2--;
                if (i > Main.maxTilesX * 0.45 && i < Main.maxTilesX * 0.55 && (terrainFeatureType == TerrainFeatureType.Mountain || terrainFeatureType == TerrainFeatureType.Valley))
                    terrainFeatureType = TerrainFeatureType.Plateau;

                if (i > Main.maxTilesX * 0.48 && i < Main.maxTilesX * 0.52)
                    terrainFeatureType = TerrainFeatureType.Plateau;

                worldSurface += GenerateWorldSurfaceOffset(terrainFeatureType);
                while (GenBase._random.Next(0, 3) == 0)
                {
                    rockLayer += GenBase._random.Next(-2, 3);
                }

                if (rockLayer < worldSurface + Main.maxTilesY * 0.06)
                    rockLayer += 1.0;

                if (rockLayer > worldSurface + Main.maxTilesY * 0.35)
                    rockLayer -= 1.0;


                surfaceHistory.Record(worldSurface);
                FillColumn(i, worldSurface, rockLayer);
            }

            Main.worldSurface = (int)(worldSurfaceHigh + 25.0);
            Main.rockLayer = rockLayerHigh;
            double num12 = (int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6;
            Main.rockLayer = (int)(Main.worldSurface + num12);
            int num13 = (int)(Main.rockLayer + Main.maxTilesY) / 2 + GenBase._random.Next(-100, 20);
            int lavaLine = num13 + GenBase._random.Next(50, 80);
            if (WorldGen.remixWorldGen)
                lavaLine = (int)(Main.worldSurface * 4.0 + rockLayer) / 5;

            int num14 = 20;
            if (rockLayerLow < worldSurfaceHigh + num14)
            {
                double num15 = (rockLayerLow + worldSurfaceHigh) / 2.0;
                double num16 = Math.Abs(rockLayerLow - worldSurfaceHigh);
                if (num16 < num14)
                    num16 = num14;

                rockLayerLow = num15 + num16 / 2.0;
                worldSurfaceHigh = num15 - num16 / 2.0;
            }

            GenVars.rockLayer = rockLayer;
            GenVars.rockLayerHigh = rockLayerHigh;
            GenVars.rockLayerLow = rockLayerLow;
            GenVars.worldSurface = worldSurface;
            GenVars.worldSurfaceHigh = worldSurfaceHigh;
            GenVars.worldSurfaceLow = worldSurfaceLow;
            GenVars.waterLine = num13;
            GenVars.lavaLine = lavaLine;
        }

        private static void FillColumn(int x, double worldSurface, double rockLayer)
        {
            for (int i = 0; i < worldSurface; i++)
            {
                Tile tile = Main.tile[x, i];
                tile.HasTile = false;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }

            for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
            {
                if (j < rockLayer)
                {
                    Tile tile = Main.tile[x, j];
                    tile.HasTile = true;
                    tile.TileType = 0;
                    tile.TileFrameX = -1;
                    tile.TileFrameY = -1;
                }
                else
                {
                    Tile tile = Main.tile[x, j];
                    tile.HasTile = true;
                    tile.TileType = 1;
                    tile.TileFrameX = -1;
                    tile.TileFrameY = -1;
                }
            }
        }

        private static void RetargetColumn(int x, double worldSurface)
        {
            for (int i = 0; i < worldSurface; i++)
            {
                Tile tile = Main.tile[x, i];
                tile.HasTile = false;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }

            for (int j = (int)worldSurface; j < Main.maxTilesY; j++)
            {
                if (Main.tile[x, j].TileType != 1 || !Main.tile[x, j].HasTile)
                {
                    Tile tile = Main.tile[x, j];
                    tile.HasTile = true;
                    tile.TileType = 0;
                    tile.TileFrameX = -1;
                    tile.TileFrameY = -1;
                }
            }
        }

        private static double GenerateWorldSurfaceOffset(TerrainFeatureType featureType)
        {
            double num = 0.0;
            if ((WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen || WorldGen.remixWorldGen) && WorldGen.genRand.Next(2) == 0)
            {
                switch (featureType)
                {
                    case TerrainFeatureType.Plateau:
                        while (GenBase._random.Next(0, 6) == 0)
                        {
                            num += GenBase._random.Next(-1, 2);
                        }
                        break;
                    case TerrainFeatureType.Hill:
                        while (GenBase._random.Next(0, 3) == 0)
                        {
                            num -= 1.0;
                        }
                        while (GenBase._random.Next(0, 10) == 0)
                        {
                            num += 1.0;
                        }
                        break;
                    case TerrainFeatureType.Dale:
                        while (GenBase._random.Next(0, 3) == 0)
                        {
                            num += 1.0;
                        }
                        while (GenBase._random.Next(0, 10) == 0)
                        {
                            num -= 1.0;
                        }
                        break;
                    case TerrainFeatureType.Mountain:
                        while (GenBase._random.Next(0, 3) != 0)
                        {
                            num -= 1.0;
                        }
                        while (GenBase._random.Next(0, 6) == 0)
                        {
                            num += 1.0;
                        }
                        break;
                    case TerrainFeatureType.Valley:
                        while (GenBase._random.Next(0, 3) != 0)
                        {
                            num += 1.0;
                        }
                        while (GenBase._random.Next(0, 5) == 0)
                        {
                            num -= 1.0;
                        }
                        break;
                }
            }
            else
            {
                switch (featureType)
                {
                    case TerrainFeatureType.Plateau:
                        while (GenBase._random.Next(0, 7) == 0)
                        {
                            num += GenBase._random.Next(-1, 2);
                        }
                        break;
                    case TerrainFeatureType.Hill:
                        while (GenBase._random.Next(0, 4) == 0)
                        {
                            num -= 1.0;
                        }
                        while (GenBase._random.Next(0, 10) == 0)
                        {
                            num += 1.0;
                        }
                        break;
                    case TerrainFeatureType.Dale:
                        while (GenBase._random.Next(0, 4) == 0)
                        {
                            num += 1.0;
                        }
                        while (GenBase._random.Next(0, 10) == 0)
                        {
                            num -= 1.0;
                        }
                        break;
                    case TerrainFeatureType.Mountain:
                        while (GenBase._random.Next(0, 2) == 0)
                        {
                            num -= 1.0;
                        }
                        while (GenBase._random.Next(0, 6) == 0)
                        {
                            num += 1.0;
                        }
                        break;
                    case TerrainFeatureType.Valley:
                        while (GenBase._random.Next(0, 2) == 0)
                        {
                            num += 1.0;
                        }
                        while (GenBase._random.Next(0, 5) == 0)
                        {
                            num -= 1.0;
                        }
                        break;
                }
            }

            return num;
        }

        private static void RetargetSurfaceHistory(SurfaceHistory history, int targetX, double targetHeight)
        {
            for (int i = 0; i < history.Length / 2; i++)
            {
                if (history[history.Length - 1] <= targetHeight)
                    break;

                for (int j = 0; j < history.Length - i * 2; j++)
                {
                    double num = history[history.Length - j - 1];
                    num -= 1.0;
                    history[history.Length - j - 1] = num;
                    if (num <= targetHeight)
                        break;
                }
            }

            for (int k = 0; k < history.Length; k++)
            {
                double worldSurface = history[history.Length - k - 1];
                RetargetColumn(targetX - k, worldSurface);
            }
        }
    }
}