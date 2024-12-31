using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Garbage
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Body)]
	public class GarbageChestplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 34; // Width of the item
			Item.height = 24; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 17; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 40;
			player.lifeRegen += 1;
			player.GetAttackSpeed(DamageClass.Throwing) += 0.10f;
			player.GetDamage(DamageClass.Throwing) += 0.25f;
			player.GetDamage(DamageClass.Summon) += 0.25f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RadianuiBar>(), 15);
			recipe.AddIngredient(ModContent.ItemType<ArmorDrive>(), 10);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}