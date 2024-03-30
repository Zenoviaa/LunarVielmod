using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Eldritchian
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class EldritchianLegs : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 22; // Width of the item
			Item.height = 12; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 8; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.runAcceleration *= 1.1f;
			player.moveSpeed += 0.2f;
			player.maxRunSpeed += 0.2f; // Increase the movement speed of the player
			player.GetAttackSpeed(DamageClass.Throwing) += 0.08f;
			player.GetDamage(DamageClass.Throwing) += 0.23f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 15);
			recipe.AddIngredient(ItemID.SoulofFlight, 10);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 8);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}