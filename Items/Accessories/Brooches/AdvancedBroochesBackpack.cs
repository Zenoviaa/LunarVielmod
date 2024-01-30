using Microsoft.Xna.Framework;
using Stellamod.Brooches;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class AdvancedBroochesBackpack : ModItem
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
			var line = new TooltipLine(Mod, "", "");


			line = new TooltipLine(Mod, "ADBP", "A+ Accessory!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);




		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<HikersBackpack>(), 3);
			recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 50);
			recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 30);
			recipe.AddIngredient(ItemID.Wood, 50);
			recipe.AddIngredient(ItemID.HellstoneBar, 10);
			recipe.AddIngredient(ItemID.JungleSpores, 10);
			recipe.AddIngredient(ItemID.Vine, 20);
			recipe.AddIngredient(ItemID.IceChest, 3);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 5);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.hasAdvancedBrooches = true;
			player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
			player.GetDamage(DamageClass.Generic) *= 1.04f; // Increase ALL player damage by 100%
		}
	}
}