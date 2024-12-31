using Stellamod.UI.AdvancedMagicSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Items.MoonlightMagic
{
    internal class AdvancedMagicPlayer : ModPlayer
    {
        public List<Item> Backpack { get; set; } = new List<Item>();
        public static event Action<Item> MagicPickupEvent;


        public void Pickup(Item item)
        {
            var uiSystem = ModContent.GetInstance<AdvancedMagicUISystem>();
            bool success = false;
            int updateIndex = 0;
            for(int i = 0; i < Backpack.Count; i++)
            {
                if (Backpack[i].IsAir)
                {
                    Backpack[i] = item;
                    updateIndex = i;
                    success = true;
                    break;
                }
            }

            if (!success)
            {
                updateIndex = Backpack.Count;
                Backpack.Add(item);
            }    

            int airCount = 0;
            for (int i = 0; i < Backpack.Count; i++)
            {
                if (Backpack[i].IsAir)
                {
                    airCount++;
                }
            }

            Backpack.RemoveAll(x => x.IsAir);
            if (airCount == 0)
            {
                Item emptyItem = new Item();
                emptyItem.SetDefaults(0);
                Backpack.Add(emptyItem);
            }

            uiSystem.Recalculate();
            MagicPickupEvent?.Invoke(item);
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            bool isBackpackEmpty = Backpack.Count == 0;
            bool isFinalSlotFull = Backpack.Count > 0 && !Backpack[Backpack.Count - 1].IsAir;
            if (isBackpackEmpty || isFinalSlotFull)
            {
                var uiSystem = ModContent.GetInstance<AdvancedMagicUISystem>();
                Item emptyItem = new Item();
                emptyItem.SetDefaults(0);
                Backpack.Add(emptyItem);
                uiSystem.Recalculate();
            }

            for(int i = 0; i < Backpack.Count; i++)
            {
                if (Backpack[i].IsAir && i + 1 < Backpack.Count)
                {
                    var uiSystem = ModContent.GetInstance<AdvancedMagicUISystem>();
                    Backpack.RemoveAt(i);
                    uiSystem.Recalculate();
                    break;
                }
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
        }
    }
}
