using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Alsis
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class AlsisMask : ModItem
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
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 14; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetDamage(DamageClass.Magic) *= 0.95f;
			player.GetCritChance(DamageClass.Magic) += 1f;
		
			player.statLifeMax2 -= 10;



		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<AlsisChestplate>() && legs.type == ModContent.ItemType<AlsisGreaves>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"You are a true mage..." + "\nGain the effects of a mana flower, magic cuffs, and retrieve magic stars from afar " + "\nMana costs are reduced by 70% and increased magic regeneration");  // This is the setbonus tooltip
			player.manaCost *= 0.3f;
			player.manaRegen += 1;
			player.magicCuffs = true;
			player.manaFlower = true;
			player.manaMagnet = true;


			player.GetModPlayer<MyPlayer>().MasteryMagic = true;
			player.GetModPlayer<MyPlayer>().MasteryMagicBCooldown--;

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 15);
			recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 30);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 9);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
			recipe.Register();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}