using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Stellamod.Tiles;

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
			Item.value = Item.buyPrice(90);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<HikersBackpack>(), 3);
			recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 5);
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
			player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
			player.GetDamage(DamageClass.Generic) *= 1.1f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().HikersBCooldown--;
			player.GetModPlayer<MyPlayer>().AdvancedBrooches = true;

		}




	}
}