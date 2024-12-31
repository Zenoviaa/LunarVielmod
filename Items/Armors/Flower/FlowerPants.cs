using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Flower
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class FlowerPants : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lovestruck Thighs");
			/* Tooltip.SetDefault("Sexy!"
				+ "\n10% increased movement speed" +
				"\n+60 Health"); */

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
			player.moveSpeed += 0.3f; // Increase the movement speed of the player
			player.GetDamage(DamageClass.Melee) += 0.10f;
			player.GetDamage(DamageClass.Ranged) += 0.10f;
			player.flowerBoots = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 100);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}