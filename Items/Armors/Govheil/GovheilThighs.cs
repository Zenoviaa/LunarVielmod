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
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class GovheilThighs : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Celestia Moon Leggings");
			/* Tooltip.SetDefault("Magical essence of an empress!"
				+ "\n5% increased movement speed" +
				"\n+20 Health"); */

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
			player.moveSpeed *= 1.07f; // Increase the movement speed of the player
			player.statLifeMax2 += 10;
			player.GetArmorPenetration(DamageClass.Generic) += 15f;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 12);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 3);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}