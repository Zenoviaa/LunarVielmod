using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities
{
    public static class SaveUtil
    {
        /// <summary>
        /// Writes an item list to a single string, so it takes up less space in the mod player
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ItemListToString(List<Item> input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Count; i++)
            {
                Item item = input[i];
                sb.AppendLine(item.ModItem.GetType().Name);
            }

            string sbString = sb.ToString();
            return sbString;
        }

        /// <summary>
        /// Reads an item list from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<Item> StringToItemList(string input)
        {
            List<Item> items = new List<Item>();
            string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Mod mod = Stellamod.Instance;
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (mod.TryFind(s, out ModItem item))
                {
                    items.Add(item.Item);
                }
            }
            return items;
        }
    }
}
