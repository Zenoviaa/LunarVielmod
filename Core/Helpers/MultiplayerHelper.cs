using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Stellamod.Core.Helpers
{
    public static class MultiplayerHelper
    {
        public static bool IsHost => Main.netMode != NetmodeID.MultiplayerClient;
        public static void WriteItemList(this BinaryWriter writer, List<Item> arr)
        {
            writer.Write(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                writer.Write(arr[i].type);
            }
        }
        public static List<Item> ReadItemList(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            List<Item> itemList = new List<Item>();
            for (int i = 0; i < length; i++)
            {
                itemList.Add(new Item(reader.ReadInt32()));
            }
            return itemList;
        }

        public static void WriteItemArray(this BinaryWriter writer, Item[] arr)
        {
            writer.Write(arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                writer.Write(arr[i].type);
            }
        }
        public static Item[] ReadItemArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            Item[] array = new Item[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = new Item(reader.ReadInt32());
            }
            return array;
        }
    }
}