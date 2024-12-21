using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.WorldG.StructureManager
{
    internal class SnapshotSystem : ModSystem
    {
        public Stack<Snapshot> Snapshots = new Stack<Snapshot>();
        public void Save()
        {
            Snapshot snapshot = new Snapshot();
            Snapshots.Push(snapshot);
        }

        public void Undo()
        {
            if(Snapshots.Count > 0)
            {
                Snapshot snapshot = Snapshots.Pop();
                snapshot.RestoreSnapshot();
            }
        }
    }

    internal class Snapshot
    {
        public readonly record struct TileData(
            TileTypeData TileTypeData, 
            WallTypeData WallTypeData, 
            TileWallWireStateData TileWallWireStateData,
            LiquidData LiquidData,
            TileWallBrightnessInvisibilityData TileWallBrightnessInvisibilityData);

        public TileData[,] snap;
        public Snapshot()
        {
            snap = new TileData[Main.maxTilesX, Main.maxTilesY];
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    snap[i, j] = new(
                        tile.Get<TileTypeData>(),
                        tile.Get<WallTypeData>(), 
                        tile.Get<TileWallWireStateData>(),
                        tile.Get<LiquidData>(),
                        tile.Get<TileWallBrightnessInvisibilityData>());
                }
            }
        }

        public void RestoreSnapshot()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    tile.TileType = snap[i, j].TileTypeData.Type;
                    tile.WallType = snap[i, j].WallTypeData.Type;
                    tile.Get<TileWallWireStateData>() = snap[i, j].TileWallWireStateData;
                    tile.Get<LiquidData>() = snap[i, j].LiquidData;
                    tile.Get<TileWallBrightnessInvisibilityData>() = snap[i, j].TileWallBrightnessInvisibilityData;
                }
            }
        }
    }
}
