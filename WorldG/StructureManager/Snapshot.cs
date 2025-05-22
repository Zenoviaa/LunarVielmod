using Microsoft.Xna.Framework;
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
        public void Save(Point bottomLeft, Point topRight)
        {
            Snapshot snapshot = new Snapshot(bottomLeft, topRight);
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
        public Point bottomLeft;
        public Point topRight;
        public Snapshot(Point bottomLeft, Point topRight)
        {
            this.bottomLeft = bottomLeft;
            this.topRight = topRight;
            int width = (int)(topRight.X - bottomLeft.X);
            int height = (int)(bottomLeft.Y - topRight.Y);
            snap = new TileData[width+1, height+1];
           // Console.WriteLine(bottomLeft);
        //    Console.WriteLine(topRight);
            for (int x = (int)(bottomLeft.X); x <= topRight.X; x++)
            {
                for (int y = (int)(topRight.Y); y <= bottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                   
                    snap[x - bottomLeft.X, y - topRight.Y] = new(
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
            for (int x = (int)(bottomLeft.X); x <= topRight.X; x++)
            {
                for (int y = (int)(topRight.Y); y <= bottomLeft.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    var tileData = snap[x - bottomLeft.X, y - topRight.Y];
                    tile.TileType = tileData.TileTypeData.Type;
                    tile.WallType = tileData.WallTypeData.Type;
                    tile.Get<TileWallWireStateData>() = tileData.TileWallWireStateData;
                    tile.Get<LiquidData>() = tileData.LiquidData;
                    tile.Get<TileWallBrightnessInvisibilityData>() = tileData.TileWallBrightnessInvisibilityData;
                }
            }
        }
    }
}
