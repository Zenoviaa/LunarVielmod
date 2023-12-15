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
	public class MagicalBroochA : ModItem
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
			line = new TooltipLine(Mod, "Brooch of the TaGoaaa", "Advanced Brooch!")
			{
				OverrideColor = new Color(254, 128, 10)
			};

			tooltips.Add(line);
			line = new TooltipLine(Mod, "Brooch of the TaGoaaa", "You need an Advanced Brooches Backpack for this!")
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

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			if (broochPlayer.hasAdvancedBrooches)
			{
				broochPlayer.KeepBroochAlive<MagicalBrooch, MagicalBroo>(ref broochPlayer.hasMagicalBrooch);

			}

			player.GetDamage(DamageClass.Magic) *= 1.2f;
	

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 100);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 100);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 30);
			recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
			recipe.AddIngredient(ItemID.NaturesGift, 1);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}
	}
}