using Stellamod.Helpers;
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
    [AutoloadEquip(EquipType.Head)]
	public class ArtisanMask : ModItem
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
			Item.rare = ItemRarityID.LightPurple;// The rarity of the item
			Item.defense = 18; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

		
			player.GetCritChance(DamageClass.Generic) += 5f;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
			player.statLifeMax2 += 15;



		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<ArtisanBreastplate>() && legs.type == ModContent.ItemType<ArtisanThighs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"3, 2, 1 .. Smile! Act like you're on stage will ya :p" + "\nEvery little while you'll get a countdown, and when you hear Zero," + "\nyour crit is 100% and damage output is doubled! " + "\nSmall bits of paint left in your tracks." + "\nCrit chance and armor penetration increased by 20!");// This is the setbonus tooltip


			player.GetCritChance(DamageClass.Generic) += 20f + player.GetModPlayer<MyPlayer>().PPCrit;
			player.GetArmorPenetration(DamageClass.Generic) += 20f + player.GetModPlayer<MyPlayer>().PPCrit;
			player.statDefense += 1 + player.GetModPlayer<MyPlayer>().PPDefense;
			player.GetModPlayer<MyPlayer>().ThreeTwoOneSmile = true;
			player.GetModPlayer<MyPlayer>().ThreeTwoOneSmileBCooldown--;
			player.GetModPlayer<MyPlayer>().PaintdropBCooldown--;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 15);
			recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 9);
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 30);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 100);
			recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 9);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);

			recipe.Register();
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}