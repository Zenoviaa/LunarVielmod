using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Daedia
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class DaediaBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Lovestruck Breastplate");
			/* Tooltip.SetDefault("Feel the love!"
				+ "\n+60 Health and +1 minion slot"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.LightRed; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.maxMinions += 2 ; // Increase how many minions the player can have by one
			player.statLifeMax2 += 60;
			
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 12);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 6);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 12);
			recipe.AddIngredient(ItemID.Silk, 10);
			recipe.AddIngredient(ItemID.LifeCrystal, 12);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.PinkThread, 12);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 20);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}