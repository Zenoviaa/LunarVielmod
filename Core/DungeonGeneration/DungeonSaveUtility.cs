using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Core.DungeonGeneration
{
    /// <summary>
    /// Helper class for saving the new dungeon generation
    /// </summary>
    public static class DungeonSaveUtility
    {

        /// <summary>
        /// Finds all the dungeon rectangles and writes them to file
        /// </summary>
        public static void FindAndWriteDungeonWorldToPrefabFiles()
        {
            Rectangle[] dungeonRectangles = FindDungeonRectangles();
            WriteDungeonRectanglesToFile(dungeonRectangles);
        }

        /// <summary>
        /// Finds all dungeon rectangles in the world
        /// </summary>
        /// <returns></returns>
        public static Rectangle[] FindDungeonRectangles()
        {
            //First get all the anchors
            PlacedDoor[] anchors = DungeonGenerationHelper.GetAnchors();

            //Next we need to loop over them and create their rectangles
            Queue<PlacedDoor> bottomLeft = new Queue<PlacedDoor>();
            List<PlacedDoor> topRight = new List<PlacedDoor>();
            for (int i = 0; i < anchors.Length; i++)
            {
                PlacedDoor anchor = anchors[i];
                if (anchor.door == Door.AnchorBottomLeft)
                {
                    bottomLeft.Enqueue(anchor);
                }
                else if (anchor.door == Door.AnchorTopRight)
                {
                    topRight.Add(anchor);
                }
            }

            List<Rectangle> prefabRects = new List<Rectangle>();
            while (bottomLeft.Count > 0)
            {
                PlacedDoor anchor = bottomLeft.Dequeue();
                Vector2 position = anchor.point.ToVector2();


                //Order by just their x distance, on need to consider y
                //If we consider y we may have a rare bug where it uses a different point
                List<PlacedDoor> orderedPoints = topRight.OrderBy(x => MathF.Abs(x.point.Y - anchor.point.Y)).ToList();

                //Remove all elements that are either to the left or below this, because that would be an invalid room
                //So this guarantees it gets the closest one that is above/right of it
                orderedPoints.RemoveAll(x => x.point.Y > anchor.point.Y || x.point.X < anchor.point.X);
                PlacedDoor anchorToPairWith = orderedPoints[0];
                topRight.Remove(anchorToPairWith);


                //Calculate the rectangle of this prefab
                int x = anchor.point.X;
                int y = anchorToPairWith.point.Y;
                int width = (anchorToPairWith.point.X - anchor.point.X);
                int height = (anchor.point.Y - anchorToPairWith.point.Y);
                Rectangle prefabRectangle = new Rectangle(x, y, width, height);
                prefabRects.Add(prefabRectangle);
            }

            return prefabRects.ToArray();
        }

        /// <summary>
        /// Takes an array of rectangles, and saves them to several dungeon save structures in the files
        /// </summary>
        /// <param name="rectangles"></param>
        public static void WriteDungeonRectanglesToFile(Rectangle[] rectangles)
        {
            Mod Mod = Stellamod.Instance;
            for (int r = 0; r < rectangles.Length; r++)
            {
                Rectangle rectangle = rectangles[r];
                Point bottomLeft = new Point(rectangle.Left, rectangle.Bottom);
                Point topRight = new Point(rectangle.Right, rectangle.Top);


                string fileName = $"Room{r}";
                string structurePath = $"Dungeon/{fileName}";
                string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{structurePath}{DungeonGenerationHelper.FileExtension}";
                using var doorStream = File.Open(savePath, FileMode.Create);
                DungeonGenerationHelper.SaveDoors(doorStream, bottomLeft, topRight);

                Structurizer.SaveStruct(structurePath, bottomLeft, topRight);
                TriggerStructurizer.SaveStruct(structurePath, bottomLeft, topRight);
                TileEntityStructurizer.SaveStruct(structurePath, bottomLeft, topRight);
            }
        }
    }
}
