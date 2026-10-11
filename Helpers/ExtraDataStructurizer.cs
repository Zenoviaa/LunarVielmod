using Stellamod.Content.Areas.Tundra.Abyss;
using Stellamod.Core.TileOverlaySystem;
using Stubble.Core.Classes;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers;

public static class ExtraDataStructurizer
{
    const string MESSAGE_SAVED = "Extra Data Structure Saved";
    private static Mod Mod => Stellamod.Instance;
    public static TagCompound Serialize(Point bottomLeft, Point topRight)
    {
        TagCompound root = new TagCompound();
        List<Point> points = new List<Point>();
        for (int x = bottomLeft.X; x <= topRight.X; x++)
        {
            for (int y = bottomLeft.Y; y >= topRight.Y; y--)
            {
                //tile
                Point point = new Point(x, y);
                if (!FancyWaterfalls.FancyWaterfallPoints.Contains(new WaterfallPoint(point)))
                    continue;
                int xOffset = x - bottomLeft.X;
                int yOffset = bottomLeft.Y - y;
                points.Add(new Point(xOffset, yOffset));
            }
        }
        if (points.Count > 0)
        {
            root["waterfalls"] = points;
        }

        //Save Tile Overlays
        List<TileOverlayStructureSaveData> overlays = new();
        for (int x = bottomLeft.X; x <= topRight.X; x++)
        {
            for (int y = bottomLeft.Y; y >= topRight.Y; y--)
            {

                //tile
                Point point = new Point(x, y);
                if (!TileOverlayUtility.PlacedTileOverlays.ContainsKey(point))
                    continue;
                var data = TileOverlayUtility.PlacedTileOverlays[point];


                int xOffset = x - bottomLeft.X;
                int yOffset = bottomLeft.Y - y;

                overlays.Add(new TileOverlayStructureSaveData
                {
                    x = xOffset,
                    y = yOffset,
                    overlayType = data.overlayType,
                    overlayFrame = data.overlayFrame
                });
            }
        }
        if(overlays.Count > 0)
        {
            root["tileOverlays"] = overlays;
        }
        return root;
    }

    public static void DeSerialize(TagCompound root, Point bottomLeft)
    {
        var waterfalls = root.Get<List<Point>>("waterfalls");
        foreach (var waterfallOffset in waterfalls)
        {
            Point point = new Point(bottomLeft.X + waterfallOffset.X, bottomLeft.Y - waterfallOffset.Y);
            FancyWaterfalls.PlaceWaterfall(point);
        }

        var overlays = root.Get<List<TileOverlayStructureSaveData>>("tileOverlays");
        foreach(var overlay in overlays)
        {
            Point point = new Point(bottomLeft.X + overlay.x, bottomLeft.Y - overlay.y);
            TileOverlayUtility.PlaceTileOverlay(point.X, point.Y, overlay.overlayType, new() { frame = overlay.overlayFrame });
        }
    }

    /// <summary>
    /// Reads the tile entity structure from the stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bottomLeft"></param>
    public static void ReadStruct(Stream stream, Point bottomLeft)
    {
        //Read the nested tag compound or whatever
        TagCompound root = TagIO.FromStream(stream, compressed: true);
        DeSerialize(root, bottomLeft);
    }
    public static void SaveStruct(string fileName, Point bottomLeft, Point topRight)
    {
        TagCompound root = Serialize(bottomLeft, topRight);
        if (root.Count == 0)
        {
            DebugHelper.NewTextOnlyInTesting("No Extra Data Structure Here");
            return;
        }

        //string Path = Main.SavePath + "/" + "ModSources" + "/" + Mod.Name + "/" + "SavedStruct.str";
        string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{fileName}{FileExtensions.EXT_PREFAB}";
        using var stream = File.Open(savePath, FileMode.Create);

        //Save the tag compound to the file
        TagIO.ToStream(root, stream, compress: true);
        stream.Flush();
        DebugHelper.NewTextOnlyInTesting(MESSAGE_SAVED);
    }


    /// <summary>
    /// Reads the file from the ModSources on your puter, this is just for development testing
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="BottomLeft"></param>
    /// <param name="tileBlend"></param>
    public static void ReadSavedStruct(string filePath, Point BottomLeft, int[] tileBlend = null)
    {
        if (filePath.Contains(FileExtensions.EXT_STRUCTURES))
            filePath = filePath.Replace(FileExtensions.EXT_STRUCTURES, FileExtensions.EXT_PREFAB);
        if (!filePath.Contains(FileExtensions.EXT_STRUCTURES))
            filePath += FileExtensions.EXT_PREFAB;
        string savedPath = Main.SavePath + "/ModSources/" + Mod.Name + "/" + filePath;
        if (!File.Exists(savedPath))
            return;

        using var stream = File.Open(savedPath, FileMode.Open);
        ReadStruct(stream, BottomLeft);
    }

    /// <summary>
    /// Reads the tile entity structure in the built mod
    /// </summary>
    /// <param name="Path"></param>
    /// <param name="bottomLeft"></param>
    public static void ReadStruct(string Path, Point bottomLeft)
    {
        string path = Path + FileExtensions.EXT_PREFAB;
        if (!Mod.FileExists(path))
            return;
        using var stream = Mod.GetFileStream(path);
        ReadStruct(stream, bottomLeft);
    }
}
