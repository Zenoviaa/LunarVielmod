using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class DiariBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of the Tale of Diari");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\n+ 4 Defense!" +
				"\nAuto swing capabilities!" +
				"\nFlame walking? Always Fed!" +
				"\n+40 Health and Mana"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Brooch of the Tale of Diari", Helpers.LangText.Common("SimpleBrooch"))
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);

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
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.KeepBroochAlive<DiariBrooch, Diarii>(ref broochPlayer.hasDiariBrooch);
			if (!hideVisual)
			{
                Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)),
					DustID.SolarFlare, Vector2.Zero);
            }
		}
	}
}