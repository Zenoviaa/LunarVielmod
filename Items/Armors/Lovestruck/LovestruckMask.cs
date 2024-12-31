using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Lovestruck
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class LovestruckMask : ModItem
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
			Item.rare = ItemRarityID.Blue; // The rarity of the item
			Item.defense = 1; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 40;
			player.GetDamage(DamageClass.Generic) *= 1.07f;
		}

		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<LovestruckBreastplate>() && legs.type == ModContent.ItemType<LovestruckLegs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen by an enormous amount!" + "\nEnemies become lovestruck when you are hit, or when you hit them!" + "\nThis weakens, burns and confuses, slows and does exponential damage"); // This is the setbonus tooltip
			player.lifeRegen += 4;
			player.GetModPlayer<MyPlayer>().Lovestruck = true;
			player.loveStruck = true;
			player.GetModPlayer<MyPlayer>().LovestruckBCooldown--;

		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 8);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddIngredient(ItemID.LifeCrystal, 2);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}