using Stellamod.Core.XixianFlaskSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.SummonerSystem
{
    public class BellPlayer : ModPlayer
    {
        private List<int> _itemTypes = new List<int>();
        private List<Item> _minions = new List<Item>();
        private List<Item> _unlockedminions = new List<Item>();
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["minions"] = _minions;
            tag["unlockedminions"] = _unlockedminions;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            _minions = new List<Item>();
            _minions = tag.Get<List<Item>>("minions");

            var u = tag.Get<List<Item>>("unlockedminions");
            _unlockedminions = u;
            ManageUnlockedMinions();
        }

        private void ManageUnlockedMinions()
        {
            _unlockedminions.RemoveAll(x => x.IsAir);
            _unlockedminions = _unlockedminions.Distinct().ToList();
            _itemTypes.Clear();
            foreach (var item in _unlockedminions)
            {
                _itemTypes.Add(item.type);
            }
        }
        public List<Item> GetMinions()
        {
            return _minions;
        }

        public void SetMinionAtIndex(Item item, int index)
        {
            List<Item> minions = GetMinions();
            while (minions.Count <= index)
            {
                Item emptyItem = new Item();
                emptyItem.SetDefaults(0);
                minions.Add(emptyItem);
            }
            minions[index] = item;
        }

        public Item GetMinionAtIndex(int index)
        {
            List<Item> minions = GetMinions();
            if (minions.Count > index)
            {
                return minions[index];
            }
            Item air = new Item(0);
            air.SetDefaults(0);
            return air;
        }

        public bool HasUnlocked(Item item)
        {
            return _itemTypes.Contains(item.type);
        }

        public bool HasUnlockedBell()
        {
            return true;
        }

        public void UnlockMinion(Item item)
        {
            _unlockedminions.Add(item);
            ManageUnlockedMinions();
        }
        public void UnlockFlask()
        {

        }

        public void ResetProgress()
        {
            _unlockedminions.Clear();
            ManageUnlockedMinions();
        }

        public void GrantAllProgress()
        {
            _unlockedminions.Clear();
            IEnumerable<ModItem> insources = ModContent.GetContent<InsourceItem>();
            foreach (var insource in insources)
            {
                _unlockedminions.Add(insource.Item);
            }

            ManageUnlockedMinions();
        }

        public bool CanUseFlask()
        {
            return true;
        }
    }
}
