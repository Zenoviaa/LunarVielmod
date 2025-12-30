using Stellamod.Content.Items.Materials;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.WeaponUpgrade
{
    public class WeaponUpgradeGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public float weaponLevel;
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(item, player, ref damage);
            float damageModifier = weaponLevel * 0.15f;
            damage += damageModifier;
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
            return ModContent.ItemType<DragonShard>();
        }
        public int GetUpgradeAmt()
        {

            switch (weaponLevel)
            {
             
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                default:
                    return 5;
            }
        }

        public bool CanUpgrade(Item item, Player player)
        {
            int mat = GetMaterialType();
            int amt = GetUpgradeAmt();
            return player.CountItem(mat) >= amt && item.damage > 0 && weaponLevel < 100;
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
