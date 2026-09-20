using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.TileOverlaySystem;

public class TileOverlayUtility : ModSystem
{
    public static readonly Dictionary<Point, TileOverlayData> PlacedTileOverlays = new Dictionary<Point, TileOverlayData>();
    public static void RequestTileOverlayData()
    {
        ModPacket packet = Stellamod.Instance.GetPacket(capacity: 16);
        packet.Write((byte)MessageType.RequestTileOverlayData);
        packet.Send();
    }

    public static void ReceiveTileOverlaySync(BinaryReader reader, int whoAmI)
    {
        List<Point> points = new List<Point>();
        int oX = reader.ReadInt32();
        int oY = reader.ReadInt32();
        int oW = reader.ReadInt32();
        int oH = reader.ReadInt32();
        Rectangle rect = new Rectangle(oX, oY, oW, oH);
        for(int i = rect.Left; i < rect.Right; i++)
        {
            for(int j = rect.Top; j < rect.Bottom; j++)
            {
                Point p = new Point(i, j);
                if (PlacedTileOverlays.ContainsKey(p))
                    PlacedTileOverlays.Remove(p);
            }
        }

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            byte overlapType = reader.ReadByte();
            byte overlayFrame = reader.ReadByte();
            PlacedTileOverlays[new Point(x,y)] = new TileOverlayData { overlayType = overlapType, overlayFrame = overlayFrame };
            points.Add(new(x, y));
        }

        if (Main.netMode == NetmodeID.Server)
        {
            // Forward the changes to the other clients
            SendTileOverlayData(-1, -1, rect, points);
        }
    }
    public static void SendTileOverlayData(int requester, int ignore, Rectangle overlayArea, List<Point> points)
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.TileOverlaySync);
        packet.Write(overlayArea.X);
        packet.Write(overlayArea.Y);
        packet.Write(overlayArea.Width);
        packet.Write(overlayArea.Height);
        packet.Write(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            Point p = points[i];
            var data = PlacedTileOverlays[p];
            packet.Write((ushort)p.X);
            packet.Write((ushort)p.Y);
            packet.Write(data.overlayType);
            packet.Write(data.overlayFrame);
        }
        packet.Send(toClient: requester);
    }

    public static void SendTileOverlayData(int requester, int ignore, int x, int y, int width, int height)
    {
        Rectangle rect = new Rectangle(x, y, width, height);
        List<Point> points = new List<Point>();
        for (int i = rect.Left; i < rect.Right; i++)
        {
            for (int j = rect.Top; j < rect.Bottom; j++)
            {
                Point overlayPoint = new Point(i, j);
                if (PlacedTileOverlays.ContainsKey(overlayPoint))
                    points.Add(overlayPoint);
            }
        }
        SendTileOverlayData(requester, ignore, rect, points);
    }

    public static void HandleRequestPacket(BinaryReader reader, int whoAmI)
    {
        //This should loop over the world, appending points and when it gets to big it cuts it off and goes next
        List<Point> points = new List<Point>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Point tilePoint = new Point(x, y);
                if (PlacedTileOverlays.ContainsKey(tilePoint))
                {
                    points.Add(tilePoint);
                    if (points.Count >= 2000)
                    {
                        SendTileOverlayData(whoAmI, -1, Rectangle.Empty, points);
                        points.Clear();
                    }
                }
            }
        }
        if(points.Count > 0)
        {
            SendTileOverlayData(whoAmI, -1, Rectangle.Empty, points);
        }
    }

    public static string TypeToName(byte type)
    {
        var tile = TileOverlayRenderer.TileOverlays[type];
        return tile.Name;
    }
    public static byte NameToType(string name) => TileOverlayRenderer.TileOverlayTypeLookup[name];
    public static byte TileOverlayType<T>() where T : TileOverlayType
    {
        string name = typeof(T).Name;

        return TileOverlayRenderer.TileOverlayTypeLookup[name];
    }
    public static void PlaceTileOverlay(int x, int y, byte overlayType, TileOverlayPlacer placer)
    {
        var tileToPlace = TileOverlayRenderer.TileOverlays[overlayType];
        tileToPlace.PlaceTile(x, y, placer);

    }

    public static void KillTileOverlay(int x, int y)
    {
        Point tilePoint = new Point(x, y);
        if (!PlacedTileOverlays.ContainsKey(tilePoint))
            return;
        PlacedTileOverlays.Remove(tilePoint);
    }
}