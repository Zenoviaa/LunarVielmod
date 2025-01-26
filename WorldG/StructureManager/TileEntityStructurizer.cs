using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.WorldG.StructureManager
{
    internal static class TileEntityStructurizer
    {
        private const string FileExtension = ".tle";
        private static Mod Mod = ModContent.GetInstance<Stellamod>();
        public static void SaveStruct(string fileName, Point bottomLeft, Point topRight)
        {
            TagCompound root = new TagCompound();
            for (int x = (int)(bottomLeft.X); x <= topRight.X; x++)
            {
                for (int y = (int)(bottomLeft.Y); y >= topRight.Y; y--)
                {
                    //tile
                    Point16 point = new Point16(x, y);
                    if (!TileEntity.ByPosition.ContainsKey(point))
                        continue;
                    TileEntity tileEntity = TileEntity.ByPosition[point];
                    ModTileEntity modTileEntity = tileEntity as ModTileEntity;
                    if (modTileEntity == null)
                        continue;

                    int xOffset = x - bottomLeft.X;
                    int yOffset = bottomLeft.Y - y;
                    TagCompound tag = new TagCompound();
                    tag["_x"] = xOffset;
                    tag["_y"] = yOffset;


                    tag["_type"] = tileEntity.GetType().Name;

                    tileEntity.SaveData(tag);
                    root[$"{xOffset}{yOffset}"] = tag;
                }
            }

            if (root.Count == 0)
            {
                Main.NewText("No Tile Entity Structure Here");
                return;
            }

            //string Path = Main.SavePath + "/" + "ModSources" + "/" + Mod.Name + "/" + "SavedStruct.str";
            string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{fileName}{FileExtension}";
            using var stream = File.Open(savePath, FileMode.Create);

            //Save the tag compound to the file
            TagIO.ToStream(root, stream, compress: true);
            stream.Flush();
            Main.NewText("Tile Entity Structure Saved");
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
            foreach (var tag in root)
            {
                TagCompound element = (TagCompound)tag.Value;
                int xOffset = element.Get<int>("_x");
                int yOffset = element.Get<int>("_y");
                string type = element.Get<string>("_type");
                ModTileEntity template = ModContent.Find<ModTileEntity>(Mod.Name + "/" + type);
                Main.NewText("Construct Tile Entity " + template.Name);

                Point16 point = new Point16(bottomLeft.X + xOffset, bottomLeft.Y - yOffset);
                Dust.QuickBox(new Vector2(point.X, point.Y) * 16, new Vector2(point.X + 1, point.Y + 1) * 16, 2, Color.Red, null);

                //Place the tile entity
                template.Place(point.X, point.Y);
                ModTileEntity entity = TileEntity.ByPosition[point] as ModTileEntity;
                entity.LoadData(element);
            }
        }

        /// <summary>
        /// Reads the file from the ModSources on your puter, this is just for development testing
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="BottomLeft"></param>
        /// <param name="tileBlend"></param>
        public static void ReadSavedStruct(string filePath, Point BottomLeft, int[] tileBlend = null)
        {
            if (filePath.Contains(".str"))
                filePath = filePath.Replace(".str", FileExtension);
            if (!filePath.Contains(FileExtension))
                filePath += FileExtension;
            string savedPath = Main.SavePath + "/ModSources/" + Mod.Name + "/" + filePath;
            if (File.Exists(savedPath))
            {
                using (var stream = File.Open(savedPath, FileMode.Open))
                {
                    ReadStruct(stream, BottomLeft);
                }
            }
        }

        /// <summary>
        /// Reads the tile entity structure in the built mod
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="bottomLeft"></param>
        public static void ReadStruct(string Path, Point bottomLeft)
        {
            string path = Path + FileExtension;
            if (!Mod.FileExists(path))
                return;
            using (var stream = Mod.GetFileStream(path))
            {
                ReadStruct(stream, bottomLeft);
            }

        }
    }
}
