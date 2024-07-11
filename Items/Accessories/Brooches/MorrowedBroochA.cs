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
    public class MorrowedBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of The Huntria Morrow");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\nHeavy increased damage to your arrows" +
				"\n +2 Defense and increased ranged damage" +
				"\n Use the power of the deep dark morrow.."); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Brooch of morrow", Helpers.LangText.Common("SimpleBrooch"))
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);



		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Blue; 
			Item.accessory = true;


		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 30);
			recipe.AddIngredient(ModContent.ItemType<Morrowshroom>(), 10);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddIngredient(ItemID.CopperShortsword, 1);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.KeepBroochAlive<MorrowedBrooch, Morrow>(ref broochPlayer.hasMorrowedBrooch);
		}
	}
}