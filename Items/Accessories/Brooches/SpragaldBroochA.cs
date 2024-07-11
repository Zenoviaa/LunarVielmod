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
    public class SpragaldBroochA : ModItem
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

			line = new TooltipLine(Mod, "Brooch of spragalds", Helpers.LangText.Common("SimpleBrooch"))
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
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 3);
			recipe.AddIngredient(ModContent.ItemType<DustedSilk>(), 5);
			recipe.AddIngredient(ItemID.JungleSpores, 10);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
			broochPlayer.KeepBroochAlive<SpragaldBrooch, Spragald>(ref broochPlayer.hasSpragaldBrooch);
			if (!hideVisual)
			{
                Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.GoldCoin, Vector2.Zero);
            }
		}
	}
}