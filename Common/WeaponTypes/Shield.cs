using Stellamod.Common.ClassReworkSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public abstract class AbstractShieldProjectile : MeleeShield
    {

    }

    public class ShieldGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isShield;
        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
        {
            if(isShield)
            {
                if(player.GetModPlayer<ClassReworkPlayer>().defaultShield)
                {
                    return base.CanEquipAccessory(item, player, slot, modded);
                }
                return false;
            }
            return base.CanEquipAccessory(item, player, slot, modded);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            base.UpdateAccessory(item, player, hideVisual);
            if (isShield)
            {
              
                player.GetModPlayer<ClassReworkPlayer>().heldShield = item.shoot;
            }
        }
    }

    public class ShieldExpandingTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            ShieldGlobalItem shieldGlobalItem = item.GetGlobalItem<ShieldGlobalItem>();
            if (shieldGlobalItem.isShield)
            {
                TooltipLine line = new TooltipLine(Mod, "ShieldHelp", LangText.Common("ShieldHelp"));
                lines.Add(line);
            }
        }
    }
}
