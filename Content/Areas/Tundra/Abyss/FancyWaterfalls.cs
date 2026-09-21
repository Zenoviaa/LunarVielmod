using Stellamod.Core;
using Stellamod.Core.TileOverlaySystem;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public readonly record struct WaterfallPoint(Point TilePoint, int FallHeight)
{

}

public class WaterfallMaker : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.White;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = false;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            if(player.altFunctionUse == 2)
            {
                FancyWaterfalls.KillWaterfall(tilePoint);
            }
            else
            {
                FancyWaterfalls.PlaceFallingWaterfall(tilePoint);
            }
  
            if (Main.netMode != NetmodeID.SinglePlayer)
                FancyWaterfalls.SendWaterfallSync(-1, -1, tilePoint.X, tilePoint.Y, 1, 1);
        }

        return true;
    }
}
public class FancyWaterfallsPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        if (Main.netMode == NetmodeID.Server)
            return;

        FancyWaterfalls.RequestWaterfallData();
    }
}

public class FancyWaterfalls : ModSystem
{
    public static readonly List<WaterfallPoint> FancyWaterfallPoints = new();
    public override void ClearWorld()
    {
        base.ClearWorld();
        FancyWaterfallPoints.Clear();
    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        
        int type = ModContent.ItemType<WaterfallMaker>();
        if (Main.LocalPlayer.HeldItem.type != type)
            return;
        Main.spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        foreach (var p in FancyWaterfallPoints)
        {
            Vector2 worldPos = p.TilePoint.ToWorldCoordinates();
            Vector2 drawPos = worldPos - Main.screenPosition;
            if (drawPos.X < 0 || drawPos.Y < 0 || drawPos.X > Main.screenWidth || drawPos.Y > Main.screenHeight)
                continue;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Main.screenPosition + drawPos);
            drawer.color = Color.Red * ExtraMath.Osc(0.5f, 0.8f, speed: 3);
            Main.spriteBatch.Draw(drawer);
        }
        Main.spriteBatch.End();
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        FancyWaterfallPoints.Clear();
        Point[] points = tag.Get<Point[]>("waterfallPoints");
        int[] heights = tag.Get<int[]>("waterfallHeights");
        for(int i = 0; i < heights.Length; i++)
        {
            FancyWaterfallPoints.Add(new WaterfallPoint(points[i], heights[i]));
        }
    }

    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        Point[] points = new Point[FancyWaterfallPoints.Count];
        int[] heights = new int[FancyWaterfallPoints.Count];
        for(int i = 0; i < FancyWaterfallPoints.Count; i++)
        {
            points[i] = FancyWaterfallPoints[i].TilePoint;
            heights[i] = FancyWaterfallPoints[i].FallHeight;
        }

        tag["waterfallPoints"] = points;
        tag["waterfallHeights"] = heights;
    }
    public static void RequestWaterfallData()
    {
        ModPacket packet = Stellamod.Instance.GetPacket(capacity: 16);
        packet.Write((byte)MessageType.RequestWaterfallData);
        packet.Send();
    }
    public static void SendWaterfallSync(int requester, int ignore, int x, int y, int width, int height)
    {
        Rectangle rect = new Rectangle(x, y, width, height);
        List<WaterfallPoint> points = new List<WaterfallPoint>();
        foreach(var waterfall in FancyWaterfallPoints)
        {
            if (rect.Contains(waterfall.TilePoint))
                points.Add(waterfall);
        }

        SendWaterfallSync(requester, ignore, rect, points);
    }


    public static void SendWaterfallSync(int requester, int ignore, Rectangle areaToSend, List<WaterfallPoint> points)
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.WaterfallSync);
        packet.Write(areaToSend.X);
        packet.Write(areaToSend.Y);
        packet.Write(areaToSend.Width);
        packet.Write(areaToSend.Height);
        packet.Write(points.Count);
        foreach (var p in points)
        {
            packet.Write((ushort)p.TilePoint.X);
            packet.Write((ushort)p.TilePoint.Y);
            packet.Write((byte)p.FallHeight);
        }
        packet.Send(-1);
    }

    public static void HandleRequestPacket(BinaryReader reader, int whoAmI)
    {
        //This should loop over the world, appending points and when it gets to big it cuts it off and goes next
        List<WaterfallPoint> points = new List<WaterfallPoint>();
        foreach (var wf in FancyWaterfallPoints)
        {
            points.Add(wf);
            if (points.Count >= 1500)
            {
                SendWaterfallSync(whoAmI, -1, Rectangle.Empty, points);
                points.Clear();
            }
        }

        if (points.Count > 0)
        {
            SendWaterfallSync(whoAmI, -1, Rectangle.Empty, points);
        }
    }

    public static void ReceiveWaterfallSync(BinaryReader reader, int whoAmI)
    {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();

        Rectangle rect = new Rectangle(x, y, width, height);
        FancyWaterfallPoints.RemoveAll(x => rect.Contains(x.TilePoint));
        int count = reader.ReadInt32();
        List<WaterfallPoint> points = new List<WaterfallPoint>();
        for(int i = 0; i < count; i++)
        {
            ushort xPos = reader.ReadUInt16();
            ushort yPos = reader.ReadUInt16();
            byte fallHeight = reader.ReadByte();
            var waterfall = new WaterfallPoint(new Point(xPos, yPos), fallHeight);
            FancyWaterfallPoints.Add(waterfall);
            points.Add(waterfall);
        }

        if (Main.netMode == NetmodeID.Server)
        {
            // Forward the changes to the other clients
            SendWaterfallSync(-1, -1, rect, points);
        }
    }

    public static void PlaceFallingWaterfall(in Point tilePoint)
    {
        Point current = tilePoint;
        int bottomPoint = TileUtilities.FallToSolidOrWaterTile(tilePoint.X, tilePoint.Y);
        int fallHeight = bottomPoint - tilePoint.Y;
        PlaceWaterfall(current, fallHeight);
    }

    public static void KillWaterfall( Point tilePoint)
    {
        FancyWaterfallPoints.RemoveAll(x => x.TilePoint == tilePoint);
    }

    public static void PlaceWaterfall(in Point tilePoint, in int fallHeight) => FancyWaterfallPoints.Add(new(tilePoint, fallHeight));
}
