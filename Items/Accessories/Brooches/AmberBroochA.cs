using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
	public class AmberBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of The Spragald");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\nEffect = +10 Defense" +
				"\n Use the power of the Spragald Spiders!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");
			line = new TooltipLine(Mod, "Brooch of Amber", Helpers.LangText.Common("SimpleBrooch"))
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.KeepBroochAlive<AmberBrooch, AmberB>(ref broochPlayer.hasAmberBrooch);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 200);
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ItemID.Amber, 15);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}
	}
}