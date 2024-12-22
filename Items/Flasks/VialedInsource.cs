using Microsoft.Xna.Framework;

using Stellamod.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class VialedInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Insource", Helpers.LangText.Common("Insource"))
            {
                OverrideColor = new Color(100, 278, 203)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourcePotionSickness = 1200;
            InsourceCannotUseDuration = 1200;
        }

        public override void TriggerEffect(Player player)
        {
            player.AddBuff(ModContent.BuffType<VialedUp>(), 600);
        }
    }
}