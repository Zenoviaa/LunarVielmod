using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Items.Weapons.Mage;
using System;
using System.Collections.Generic;
using System.IO;
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
        public List<Item> equippedEnchantments;
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
            primaryElement.SetDefaults(0);
            equippedEnchantments = new List<Item>();
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
            writer.Write(equippedEnchantments.Count);
            for (int i = 0; i < equippedEnchantments.Count; i++)
            {
                ItemIO.Send(equippedEnchantments[i], writer);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            primaryElement = ItemIO.Receive(reader);
            int length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                equippedEnchantments[i] = ItemIO.Receive(reader);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(player, ref damage);
            float damageModifier = 1f;
            for (int i = 0; i < equippedEnchantments.Count; i++)
            {
                Item item = equippedEnchantments[i];
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
            for (int i = 0; i < equippedEnchantments.Count; i++)
            {
                Item item = equippedEnchantments[i];
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
            staff.equippedEnchantments = new List<Item>();
            for(int i = 0; i < equippedEnchantments.Count; i++)
            {
                staff.equippedEnchantments.Add(equippedEnchantments[i].Clone());
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

            string preferenceString = "MoonPreferences_";
            if(preferences.Count > 0)
            {
                for (int p = 0; p < preferences.Count; p++)
                {
                    int preferenceType = preferences[p];
                    ModItem element = ModContent.GetModItem(preferenceType);
                    preferenceString += $"{element.Texture}_";

                }
                tooltipLine = new TooltipLine(Mod, preferenceString, "Preferences");
                tooltips.Add(tooltipLine);
            }

            if (!IsMatchingPreference())
            {
                tooltipLine = new TooltipLine(Mod, "EnchantHelp",
                  Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentMismatch"));
                tooltipLine.OverrideColor = Color.Red;
                tooltips.Add(tooltipLine);
            }

            for (int i = 0; i < equippedEnchantments.Count; i++)
            {
                var item = equippedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    tooltipLine = new TooltipLine(Mod, $"MoonMagicEnchant_{enchantment.Texture}_{i}", enchantment.DisplayName.Value);
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
            ModContent.GetInstance<MagicUISystem>().OpenUI(Item.Clone().ModItem as AbstractMagicWand);
            Item.SetDefaults(0); 
        }

        public void SetElement(Item item)
        {
            primaryElement = item;
        }

        public Item GetElement()
        {
            return primaryElement;
        }

        public void SetEnchantment(Item item, int index)
        {
            while(equippedEnchantments.Count < index)
            {
                Item air = new Item();
                air.SetDefaults(0);
                equippedEnchantments.Add(air);
            }
            equippedEnchantments[index] = item;

        }

        public Item GetEnchantment(int index)
        {
            if (equippedEnchantments.Count > index)
            {
                Item item = equippedEnchantments[index];
                return item;
            }
            Item airItem2 = new Item();
            airItem2.SetDefaults(0);
            return airItem2;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (primaryElement.ModItem is BaseElement element)
            {
                element.SpecialInventoryDraw(Item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }

            for (int i = 0; i < equippedEnchantments.Count; i++)
            {
                var enchant = equippedEnchantments[i];
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
            tag["enchantments"] = equippedEnchantments;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            primaryElement = tag.Get<Item>("element");
            equippedEnchantments = tag.Get<List<Item>>("enchantments");
        }

        public void RandomizeEnchantments()
        {
            for(int i = 0; i < equippedEnchantments.Count; i++)
            {
                var enchantmentsToSpawn = BaseEnchantment.AllEnchantments;
                BaseEnchantment enchantmentToSwapTo = enchantmentsToSpawn[Main.rand.Next(0, enchantmentsToSpawn.Length)];
                equippedEnchantments[i] = enchantmentToSwapTo.Item;
            }
        }
    }
}
