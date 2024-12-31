using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class CelestiaMoonLegs : ModItem
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
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 2; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.03f; // Increase the movement speed of the player
			player.statLifeMax2 += 20;
		
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 20);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 8);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 1);
			recipe.AddIngredient(ItemID.Wood, 5);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}