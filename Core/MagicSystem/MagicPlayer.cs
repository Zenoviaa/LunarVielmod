using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.MagicSystem
{
    internal class MagicPlayer : ModPlayer
    {
        private List<Item> _unlockedItems;
        public List<Item> UnlockedItems
        {
            get
            {
                if(_unlockedItems == null)
                    _unlockedItems = new List<Item>();
                return _unlockedItems;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["unlockednum"] = UnlockedItems.Count;
            for(int i = 0; i < UnlockedItems.Count; i++)
            {
                string key = $"unlocked_{i}";
                tag[key] = UnlockedItems[i];
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            UnlockedItems.Clear();
            int num = tag.GetInt("unlockednum");
            for(int n = 0; n < num; n++)
            {
                string key = $"unlocked_{n}";
                Item item = tag.Get<Item>(key);
                UnlockedItems.Add(item);
            }
        }
    }
}
