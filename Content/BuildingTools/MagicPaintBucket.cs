using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Content.BuildingTools;

public class TheMagicHandPlayer : ModPlayer
{
    public bool hasMagicHand;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasMagicHand = false;
    }
}

public class TheMagicHand : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<TheMagicHandPlayer>().hasMagicHand = true;
    }
}

public class TileEyeDropper : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 16;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item9;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            Point point = Main.MouseWorld.ToTileCoordinates();
            point = TileUtilities.Clamp(point);
            Tile tile = Main.tile[point];
            Point playerPoint = player.position.ToTileCoordinates();
            if(player.altFunctionUse == 2)
            {
                if (tile.WallType > WallID.None)
                {
                    int itemType = 0;
                    //int itemType = WallLoader.Drop[tile.WallType];
                    WallLoader.Drop(playerPoint.X, playerPoint.Y, tile.WallType, ref itemType);
                    player.QuickSpawnItem(new EntitySource_TileBreak(point.X, point.Y), itemType, Item.CommonMaxStack);
            
                    //TODO: See if there's a way to get the paint item associatied with a paint ID
                    if(tile.WallColor > PaintID.None)
                    {
                     
                    }
                }
            } else
            {
                if (tile.HasTile)
                {
                    int dropItem = TileLoader.GetItemDropFromTypeAndStyle(tile.TileType);
                    player.QuickSpawnItem(new EntitySource_TileBreak(point.X, point.Y), dropItem, Item.CommonMaxStack);
                }
            }
            //Main.NewText("Collected Tile!");    
        }

        return true;
    }
}

public class MagicPaintBucket : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 16;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item9;
    }

    public override bool? UseItem(Player player)
    {
        if(player.whoAmI == Main.myPlayer)
        {
            Point point = Main.MouseWorld.ToTileCoordinates();
            MagicTileUtility.GetFloodCreateTiles(player, out var selected);
            if (Main.mouseRight)
            {
                if (selected.tileType > 0)
                    selected.tileType = 0;
                if (selected.wallType > 0)
                    selected.wallType = 0;
            }
            MagicTileUtility.FloodFill(point, selected);
 
        }

        return true;
    }
}
public class UndoBucket : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 16;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item9;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            MagicTileUtility.FloodUndo();
        }

        return true;
    }
}



[Autoload(Side = ModSide.Client)]
public class MagicPaintBucketPreview : ModSystem
{
    private HashSet<Point> _visited;
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        Player player = Main.LocalPlayer;
        if (player.HeldItem.type != ModContent.ItemType<MagicPaintBucket>())
            return;
        
        Point tilePoint = Main.MouseWorld.ToTileCoordinates();
        if(Main.GameUpdateCount % 12 == 0)
        {
            MagicTileUtility.GetFloodCreateTiles(player, out var selected);
            (int loops, HashSet<Point> visited) = MagicTileUtility.GetAffectedPoints(tilePoint, selected);
            _visited = visited;
        }

        
        if (_visited == null)
            return;


        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
        drawer.color = Color.Green * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        foreach(Point p in _visited)
        {
            drawer.worldPosition = p.ToWorldCoordinates();
            spriteBatch.Draw(drawer);
        }

        spriteBatch.End();
    }
}
public class MagicTilePlacer : ModSystem
{
    public override void Load()
    {
        base.Load();       
        On_Player.FigureOutWhatToPlace += AllowAnythingToPlace;
    }

    public override void Unload()
    {
        base.Unload();
        On_Player.FigureOutWhatToPlace -= AllowAnythingToPlace;
    }

    private void AllowAnythingToPlace(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom)
    {
        orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);
        if(self.GetModPlayer<TheMagicHandPlayer>().hasMagicHand)
            overrideCanPlace = true;
    }


}
public static class MagicTileUtility
{
    public struct MagicTileParams
    {
        public MagicTileParams()
        {
            tileType = -1;
            wallType = -1;
            tileReplace = -1;
            wallReplace = -1;
            paint = 0;
        }
        public int tileType;
        public int wallType;
        public int tileReplace;
        public int wallReplace;
        public byte paint;
    }

