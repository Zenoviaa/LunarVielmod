using Stellamod.Common.MagicSystem.UI;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class AdvancedMagicPlayer : ModPlayer
    {
        private List<int> _itemTypes = new List<int>();
        public List<Item> Backpack { get; set; } = new List<Item>();


        public float chargeTimeBonus;
        public float chargeWidthBonus;
        public float chargeDamageBonus;
        public float chargeDamagePenalty;
        public bool overchargingVisual;
        public bool hasMiniWand;
        public Item miniWand;
        public static event Action<Item> OnPickupMagicItem;

        public override void ResetEffects()
        {
            base.ResetEffects();
            chargeTimeBonus = 0f;
            chargeDamagePenalty = 0f;
            overchargingVisual = false;
            hasMiniWand = false;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(hasMiniWand && item.ModItem is AbstractMagicWand)
            {
                var wandProj = Projectile.NewProjectileDirect(source, position, velocity, miniWand.shoot, damage, knockback, Player.whoAmI);
                if(wandProj.ModProjectile is AdvancedMagicStaffHold subWand)
                {
                    subWand.castMiniWand = true;
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        public void ResetChargeEffects()
        {
            chargeTimeBonus = 0f;
            chargeWidthBonus = 0f;
            chargeDamageBonus = 0f;
 
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
            tag["enchantments"] = Backpack;
            
            if(miniWand == null)
            {
                miniWand = new Item(0);
                miniWand.SetDefaults(0);
            }

            tag["miniwand"] = ItemIO.Save(miniWand);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            Backpack.Clear();
            Backpack = tag.Get<List<Item>>("enchantments");
            ManageMagicItems();
            miniWand = tag.Get<Item>("miniwand");
            if(miniWand == null)
            {
                miniWand = new Item(0);
                miniWand.SetDefaults(0);
            }
        }
    }
}
