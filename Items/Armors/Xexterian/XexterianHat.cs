using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Xexterian
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class XexterianHat : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Vextin Mask");
			/* Tooltip.SetDefault("You feel the hidden sands flow in you"
				+ "\n+7% increased damage" +
				"\n+20 Health"); */

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
			Item.defense = 6; // The amount of defense the item will give when equipped
		}
		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 20;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
		
			
		}
		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<XexterianHeatplate>() && legs.type == ModContent.ItemType<XexterianLeggings>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Gain the accessory effects of desert boots!" + "\nAutomatically run at fast speeds without boots! " + "\n +5 Defense!"+ "\nGain the effects if Forbidden armor!" + "\nArrows are heated up with the effects of a molten quiver!"); // This is the setbonus tooltip
			
			player.maxRunSpeed += 0.5f;
			player.statDefense += 5;
			player.moveSpeed += 0.3f;
		
			player.armorEffectDrawOutlinesForbidden = true;
			player.desertBoots = true;
			player.setForbidden = true;
			player.hasMoltenQuiver = true;
			
		
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 20);
			recipe.AddIngredient(ItemID.SandBlock, 55);
			recipe.AddIngredient(ItemID.AntlionMandible, 2);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 12);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}