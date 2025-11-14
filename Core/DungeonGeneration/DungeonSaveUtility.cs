using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Colosseum.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Core.DungeonGeneration
{
    public class DungeonSaveHelper : ModItem
    {
        private int _useIndex;
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {
            DungeonSaveUtility.FindAndWriteDungeonWorldToPrefabFiles();
            return true;
        }
    }

    /// <summary>
    /// Helper class for saving/loading the new dungeon generation
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
                List<PlacedDoor> orderedPoints = topRight.OrderBy(x => MathF.Abs(x.point.X - anchor.point.X)).ToList();

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
                Dust.QuickBox(rectangle.TopLeft(), rectangle.BottomRight(), 64, Color.White, (Dust d) => { });
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

        /// <summary>
        /// Returns an array of all prefabs from the dungeon files
        /// </summary>
        /// <returns></returns>
        public static Room[] ReadDungeonPrefabsFromFiles()
        {
            Mod mod = Stellamod.Instance;
            List<Room> rooms = new List<Room>();
            foreach (var file in mod.GetFileNames())
            {
                //Dungeon file
                if (file.Contains(DungeonGenerationHelper.FileExtension))
                {
                    string structureFile = file.Replace(DungeonGenerationHelper.FileExtension, ".str");
                    string prefab = structureFile.Replace(".str", string.Empty);

                    Rectangle rectangle = default;
                    using (var stream = mod.GetFileStream(structureFile))
                    {
                        rectangle = Structurizer.ReadRectangle(stream); 
                    }
                    PlacedDoor[] placedDoors;
                    using (var stream = mod.GetFileStream(file))
                    {
                        placedDoors = DungeonGenerationHelper.DoorsFromStream(stream);
                    }

                   
                    Room room = new Room(prefab, rectangle, placedDoors);
                    rooms.Add(room);
                }
            }
            return rooms.ToArray();
        }
    }
}
