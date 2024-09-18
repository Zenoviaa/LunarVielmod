using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using System.Collections.Generic;

using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Igniter
{
	public class GrailedExtenderPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Book of Wooden Illusion");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Increases crit strike change by 5% "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");


			line = new TooltipLine(Mod, "ADBPau",  Helpers.LangText.Common("NoStack"))
			{
				OverrideColor = new Color(110, 187, 24)

			};
			tooltips.Add(line);




		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();

			recipe.AddIngredient(ModContent.ItemType<MidfortuneExtenderPowder>(), 1);
			recipe.AddIngredient(ModContent.ItemType<MiracleThread>(), 30);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 50);
			recipe.AddIngredient(ItemID.Ectoplasm, 20);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
		}
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(silver: 25);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;


		}

		

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			player.GetModPlayer<MyPlayer>().IgniterVelocity = 2.5f;
			
		}




	}
}