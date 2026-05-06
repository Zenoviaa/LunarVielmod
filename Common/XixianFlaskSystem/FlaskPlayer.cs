using Stellamod.Common.ArmorRework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.XixianFlaskSystem
{
    public class FlaskPlayer : ModPlayer
    {
        private List<Item> _insources = new List<Item>();
        private List<Item> _unlockedInsources = new List<Item>();
        private List<int> _itemTypes = new List<int>();

        public int maxInsourceCount;
        public int insourceTime;
        public bool unlockedFlask;
        public float insourceSecondsBonusPerInsource;
        public static event Action<Player> OnProc;
        public override void ResetEffects()
        {
            base.ResetEffects();
            maxInsourceCount = 1;
            insourceTime = 0;
            insourceSecondsBonusPerInsource = 0;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.myPlayer == Player.whoAmI && LunarVeilKeybinds.FlaskKeybind.JustPressed)
            {
                if (CanUseFlask())
                {
                    ProcEffects();
                }
            }
        }

        public void ResetProgress()
        {
            unlockedFlask = false;
            _unlockedInsources.Clear();
            ManageUnlockedInsources();
        }
        public void GrantAllProgress()
        {
            _unlockedInsources.Clear();
            IEnumerable<ModItem> insources = ModContent.GetContent<InsourceItem>();
            foreach (var insource in insources)
            {
                _unlockedInsources.Add(insource.Item);
            }

            ManageUnlockedInsources();
        }

        public bool CanUseFlask()
        {
            if (!unlockedFlask)
                return false;
            return !Player.HasBuff<CannotUseFlask>() && !Player.HasBuff(BuffID.PotionSickness);
        }

        public bool HasUnlockedFlask()
        {
            return unlockedFlask;
        }

        public void UnlockFlask()
        {
            if (unlockedFlask)
                return;

            unlockedFlask = true;
        }

        public void ProcEffects()
        {
            var insources = GetInsources();
            int insoureCount = 0;
            for (int i = 0; i < maxInsourceCount && i < insources.Count; i++)
            {
                insoureCount++;
                var item = insources[i];
                if (item.ModItem is InsourceItem myInsource)
                {
                    myInsource.PreUseInsource(this);
                }
            }


            for (int i = 0; i < maxInsourceCount && i < insources.Count; i++)
            {
                var item = insources[i];
                if (item.ModItem is InsourceItem myInsource)
                {
                    myInsource.UseInsource(this);
                }
            }

            float time = insourceTime;

            ArmorStatsPlayer statsPlayer = Player.GetModPlayer<ArmorStatsPlayer>();
            time *= (1f - statsPlayer.insourceTimeBonus);
            time += insourceSecondsBonusPerInsource * insoureCount;

            SoundStyle xixianFlaskUseSound = SoundID.Item3;
            SoundEngine.PlaySound(xixianFlaskUseSound);
            
            Player.AddBuff(ModContent.BuffType<CannotUseFlask>(), insourceTime);
            Player.AddBuff(BuffID.PotionSickness, insourceTime);
            OnProc?.Invoke(Player);
        }

        public bool HasUnlocked(Item item)
        {
            return _itemTypes.Contains(item.type);
        }

        public void UnlockInsource(Item item)
        {
            _unlockedInsources.Add(item);
            ManageUnlockedInsources();
        }

        private void ManageUnlockedInsources()
        {
            _unlockedInsources.RemoveAll(x => x.IsAir);
            _unlockedInsources = _unlockedInsources.Distinct().ToList();
            _itemTypes.Clear();
            foreach (var item in _unlockedInsources)
            {
                _itemTypes.Add(item.type);
            }
        }

        public List<Item> GetInsources()
        {
            return _insources;
        }

        public void SetInsourceAtIndex(Item item, int index)
        {
            List<Item> insources = GetInsources();
            while (insources.Count <= index)
            {
                Item emptyItem = new Item();
                emptyItem.SetDefaults(ItemID.None);
                insources.Add(emptyItem);
            }
            insources[index] = item;
        }

        public Item GetInsourceAtIndex(int index)
        {
            List<Item> insources = GetInsources();
            if (insources.Count > index)
            {
                return insources[index];
            }
            Item air = new Item(0);
            air.SetDefaults(ItemID.None);
            return air;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["insources"] = _insources;
            tag["unlockedinsources"] = _unlockedInsources;
            tag["unlockedFlask"] = unlockedFlask;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            var insources = tag.Get<List<Item>>("insources");
            _insources = insources;

            var unlockedinsources = tag.Get<List<Item>>("unlockedinsources");
            _unlockedInsources = unlockedinsources;
            ManageUnlockedInsources();

            unlockedFlask = tag.GetBool("unlockedFlask");
        }
    }
}
