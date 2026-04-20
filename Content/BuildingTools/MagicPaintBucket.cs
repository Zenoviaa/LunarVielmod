using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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
        Point point = Main.MouseWorld.ToTileCoordinates();
        
        GetFloodCreateTiles(player, out var selected);
        MagicTileUtility.FloodFill(point, selected.createTile, selected.createWall);
        return true;
    }

    private void GetFloodCreateTiles(Player player, out Item selected)
    {
        Item air = new Item(0);
        air.TurnToAir();

        selected = air;
        for (int i = 0; i < player.inventory.Length; i++)
        {
            Item item = player.inventory[i];
            if (item.createTile != -1 || item.createWall != -1)
            {
                selected = item;
                break;
            }
        }
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        GetFloodCreateTiles(Main.LocalPlayer, out Item selected);
        if (selected.IsAir)
            return;

        Vector2 drawPos = position + new Vector2(8);
        drawPos.Y += ExtraMath.Osc(0f, 2f, speed: 1);
       // drawPos -= Main.screenPosition;
        ItemSlot.DrawItemIcon(selected, 0, spriteBatch, drawPos, scale * 0.66f, 32, itemColor);
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
    public static void FloodFill_InnerRecursive(HashSet<Point> visited, Point tilePoint, int tileType = -1, int wallType = -1)
    {
       
        visited.Add(tilePoint);
        if (tilePoint.X < 0 || tilePoint.X > Main.maxTilesX || tilePoint.Y < 0 || tilePoint.Y > Main.maxTilesY)
            return;
        Tile tile = Main.tile[tilePoint];
        if (WorldGen.SolidTile(tilePoint))
            return;
        if (tileType != -1)
        {
            WorldGen.PlaceTile(tilePoint.X, tilePoint.Y, tileType);
        }
        if (wallType != -1)
        {
            tile.WallType = (ushort)wallType;
        }

        Point left = tilePoint + new Point(1, 0);
        Point right = tilePoint + new Point(-1, 0);
        Point up = tilePoint + new Point(0, -1);
        Point down = tilePoint + new Point(0, 1);

        if(!visited.Contains(left))
            FloodFill_InnerRecursive(visited, left, tileType, wallType);

        if (!visited.Contains(right))
            FloodFill_InnerRecursive(visited, right, tileType, wallType);

        if (!visited.Contains(up))
            FloodFill_InnerRecursive(visited, up, tileType, wallType);

        if (!visited.Contains(down))
            FloodFill_InnerRecursive(visited, down, tileType, wallType);
    }

    public static int CountLoops(Point tilePoint, int tileType = -1, int wallType = -1)
    {
        var visited = new HashSet<Point>();
        var path = new Stack<Point>();
        path.Push(tilePoint);
        int loops = 0;
        while (path.Count > 0)
        {
            Point next = path.Pop();
            Point left = next + new Point(1, 0);
            Point right = next + new Point(-1, 0);
            Point up = next + new Point(0, -1);
            Point down = next + new Point(0, 1);

            loops++;
            if (loops > 100000)
                break;
            if (next.X < 0 || next.X > Main.maxTilesX || next.Y < 0 || next.Y > Main.maxTilesY)
            {
                continue;
            }

            Tile tile = Main.tile[next];
            if (WorldGen.SolidTile(next))
            {
                continue;
            }

            

            if (!visited.Contains(left))
            {
                path.Push(left);
                visited.Add(left);
            }
            if (!visited.Contains(right))
            {
                path.Push(right);
                visited.Add(right);
            }
            if (!visited.Contains(up))
            {
                path.Push(up);
                visited.Add(up);
            }
            if (!visited.Contains(down))
            {
                path.Push(down);
                visited.Add(down);
            }
        }
        return loops;
        //     FloodFill_Inner(visited, tilePoint, tileType, wallType);
    }
    public static void FloodFill(Point tilePoint, int tileType = -1, int wallType = -1)
    {
        int loops = CountLoops(tilePoint, tileType, wallType);
        if(loops > 100000)
        {
            Vector2 pos = tilePoint.ToWorldCoordinates();
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.Red, "....", true);
            return;
        }    
        var visited = new HashSet<Point>();
        var path = new Stack<Point>();
        path.Push(tilePoint);

        while(path.Count > 0)
        {
            Point next = path.Pop();
            Point left = next + new Point(1, 0);
            Point right = next + new Point(-1, 0);
            Point up = next + new Point(0, -1);
            Point down = next + new Point(0, 1);
     
        
            if (next.X < 0 || next.X > Main.maxTilesX || next.Y < 0 || next.Y > Main.maxTilesY)
            {
                continue;
            }

            Tile tile = Main.tile[next];
            if (WorldGen.SolidTile(next))
            {
                continue;
            }

            if (tileType != -1)
            {
                WorldGen.PlaceTile(next.X, next.Y, tileType, true);
            }
            if (wallType != -1)
            {
                WorldGen.PlaceWall(next.X, next.Y, wallType, true);
            }

            if (!visited.Contains(left))
            {
                path.Push(left);
                visited.Add(left);
            }
            if (!visited.Contains(right))
            {
                path.Push(right);
                visited.Add(right);
            }
            if (!visited.Contains(up))
            {
                path.Push(up);
                visited.Add(up);
            }
            if (!visited.Contains(down))
            {
                path.Push(down);
                visited.Add(down);
            }
        }
   //     FloodFill_Inner(visited, tilePoint, tileType, wallType);
    }
}
