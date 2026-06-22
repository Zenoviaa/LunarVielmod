using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Items.Weapons.Mage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public abstract class AbstractMagicWand : ModItem
    {
        public Texture2D Form { get; set; }
        public BaseMovement Movement { get; set; }
        public int Size { get; set; }
        public int TrailLength { get; set; }

        //Enchantment Attributes
        public Item primaryElement;
        public List<Item> normalEnchantments;
        public List<Item> timedEnchantments;
        public int normalSlotCount;
        public int timedSlotCount;
        //Seal this so we don't accidentally override the base functionality
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.mana = 10;

            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.shoot = ModContent.ProjectileType<AdvancedMagicProjectile>();
            Item.autoReuse = true;
            TrailLength = 16;
            Size = 16;
      
            Item.shoot = ModContent.ProjectileType<AdvancedMagicStaffHold>();
            Item.shootSpeed = 15;
            Item.channel = true;
            Item.autoReuse = false;
            SetWandDefaults();
            SetDefaults2();
        }

        private void SetWandDefaults()
        {
            primaryElement = new Item();
            primaryElement.SetDefaults(ItemID.None);
            normalEnchantments = new List<Item>();
            timedEnchantments = new List<Item>();
            normalSlotCount = 3;
            timedSlotCount = 4;
        }

        public virtual void SetDefaults2()
        {

        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public virtual void ModifyElementPreferences(List<int> elements)
        {

        }

        public Item[] GetEquippedEnchantments(Player player)
        {
            List<Item> equippedEnchantments = new List<Item>();
            int normalEnchantmentCount = GetCombinedNormalSlotCount(player);
            int timedEnchantmentCount = GetCombinedTimedSlotCount(player);
            for(int n = 0; n < normalEnchantmentCount && n < normalEnchantments.Count; n++)
            {
                equippedEnchantments.Add(normalEnchantments[n]);
            }
            for (int n = 0; n < timedEnchantmentCount && n < timedEnchantments.Count; n++)
            {
                equippedEnchantments.Add(timedEnchantments[n]);
            }
            return equippedEnchantments.ToArray();
        }

        public bool IsMatchingPreference()
        {
            if (primaryElement.IsAir)
                return true;
            if (primaryElement.ModItem.Type == ModContent.ItemType<BasicElement>())
                return true;
            List<int> elements = new List<int>();
            ModifyElementPreferences(elements);
            if (elements.Count == 0)
                return true;
            foreach(int e in elements)
            {
                if(primaryElement.ModItem.Type == e)
                {
                    return true;
                }
            }

            return false;
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            ItemIO.Send(primaryElement, writer);
            writer.Write(normalEnchantments.Count);
            for (int i = 0; i < normalEnchantments.Count; i++)
            {
                ItemIO.Send(normalEnchantments[i], writer);
            }
            writer.Write(timedEnchantments.Count);
            for (int i = 0; i < timedEnchantments.Count; i++)
            {
                ItemIO.Send(timedEnchantments[i], writer);
            }

        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            primaryElement = ItemIO.Receive(reader);
            int length = reader.ReadInt32();
            while (normalEnchantments.Count <= length)
            {
                Item air = new Item();
                air.SetDefaults(0);
                normalEnchantments.Add(air);
            }
            for (int i = 0; i < length; i++)
            {
                normalEnchantments[i] = ItemIO.Receive(reader);
            }

            length = reader.ReadInt32();
            while (timedEnchantments.Count <= length)
            {
                Item air = new Item();
                air.SetDefaults(0);
                timedEnchantments.Add(air);
            }
            for (int i = 0; i < length; i++)
            {
                timedEnchantments[i] = ItemIO.Receive(reader);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(player, ref damage);
            float damageModifier = 1f;
            for (int i = 0; i < normalEnchantments.Count; i++)
            {
                Item item = normalEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    //base 5% damage buff per element
                    damageModifier += 0.05f;
                    if (primaryElement.ModItem is BaseElement element)
                    {
                        if (element.IsSynergizingWith(element.Type))
                        {
                             damageModifier += 0.05f;
                        }
                    }
                }
            }
            for (int i = 0; i < timedEnchantments.Count; i++)
            {
                Item item = timedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    //base 5% damage buff per element
                    damageModifier += 0.05f;
                    if (primaryElement.ModItem is BaseElement element)
                    {
                        if (element.IsSynergizingWith(element.Type))
                        {
                            damageModifier += 0.05f;
                        }
                    }
                }
            }

            AdvancedMagicPlayer magicPlayer = player.GetModPlayer<AdvancedMagicPlayer>();
            damageModifier -= magicPlayer.chargeDamagePenalty;
            if (!IsMatchingPreference())
            {
                damageModifier -= 0.3f;
            }
            damage *= damageModifier;
        }


        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(player, ref reduce, ref mult);
            for (int i = 0; i < normalEnchantments.Count; i++)
            {
                Item item = normalEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    mult += enchantment.GetStaffManaModifier();
                }
            }
            for (int i = 0; i < timedEnchantments.Count; i++)
            {
                Item item = timedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    mult += enchantment.GetStaffManaModifier();
                }
            }
        }

        public override ModItem Clone(Item newEntity)
        {
            ModItem clone = base.Clone(newEntity);
            AbstractMagicWand staff = clone as AbstractMagicWand;
            staff.normalEnchantments = new List<Item>();
            for(int i = 0; i < normalEnchantments.Count; i++)
            {
                staff.normalEnchantments.Add(normalEnchantments[i].Clone());
            }
            staff.timedEnchantments = new List<Item>();
            for (int i = 0; i < timedEnchantments.Count; i++)
            {
                staff.timedEnchantments.Add(timedEnchantments[i].Clone());
            }
            staff.primaryElement = primaryElement.Clone();
            return staff;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

            TooltipLine tooltipLine;

            tooltipLine = new TooltipLine(Mod, "WeaponType",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonStaff"));
            tooltipLine.OverrideColor = Color.White;
            tooltips.Add(tooltipLine);

            tooltipLine = new TooltipLine(Mod, "EnchantHelp",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonStaffHelp"));
            tooltipLine.OverrideColor = Color.Gray;
            tooltips.Add(tooltipLine);

            List<int> preferences = new List<int>();
            ModifyElementPreferences(preferences);

            string preferenceString = "";
            if(preferences.Count > 0)
            {
                for (int p = 0; p < preferences.Count; p++)
                {
                    int preferenceType = preferences[p];
                    preferenceString += $"[i:{preferenceType}]";

                }
                tooltipLine = new TooltipLine(Mod, "Preferences", "Preferences " + preferenceString);
                tooltips.Add(tooltipLine);
            }

            if (!IsMatchingPreference())
            {
                tooltipLine = new TooltipLine(Mod, "EnchantHelp",
                  Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentMismatch"));
                tooltipLine.OverrideColor = Color.Red;
                tooltips.Add(tooltipLine);
            }

            for (int i = 0; i < normalEnchantments.Count; i++)
            {
                var item = normalEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    tooltipLine = new TooltipLine(Mod, $"MoonMagicEnchant_{i}", $"[i:{enchantment.Type}] " + enchantment.DisplayName.Value);
                    tooltips.Add(tooltipLine);
                }
            }
            for (int i = 0; i < timedEnchantments.Count; i++)
            {
                var item = timedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    tooltipLine = new TooltipLine(Mod, $"MoonMagicEnchantT_{i+normalEnchantments.Count}", $"[i:{enchantment.Type}] " + enchantment.DisplayName.Value);
                    tooltips.Add(tooltipLine);
                }
            }
        }

        public int GetCombinedNormalSlotCount(Player player)
        {
            ArmorStatsPlayer armorStats = player.GetModPlayer<ArmorStatsPlayer>();
            return normalSlotCount + armorStats.wandNormalEnchantmentSlots;
        }

        public int GetCombinedTimedSlotCount(Player player)
        {
            ArmorStatsPlayer armorStats = player.GetModPlayer<ArmorStatsPlayer>();
            return timedSlotCount + armorStats.wandTimerEnchantmentSlots;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            base.RightClick(player);
            AbstractMagicWand wand = Item.ModItem as AbstractMagicWand;
            ModContent.GetInstance<MagicUISystem>().OpenUI(wand);
        }

        public void SetElement(Item item)
        {
            primaryElement = item;
        }

        public Item GetElement()
        {
            if (primaryElement == null || primaryElement.IsAir || primaryElement.type == 0)
            {
                Item item = new Item(ModContent.ItemType<BasicElement>());
                return item;
            }
                
            return primaryElement;
        }

        public void SetEnchantment(Item item, int index, bool isTimedSlot)
        {
            List<Item> itemList = isTimedSlot ? ref timedEnchantments : ref normalEnchantments;
            while(itemList.Count <= index)
            {
                Item air = new Item();
                air.SetDefaults(0);
                itemList.Add(air);
            }
            itemList[index] = item;
        }

        public Item GetEnchantment(int index, bool isTimedSlot)
        {
            List<Item> itemList = isTimedSlot ? ref timedEnchantments : ref normalEnchantments;
            if (itemList.Count > index)
            {
                Item item = itemList[index];
                return item;
            }
            Item airItem2 = new Item();
            airItem2.SetDefaults(ItemID.None);
            return airItem2;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (primaryElement.ModItem is BaseElement element)
            {
                element.SpecialInventoryDraw(Item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }

            for (int i = 0; i < normalEnchantments.Count; i++)
            {
                var enchant = normalEnchantments[i];
                if (enchant.ModItem is BaseEnchantment enchantment)
                {
                    enchantment.SpecialInventoryDraw(Item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
                }
            }
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["element"] = primaryElement;
            tag["enchantments"] = normalEnchantments;
            tag["timedenchantments"] = timedEnchantments;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            primaryElement = tag.Get<Item>("element");
            normalEnchantments = tag.Get<List<Item>>("enchantments");
            timedEnchantments = tag.Get<List<Item>>("timedEnchantments");
        }

        public void RandomizeEnchantments()
        {
            for(int i = 0; i < normalEnchantments.Count; i++)
            {
                var enchantmentsToSpawn = ItemHelper.Enchantments;
                BaseEnchantment enchantmentToSwapTo = enchantmentsToSpawn[Main.rand.Next(0, enchantmentsToSpawn.Length)];
                normalEnchantments[i] = enchantmentToSwapTo.Item;
            }
        }
    }
}
