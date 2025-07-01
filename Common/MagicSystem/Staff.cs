using Stellamod.Common.MagicSystem.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.MagicSystem
{
    internal abstract class Staff : ModItem
    {
        private Element _element;
        private List<Item> _enchantmentItems;
        public Element Element
        {
            get
            {
                if (_element == null)
                    _element = new NoElement();
                return _element;
            }
            set
            {
                _element = value;
            }
        }

        public List<Item> EnchantmentItems
        {
            get
            {
                if (_enchantmentItems == null)
                    _enchantmentItems = new List<Item>();
                return _enchantmentItems;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            //Set some defaults for the staff
            Item.damage = 9;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.autoReuse = true;

            //Shoot magic projectile of course
            Item.mana = 10;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<MagicProjectile>();
        }

        public List<Enchantment> GetEnchantments()
        {
            List<Enchantment> enchantments = new List<Enchantment>();
            foreach(var item in _enchantmentItems)
            {
                if(item.ModItem is Enchantment enchantment)
                {
                    enchantments.Add(enchantment);
                }
            }
            return enchantments;
        }

        internal void SetEnchantmentAtIndex(Item item, int index)
        {
            if (EnchantmentItems.Count > index)
            {
                EnchantmentItems[index] = item;
            }
            else
            {
                EnchantmentItems.Insert(index, item);
            }
        }

        internal Item GetEnchantmentAtIndex(int index)
        {          
            if(EnchantmentItems.Count > index)
            {
                Item item = EnchantmentItems[index];
                if(item == null)
                {
                    Item airItem = new Item();
                    airItem.SetDefaults(0);
                    EnchantmentItems[index] = airItem;
                    return airItem;
                }
                else
                {
                    return item;
                }
            }
            Item airItem2 = new Item();
            airItem2.SetDefaults(0);
            EnchantmentItems.Insert(index, airItem2);
            return airItem2;
        }

        public virtual int GetNormalSlotCount()
        {
            return 5;
        }

        public virtual int GetTimedSlotCount()
        {
            return 2;
        }
        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            base.RightClick(player);
            ModContent.GetInstance<MagicUISystem>().OpenUI(this);
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["enchantnum"] = EnchantmentItems.Count;
            for (int i = 0; i < EnchantmentItems.Count; i++)
            {
                string key = $"enchant_{i}";
                tag[key] = EnchantmentItems[i];
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            EnchantmentItems.Clear();
            int num = tag.GetInt("enchantnum");
            for (int n = 0; n < num; n++)
            {
                string key = $"enchant_{n}";
                Item item = tag.Get<Item>(key);
                EnchantmentItems.Add(item);
            }
        }
    }
}
