using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Govheil
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class GovheilBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Celestia Moon Breastplate");
			/* Tooltip.SetDefault("Magical essence of an empress!"
				+ "\n+60 max mana and +1 minion slot" +
				"\n+20 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.LightRed; // The rarity of the item
			Item.defense = 10; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.statManaMax2 += 50; // Increase how many mana points the player can have by 20
			player.maxMinions += 1; // Increase how many minions the player can have by one
			player.statLifeMax2 += 10;
			player.GetDamage(DamageClass.Magic) *= 1.06f;
			player.GetDamage(DamageClass.Summon) *= 1.06f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 3);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
		}
	}
}