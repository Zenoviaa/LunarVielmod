using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Helpers;
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
	public class StoningFlyBroochA : ModItem
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
			line = new TooltipLine(Mod, "Brooch of the HV",  Helpers.LangText.Common("RadiantBrooch"))
			{
				OverrideColor = new Color(220, 252, 255)
			};

			tooltips.Add(line);
			line = new TooltipLine(Mod, "Brooch of the Radiant",  Helpers.LangText.Common("RadiantBackpack"))
			{
				OverrideColor = new Color(177, 255, 117)

			};
			tooltips.Add(line);
		}

		public override void SetDefaults()
		{
			Item.width = 49;
			Item.height = 34;
			Item.value = Item.sellPrice(gold: 15);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.hasStonefly = true;
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Player player = Main.player[Main.myPlayer];
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

			//Check that this item is equipped
			if (player.HasItemEquipped(Item))
			{
				//Check that you have advanced brooches since these don't work without
				if (broochPlayer.hasRadiantBrooches)
				{
					//Give backglow to show that the effect is active
					DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(247, 209, 92));
				}
				else
				{
					float sizeLimit = 49;
					//Draw the item icon but gray and transparent to show that the effect is not active
					Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
					return false;
				}
			}

			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
			return true;
		}
		public override void AddRecipes()
		{

			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<StoniaBroochA>(), 1);
			recipe.AddIngredient(ModContent.ItemType<FlyingFishBroochA>(), 1);
			recipe.AddIngredient(ModContent.ItemType<SlimeBroochA>(), 1);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 15);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}


	}


}