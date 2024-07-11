using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Verl
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class VerlHat : ModItem
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
			Item.rare = ItemRarityID.Orange; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetDamage(DamageClass.Magic) += 0.07f;
			player.GetCritChance(DamageClass.Generic) += 5f;
			player.statLifeMax2 += 40;
			player.manaRegen += 20;
			player.statManaMax2 += 100;

		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<VerlBreastplate>() && legs.type == ModContent.ItemType<VerlLeggings>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Auto gain the abilities of Magic cuffs and Mana Flower!" + "\nEnemies will be attacked by your power of music!" + "\nEvery few seconds you'll gain a major increase to Magic and Ranged damage!");  // This is the setbonus tooltip
			player.GetModPlayer<MyPlayer>().NotiaB = true;
			player.GetModPlayer<MyPlayer>().NotiaBCooldown++;
			player.magicCuffs = true;
			player.manaFlower = true;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 8);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 10);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 3);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();




		}
	}
}