using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Artisan
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class ArtisanBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Verl Breastplate");
			/* Tooltip.SetDefault("Shines with a blooming moon"
				+ "\n+10% Ranged and Magic Damage!" +
				"\n+12 Penetration" +
				"\n+5 Critical Strike Chance!" +
				"\n+20 Max Life"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.LightPurple; // The rarity of the item
			Item.defense = 20; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetCritChance(DamageClass.Generic) += 5f; 
			player.GetDamage(DamageClass.Generic) *= 1.05f;
			player.statLifeMax2 += 15;
	
			

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 15);
			recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 7);
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 30);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 30);
			recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 9);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);

			recipe.Register();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}