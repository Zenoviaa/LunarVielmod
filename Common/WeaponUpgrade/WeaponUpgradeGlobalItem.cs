using Stellamod.Common.ArmorReforge;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.WeaponUpgrade
{
    internal class WeaponUpgradeGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float weaponLevel;
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(item, player, ref damage);
            float damageModifier = MathF.Pow(1.05f, weaponLevel);
            damage += damageModifier - 1.0f;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            writer.Write(weaponLevel);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            weaponLevel = reader.ReadSingle();
        }

        public int GetMaterialType()
        {
            int lunarStone = ModContent.ItemType<LunarStone>();
            int rareLunarStone = ModContent.ItemType<RareLunarStone>();
            int ancientLunarStone = ModContent.ItemType<AncientLunarStone>();
            if (weaponLevel < 5)
            {
                return lunarStone;
            } else if (weaponLevel < 10)
            {
                return rareLunarStone;
            } 
            else if (weaponLevel < 15)
            {
                return ancientLunarStone;
            }

            return ancientLunarStone;
        }
        public int GetUpgradeAmt()
        {

            switch (weaponLevel)
            {
                default:
                case 0:
                    return 5;
                case 1:
                    return 10;
                case 2:
                    return 20;
                case 3:
                    return 50;
                case 4:
                    return 100;
                case 5:
                    return 3;
                case 6:
                    return 5;
                case 7:
                    return 10;
                case 8:
                    return 15;
                case 9:
                    return 20;
                case 10:
                    return 1;
                case 11:
                    return 2;
                case 12:
                    return 3;
                case 13:
                    return 4;
                case 14:
                    return 5;
            }
        }
        
        public bool CanUpgrade(Item item, Player player)
        {
            int mat = GetMaterialType();
            int amt = GetUpgradeAmt();
            return player.CountItem(mat) >= amt && item.damage > 0;
        }

        public void Upgrade(Item item, Player player)
        {
            int mat = GetMaterialType();
            int amt = GetUpgradeAmt();
            player.RemoveItem(mat, amt);
            weaponLevel += 1;
            item.NetStateChanged();
        }
    
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (weaponLevel < 1)
                return;

            TooltipLine itemNameLine = tooltips.Find(x => x.Name == "ItemName");
            itemNameLine.Text = itemNameLine.Text + " " + $"+{weaponLevel}";
            if(weaponLevel >= 15)
            {
                item.rare = ModContent.RarityType<GoldenSpecialRarity>();
            }
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            base.SaveData(item, tag);
            tag["weaponLevel"] = (int)weaponLevel;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            base.LoadData(item, tag);
            weaponLevel = tag.Get<int>("weaponLevel");
        }
    }
}
