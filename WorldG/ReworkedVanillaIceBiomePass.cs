using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG
{
    public class ReworkedVanillaIceBiomePass : GenPass
    {
        public ReworkedVanillaIceBiomePass()
            : base("Generate Ice Biome", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            var genRand = WorldGen.genRand;
            progress.Message = Lang.gen[56].Value;
            GenVars.snowTop = (int)Main.worldSurface;
            int num975 = GenVars.lavaLine - genRand.Next(160, 200);
            int bottom = GenVars.lavaLine;

            int left = GenVars.snowOriginLeft;
            int right = GenVars.snowOriginRight;
            int num979 = 10;
            for (int tileY = 0; tileY <= bottom - 140; tileY++)
            {
                progress.Set((double)tileY / (double)(bottom - 140));
              /*  num977 += genRand.Next(-4, 4);
                num978 += genRand.Next(-3, 5);
                if (num980 > 0)
                {
                    num977 = (num977 + GenVars.snowMinX[num980 - 1]) / 2;
                    num978 = (num978 + GenVars.snowMaxX[num980 - 1]) / 2;
                }
                */

                GenVars.snowMinX[tileY] = left;
                GenVars.snowMaxX[tileY] = right;
                for (int tileX = left; tileX < right; tileX++)
                {
                    if (tileY < num975)
                    {
                        if (Main.tile[tileX, tileY].WallType == WallID.DirtUnsafe)
                            Main.tile[tileX, tileY].WallType = WallID.SnowWallUnsafe;

                        switch (Main.tile[tileX, tileY].TileType)
                        {
                            case TileID.Dirt:
                            case TileID.Grass:
                            case TileID.CorruptGrass:
                            case TileID.ClayBlock:
                            case TileID.Sand:
                                Main.tile[tileX, tileY].TileType = TileID.SnowBlock;
                                break;
                            case 1:
                                Main.tile[tileX, tileY].TileType = TileID.IceBlock;
                                break;
                        }
                    }
                    else
                    {
                        num979 += genRand.Next(-3, 4);
                        if (genRand.Next(3) == 0)
                        {
                            num979 += genRand.Next(-4, 5);
                            if (genRand.Next(3) == 0)
                                num979 += genRand.Next(-6, 7);
                        }

                        if (num979 < 0)
                            num979 = genRand.Next(3);
                        else if (num979 > 50)
                            num979 = 50 - genRand.Next(3);

                        for (int num982 = tileY; num982 < tileY + num979; num982++)
                        {
                            if (Main.tile[tileX, num982].WallType == WallID.DirtUnsafe)
                                Main.tile[tileX, num982].WallType = WallID.SnowWallUnsafe;

                            switch (Main.tile[tileX, num982].TileType)
                            {
                                case TileID.Dirt:
                                case TileID.Grass:
                                case TileID.CorruptGrass:
                                case TileID.ClayBlock:
                                case TileID.Sand:
                                    Main.tile[tileX, num982].TileType = TileID.SnowBlock;
                                    break;
                                case 1:
                                    Main.tile[tileX, num982].TileType = TileID.IceBlock;
                                    break;
                            }
                        }
                    }
                }

                if (GenVars.snowBottom < tileY)
                    GenVars.snowBottom = tileY;
            }
        }
    }
}
