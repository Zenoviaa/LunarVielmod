using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Huntrian
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class HuntrianBoots : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Huntrian Boots");
			/* Tooltip.SetDefault("Corroded Woods"
				+ "\n+5% Damage!" +
				"\n+10 Health" +
				"\n+7 Critical Strike Chance!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
		
			player.statLifeMax2 -= 10;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
			player.GetCritChance(DamageClass.Generic) += 7f;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 10);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddRecipeGroup(nameof(ItemID.DemoniteBar), 8);
			recipe.AddIngredient(ModContent.ItemType<GintzlMetal>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}