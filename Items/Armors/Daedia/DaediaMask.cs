using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Daedia
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class DaediaMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lovestruck Mask");
			/* Tooltip.SetDefault("Magical essence of an Lusting Goddess"
				+ "\n+7% increased damage" +
				"\n+40 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.LightRed; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 65;
			player.hasAngelHalo = true;
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<DaediaBreastplate>() && legs.type == ModContent.ItemType<DaediaThighs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen by decent amount!" + "\nEnemies become lovestruck when you are hit, or when you hit them!" + "\nThis weakens, burns and confuses, slows and does exponential damage " + "\nSpirit balls come from a portal on your armor and attack enemies " + "\nNo fall Damage " + "\nPick up hearts from afar!"); // This is the setbonus tooltip
			player.lifeRegen += 1;
			player.GetModPlayer<MyPlayer>().Lovestruck = true;
			player.GetModPlayer<MyPlayer>().Daedstruck = true;
			player.lifeMagnet = true;
			player.noFallDmg = true;
			player.GetModPlayer<MyPlayer>().LovestruckBCooldown--;
			player.GetModPlayer<MyPlayer>().DaedstruckBCooldown--;

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 6);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 6);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 6);
			recipe.AddIngredient(ItemID.Silk, 10);
			recipe.AddIngredient(ItemID.LifeCrystal, 7);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.PinkThread, 3);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 20);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}