    public class TileSnapshot
    {
        public struct OldTile
        {
            public ushort oldWall;
            public ushort oldTile;
            public byte oldWallColor;
            public byte oldTileColor;
            public bool oldHasTile;
            public short oldTileFrameX;
            public short oldTileFrameY;
            public int oldTileFrameNumber;
            public int oldWallFrameX;
            public int oldWallFrameY;
            public int oldWallFrameNumber;
            public bool oldTileInvis;
            public bool oldWallinvis;
        }
        public TileSnapshot(Point topLeft, Point bottomRight)
        {
            int width = bottomRight.X - topLeft.X;
            int height = bottomRight.Y - topLeft.Y;
            width += 1;
            height += 1;
            OldTiles = new OldTile[width, height];
            for(int x = topLeft.X; x < bottomRight.X + 1; x++)
            {
                for(int y = topLeft.Y; y < bottomRight.Y + 1; y++)
                {
                    Tile tile = Main.tile[x, y];
                   
                    OldTiles[x - topLeft.X, y - topLeft.Y] = new OldTile
                    {
                        oldTile = tile.TileType,
                        oldWall = tile.WallType,
                        oldWallColor = tile.WallColor,
                        oldTileColor = tile.TileColor,
                        oldHasTile = tile.HasTile,
                        oldTileFrameX = tile.TileFrameX,
                        oldTileFrameY = tile.TileFrameY,
                        oldTileFrameNumber = tile.TileFrameNumber,
                        oldWallFrameX = tile.WallFrameX,
                        oldWallFrameY = tile.WallFrameY,
                        oldWallFrameNumber = tile.WallFrameNumber,
                        oldTileInvis = tile.IsTileInvisible,
                        oldWallinvis = tile.IsWallInvisible
                    };
                }
            }

            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        public Point TopLeft;
        public Point BottomRight;
        public OldTile[,] OldTiles;
    }

    public static Stack<TileSnapshot> TileSnapshots;
    public static int Limit => 50000;
    public static void GetFloodCreateTiles(Player player, out MagicTileParams tileParams)
    {
        Item air = new Item(0);
        air.TurnToAir();

        tileParams = new MagicTileParams();

        for (int i = 0; i < player.inventory.Length; i++)
        {
            Item item = player.inventory[i];
            if (item.createTile != -1 || item.createWall != -1)
            {
                tileParams.tileType = item.createTile;
                tileParams.wallType = item.createWall;
                break;
            }
        }

        for (int i = 0; i < player.inventory.Length; i++)
        {
            Item item = player.inventory[i];
            if (item.paint != 0)
            {
                tileParams.paint = item.paint;
                break;
            }
        }


        Point point = Main.MouseWorld.ToTileCoordinates();
        point = TileUtilities.Clamp(point);
        Tile tile = Main.tile[point];
        if (tile.HasTile)
        {
            tileParams.tileReplace = tile.TileType;
        }
        if (tile.WallType > 0)
        {
            tileParams.wallReplace = tile.WallType;
        }
    }

    public delegate bool BlockerFunction(in Point p, in MagicTileParams tileParams);
    public static (int, HashSet<Point>) GetAffectedPoints(Point tilePoint, MagicTileParams tileParams)
    {
        var visited = new HashSet<Point>();
        var path = new Stack<Point>();
        path.Push(tilePoint);
        int loops = 0;

        //Decide blocker function
        BlockerFunction solidFunction;
        if(tileParams.wallType != -1)
        {
            if(tileParams.wallReplace != -1)
            {
                solidFunction = IsWallBlockedWallReplace;
            }
            else
            {
                solidFunction = IsWallBlocked;
            }
        }
        else
        {
            if(tileParams.tileReplace != -1)
            {
                solidFunction = IsTileBlockedTileReplace;
            }
            else
            {
                solidFunction = IsTileBlocked;
            }
        }


        bool NeedsVisiting(Point tilePoint)
        {
            return !solidFunction(tilePoint, tileParams) && !visited.Contains(tilePoint);
        }

        void Visit(Point tilePoint)
        {
            path.Push(tilePoint);
            visited.Add(tilePoint);
        }
        while (path.Count > 0)
        {
            Point next = path.Pop();
            Point left = next + new Point(1, 0);
            Point right = next + new Point(-1, 0);
            Point up = next + new Point(0, -1);
            Point down = next + new Point(0, 1);

          
            if (visited.Count > Limit)
                break;


            if (next.X < 1 || next.X > Main.maxTilesX - 1|| next.Y < 1 || next.Y > Main.maxTilesY - 1)
            {
                continue;
            }

            Tile tile = Main.tile[next];
            if (solidFunction(next, tileParams))
            {
                continue;
            }

            if (NeedsVisiting(left))
            {
                Visit(left);
            }
            if (NeedsVisiting(right))
            {
                Visit(right);
            }

            if (NeedsVisiting(up))
            {
                Visit(up);
            }
            if (NeedsVisiting(down))
            {
                Visit(down);
            }
        }
        return (loops, visited);
    }

    public static bool IsTileBlockedTileReplace(in Point tilePoint, in MagicTileParams tileParams)
    {
        Tile tile = Main.tile[tilePoint];
        if (tile.HasTile && tile.TileType == tileParams.tileReplace)
            return false;
        if (!tile.HasTile)
            return true;

        return IsTileBlocked(tilePoint, tileParams);
    }
    public static bool IsTileBlocked(in Point tilePoint, in MagicTileParams tileParams)
    {
        return WorldGen.SolidTile(tilePoint);
    }
    public static bool IsWallBlockedWallReplace(in Point tilePoint, in MagicTileParams tileParams)
    {
        Tile tile = Main.tile[tilePoint];
        if (WorldGen.SolidTile(tilePoint))
            return true;
        if (tile.WallType == tileParams.wallReplace)
            return false;
        if (tile.WallType == 0)
            return true;
        return tile.WallType != 0;
        //return IsWallBlocked(tilePoint, tileParams);
    }
    public static bool IsWallBlocked(in Point tilePoint, in MagicTileParams tileParams)
    {
        Tile tile = Main.tile[tilePoint];
        return tile.WallType > 0 || WorldGen.SolidTile(tilePoint);
    }

