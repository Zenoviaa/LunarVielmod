using Stellamod.Core.MagicSystem.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class AdvancedMagicPlayer : ModPlayer
    {
        private List<int> _itemTypes = new List<int>();
        public List<Item> Backpack { get; set; } = new List<Item>();


        public float chargeTimeBonus;
        public float chargeDamageBonus;
        public bool overchargingVisual;

        public static event Action<Item> OnPickupMagicItem;

        public override void ResetEffects()
        {
            base.ResetEffects();
            chargeTimeBonus = 0f;
            overchargingVisual = false;
        }

        public bool IsUnlocked(Item item)
        {
            return _itemTypes.Contains(item.type);
        }

        public void Pickup(Item item)
        {
            var uiSystem = ModContent.GetInstance<MagicUISystem>();
            Backpack.Add(item);
            ManageMagicItems();
            OnPickupMagicItem?.Invoke(item);
        }
        public void ResetProgress()
        {
            Backpack.Clear();
            ManageMagicItems();
        }
        public void GrantAllProgress()
        {
            Backpack.Clear();
            IEnumerable<BaseEnchantment> enchantments = ModContent.GetContent<BaseEnchantment>();
            foreach (var enchantment in enchantments)
            {
                Backpack.Add(enchantment.Item);
            }

            IEnumerable<BaseElement> elements = ModContent.GetContent<BaseElement>();
            foreach (var element in elements)
            {
                Backpack.Add(element.Item);
            }
            ManageMagicItems();
        }

        private void ManageMagicItems()
        {
            Backpack.RemoveAll(x => x.IsAir);
            Backpack = Backpack.Distinct().ToList();
            _itemTypes.Clear();
            foreach (var item in Backpack)
            {
                _itemTypes.Add(item.type);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["magicbackpack_itemCount"] = Backpack.Count;
            for (int i = 0; i < Backpack.Count; i++)
            {
                var enchantment = Backpack[i];
                if (enchantment == null)
                    continue;
                tag[$"magicbackpack_enchantment_{i}"] = enchantment;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("magicbackpack_itemCount"))
            {
                int itemCount = tag.GetInt("magicbackpack_itemCount");
                Backpack.Clear();
                for (int i = 0; i < itemCount; i++)
                {
                    if (tag.ContainsKey($"magicbackpack_enchantment_{i}"))
                    {
                        var enchantment = tag.Get<Item>($"magicbackpack_enchantment_{i}");
                        Backpack.Add(enchantment);
                    }
                }
            }
            ManageMagicItems();
        }
    }
}
