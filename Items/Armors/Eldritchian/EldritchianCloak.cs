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
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class EldritchianCloak : ModItem
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
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 12; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.lifeRegen += 3;
			player.GetAttackSpeed(DamageClass.Throwing) += 0.12f;
			player.GetDamage(DamageClass.Throwing) += 0.16f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 25);
			recipe.AddIngredient(ItemID.SoulofMight, 10);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 8);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}