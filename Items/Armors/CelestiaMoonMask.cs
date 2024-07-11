using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class CelestiaMoonMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Celestia Moon Mask");
			/* Tooltip.SetDefault("Magical essence of an empress!"
				+ "\n+20 Max mana and +1 Minion Slot"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

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
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 2; // The amount of defense the item will give when equipped
		}
		public override void UpdateEquip(Player player)
		{
			player.statManaMax2 += 20; // Increase how many mana points the player can have by 20
			player.maxMinions++; // Increase how many minions the player can have by one
			
		}
		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<CelestiaMoonBreastplate>() && legs.type == ModContent.ItemType<CelestiaMoonLegs>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen by a great amount!" + "\nMove faster and gain the effects of magic cuffs!"); // This is the setbonus tooltip
			player.moveSpeed += 0.3f;
			player.maxRunSpeed += 0.3f;
			player.magicCuffs = true;
			player.lifeRegen += 1;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 30);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 1);
			recipe.AddIngredient(ItemID.Wood, 5);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}