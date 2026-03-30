using Stellamod.Core.TriggersSystem;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    public static class TriggerStructurizer
    {
        private const string FileExtension = ".tle";
        private static Mod Mod => Stellamod.Instance;
        public static void SaveStruct(string fileName, Point bottomLeft, Point topRight)
        {
            TriggerManager triggerManager = ModContent.GetInstance<TriggerManager>();
            TagCompound root = new TagCompound();
            for (int x = bottomLeft.X; x <= topRight.X; x++)
            {
                for (int y = bottomLeft.Y; y >= topRight.Y; y--)
                {
                    //tile
                    Point point = new Point(x, y);
                    if (triggerManager.TryGetTrigger(point, out var trigger))
                    {
                        int xOffset = x - bottomLeft.X;
                        int yOffset = bottomLeft.Y - y;
                        TagCompound tag = new TagCompound();
                        tag["_x"] = xOffset;
                        tag["_y"] = yOffset;


                        tag["_id"] = trigger.id;
                        if (trigger is ISaveData saveData)
                        {
                            saveData.SaveData(tag);
                        }


                        root[$"{xOffset}{yOffset}"] = tag;
                    }



                }
            }

            if (root.Count == 0)
            {
                DebugHelper.NewTextOnlyInTesting("No Trigger Structure Here");
                return;
            }

            //string Path = Main.SavePath + "/" + "ModSources" + "/" + Mod.Name + "/" + "SavedStruct.str";
            string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{fileName}{FileExtension}";
            using var stream = File.Open(savePath, FileMode.Create);

            //Save the tag compound to the file
            TagIO.ToStream(root, stream, compress: true);
            stream.Flush();
            DebugHelper.NewTextOnlyInTesting("Trigger Structure Saved");
        }

        /// <summary>
        /// Reads the tile entity structure from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bottomLeft"></param>
        public static void ReadStruct(Stream stream, Point bottomLeft)
        {
            TriggerManager triggerManager = ModContent.GetInstance<TriggerManager>();
            //Read the nested tag compound or whatever
            TagCompound root = TagIO.FromStream(stream, compressed: true);
            foreach (var tag in root)
            {
                TagCompound element = (TagCompound)tag.Value;
                int xOffset = element.Get<int>("_x");
                int yOffset = element.Get<int>("_y");
                int type = element.Get<int>("_id");
                Trigger trigger = TriggerFactory.Create((TriggerID)type);
                if (trigger is ISaveData saveData)
                {
                    saveData.LoadData(element);
                }
                Point point = new Point(bottomLeft.X + xOffset, bottomLeft.Y - yOffset);
                triggerManager.PlaceTrigger(point, trigger);
               // DebugHelper.NewTextOnlyInTesting("Construct Trigger " + trigger);

                Dust.QuickBox(new Vector2(point.X, point.Y) * 16, new Vector2(point.X + 1, point.Y + 1) * 16, 2, Color.Red, null);
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
            string path = Path + FileExtension;
            if (!Mod.FileExists(path))
                return;
            using var stream = Mod.GetFileStream(path);
            ReadStruct(stream, bottomLeft);

        }
    }
}
