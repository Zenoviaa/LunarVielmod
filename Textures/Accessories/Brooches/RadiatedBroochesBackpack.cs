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
	[AutoloadEquip(EquipType.Waist)]
	public class RadiatedBroochesBackpack : ModItem
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


			line = new TooltipLine(Mod, "RADBBP", "S+ Accessory!")
			{
				OverrideColor = new Color(220, 87, 24)

			};
			tooltips.Add(line);




		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 25);
			Item.rare = ItemRarityID.Orange;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AdvancedBroochesBackpack>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 30);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 30);
			recipe.AddIngredient(ItemID.Wood, 100);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
			recipe.AddIngredient(ItemID.Seashell, 10);
			recipe.AddIngredient(ItemID.Feather, 5);
			recipe.AddIngredient(ItemID.SoulofMight, 20);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.hasAdvancedBrooches = true;
			broochPlayer.hasRadiantBrooches = true;
			player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
			player.GetDamage(DamageClass.Generic) *= 1.08f; // Increase ALL player damage by 100%
		}
	}
}