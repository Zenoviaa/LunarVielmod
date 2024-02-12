using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Stellamod.Tiles.Catacombs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod
{
    internal class TeleportSystem : ModSystem
    {
        private static bool _findTeleportTiles;
        public static Vector2[] DungeonAltarWorld;
        public static Vector2 StoneGolemAltarWorld;
        public override void PostUpdateWorld()
        {
            if (!_findTeleportTiles)
            {
                FindTiles(ModContent.TileType<CatacombsSummon>(), out DungeonAltarWorld, out bool foundTile);
                FindTiles(ModContent.TileType<FlowerSummon>(), out Vector2[] stoneGolemAltar, out foundTile);
                if (foundTile)
                {
                    StoneGolemAltarWorld = stoneGolemAltar[0];
                }
                _findTeleportTiles = true;
            }
        }

        private void FindTiles(int tileType, out Vector2[] world, out bool foundTile)
        {
            foundTile = false;
            List<Vector2> worldList = new List<Vector2>();
            for (int x = 0; x < Main.tile.Width; x++)
            {
                for (int y = 0; y < Main.tile.Height; y++)
                {
                    if (Main.tile[x, y].TileType == tileType)
                    {
                        Vector2 worldPoint = new Vector2(x, y).ToWorldCoordinates();
                        worldList.Add(worldPoint);
                        foundTile = true;
                    }
                }
            }

            world = worldList.ToArray();
        }
    }
}
