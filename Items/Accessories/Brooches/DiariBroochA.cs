using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class DiariBroochA : BaseBrooch
	{
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			base.ModifyTooltips(tooltips);
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "Love you and have fun -Sirestias")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "Please check out my game Diari!")
            {
                OverrideColor = new Color(244, 202, 59)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "This mod is in relation to the game")
            {
                OverrideColor = new Color(244, 202, 59)

            };
            tooltips.Add(line);


        }
        public override void SetDefaults()
		{
            base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
            Item.buffType = ModContent.BuffType<Diarii>();
		}

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            if (!HideVisual)
            {
                Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)),
                    DustID.SolarFlare, Vector2.Zero);
            }
        }
	}
}