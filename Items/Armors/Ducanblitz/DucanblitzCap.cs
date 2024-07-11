using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Ducanblitz
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class DucanblitzCap : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Verl Hat");
			/* Tooltip.SetDefault("You feel prettier dont you?"
				+ "\n+7% Magic Damage!" +
				"\n+Fast mana regeneration!" +
				"\n+100 Max Mana" +
				"\n+40 Max Life"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.LightRed; // The rarity of the item
			Item.defense = 20; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetDamage(DamageClass.Melee) *= 1.12f;
			player.GetCritChance(DamageClass.Generic) += 10f;
			player.autoReuseGlove = true;

		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<DucanblitzBreastplate>() && legs.type == ModContent.ItemType<DucanblitzThighs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Enemies are less likely to target you!" + "\nBlades of Cinder and Rhamenthal will fight alongside you" + "\nEvery few seconds you'll gain a major increase to Melee Damage" + "\nAn upgraded Govheil set essentially.");  // This is the setbonus tooltip
			player.GetModPlayer<MyPlayer>().DucanB = true;
			player.GetModPlayer<MyPlayer>().DucanBCooldown++;
			player.aggro *= 2;
			player.hasPaladinShield = true;


		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 30);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 10);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 8);
			recipe.AddIngredient(ItemID.Ectoplasm, 8);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}