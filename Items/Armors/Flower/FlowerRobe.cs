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
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class FlowerRobe : ModItem
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
			Item.defense = 16; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			// Increase how many minions the player can have by one

			player.statLifeMax2 += 60;
			player.GetDamage(DamageClass.Melee) += 0.18f;
			player.GetDamage(DamageClass.Ranged) += 0.18f;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 2);
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 5);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 150);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}