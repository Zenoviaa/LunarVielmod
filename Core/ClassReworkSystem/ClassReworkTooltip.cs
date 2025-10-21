using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.ClassReworkSystem
{
    public class ClassReworkTooltip : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            ClassReworkPlayer classReworkPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
            if(classReworkPlayer.damageClass != item.DamageType && 
                classReworkPlayer.playerClass != PlayerClass.Omni && 
                classReworkPlayer.playerClass != PlayerClass.God && item.damage > 0)
            {
                var line = new TooltipLine(Mod, "ClassNerf", LangText.Common("ClassNerf"));
                line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);
            }
        }
    }
}
