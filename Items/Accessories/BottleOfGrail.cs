using Microsoft.Xna.Framework;

using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Town;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
	public class BottleOfGrail : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Advanced Brooch Knapsack");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +10% damage" +
				"\n Allows you to equip advanced brooches! (Very useful :P)" +
				"\n Allows the effects of the Hiker's Backpack! "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "ADBPau", "This'll drive you insane for one minion")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<GrailBar>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.vortexMonolithShader = true;
			player.maxMinions += 1;
		}
	}
}