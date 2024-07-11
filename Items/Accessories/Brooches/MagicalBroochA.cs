using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
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
			line = new TooltipLine(Mod, "Brooch of the TaGoaaa",  Helpers.LangText.Common("AdvancedBrooch"))
			{
				OverrideColor = new Color(254, 128, 10)
			};

			tooltips.Add(line);
			line = new TooltipLine(Mod, "Brooch of the TaGoaaa",  Helpers.LangText.Common("AdvancedBackpack"))
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(gold: 25);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.hasMagicalBrooch = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 100);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 100);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 30);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
			recipe.AddIngredient(ItemID.NaturesGift, 1);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}


		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

			//Check that this item is equipped
			if (player.HasItemEquipped(Item))
			{
				//Check that you have advanced brooches since these don't work without
				if (broochPlayer.hasAdvancedBrooches)
				{
					//Give backglow to show that the effect is active
					DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
				}
				else
				{
					float sizeLimit = 28;
					//Draw the item icon but gray and transparent to show that the effect is not active
					Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
					return false;
				}
			}

			return true;
		}
	}
}