    public static void FloodUndo()
    {
        TileSnapshots ??= new();
        if (TileSnapshots.Count <= 0)
            return;
        TileSnapshot snapShot = TileSnapshots.Pop();
        for(int x = 0; x < snapShot.OldTiles.GetLength(0); x++)
        {
            for(int y = 0; y < snapShot.OldTiles.GetLength(1); y++)
            {
                ref var oldTile = ref snapShot.OldTiles[x, y];
                Point tilePoint = snapShot.TopLeft + new Point(x, y);
                Tile tile = Main.tile[tilePoint];
                tile.ClearTile();
                tile.ClearBlockPaintAndCoating();
                tile.TileType = oldTile.oldTile;
                tile.HasTile = oldTile.oldHasTile;
                tile.TileColor = oldTile.oldTileColor;
                tile.TileFrameX = oldTile.oldTileFrameX;
                tile.TileFrameY = oldTile.oldTileFrameY;
                tile.TileFrameNumber = oldTile.oldTileFrameNumber;
               // WorldGen.SquareTileFrame(tilePoint.X, tilePoint.Y);

                tile.WallType = oldTile.oldWall;
                tile.WallColor = oldTile.oldWallColor;
                tile.WallFrameX = oldTile.oldWallFrameX;
                tile.WallFrameY = oldTile.oldWallFrameY;
                tile.WallFrameNumber = oldTile.oldWallFrameNumber;
                tile.IsTileInvisible = oldTile.oldTileInvis;
                tile.IsWallInvisible = oldTile.oldWallinvis;
              //  WorldGen.SquareWallFrame(tilePoint.X, tilePoint.Y);
            }
        }
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        int width = snapShot.BottomRight.X - snapShot.TopLeft.X;
        int height = snapShot.BottomRight.Y - snapShot.TopLeft.Y;
        width += 1;
        height += 1;

        SendTileData(snapShot.TopLeft.X, snapShot.TopLeft.Y, width, height);

    }
    public static void FloodFill(Point tilePoint, in MagicTileParams tileParams)
    {
        (int loops, HashSet<Point> visited) = GetAffectedPoints(tilePoint, tileParams);
        if(loops > Limit)
        {
            Vector2 pos = tilePoint.ToWorldCoordinates();
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.Red, "....", true);
            return;
        }

        TileSnapshots ??= new();
        Point topLeft = tilePoint, bottomRight = tilePoint;
        foreach (Point next in visited)
        {
            if (next.X < topLeft.X)
                topLeft.X = next.X;
            if (next.Y < topLeft.Y)
                topLeft.Y = next.Y;
            if (next.X > bottomRight.X)
                bottomRight.X = next.X;
            if (next.Y > bottomRight.Y)
                bottomRight.Y = next.Y;
        }

        TileSnapshots.Push(new TileSnapshot(topLeft, bottomRight));
        if (tileParams.tileType != -1)
        {
          
            foreach (Point next in visited)
            {
                Tile tile = Main.tile[next];
                tile.ClearTile();
                tile.ClearBlockPaintAndCoating();
                tile.TileType = (ushort)tileParams.tileType;
                tile.HasTile = true;
                tile.TileColor = tileParams.paint;
                WorldGen.SquareTileFrame(next.X, next.Y);
            }
        }

        if (tileParams.wallType != -1)
        {
            foreach (Point next in visited)
            {
                Tile tile = Main.tile[next];
                tile.WallType = (ushort)tileParams.wallType;
                tile.WallColor = tileParams.paint;
                WorldGen.SquareWallFrame(next.X, next.Y);
            }
        }

        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        int width = bottomRight.X - topLeft.X;
        int height = bottomRight.Y - topLeft.Y;
        width += 1;
        height += 1;

        SendTileData(topLeft.X, topLeft.Y, width, height);
    }

    public static void SendTileData(int i, int j, int width, int height)
    {
        int maxSquareSize = 25;

        //ints round downward, we need to round up to make sure we actually get all the tiles
        int numXLoops = (int)MathF.Ceiling((float)width / (float)maxSquareSize);
        int numYLoops = (int)MathF.Ceiling((float)height / (float)maxSquareSize);

        //Here we are dividing up the tile squares because if we pack too much data in the same square 
        for(int x = 0; x < numXLoops; x++)
        {
            for(int y = 0; y < numYLoops; y++)
            {
                Point topLeft = new Point();
                topLeft.X = i + x * maxSquareSize;
                topLeft.Y = j + y * maxSquareSize;


                NetMessage.SendTileSquare(-1, topLeft.X, topLeft.Y, maxSquareSize, maxSquareSize);
            }
        }
    }
}
