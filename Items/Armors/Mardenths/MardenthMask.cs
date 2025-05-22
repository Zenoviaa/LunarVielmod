using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Mardenths
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class MardenthMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astolfo Wig");
			/* Tooltip.SetDefault("Yummy kummy :3"
				+ "\n+2% increased damage" +
				"\n+35 Health" +
				"\nIncreased Pickaxe Speed!"); */

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
			Item.value = Item.sellPrice(gold: 2); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}
		public override void UpdateEquip(Player player)
		{
			player.statLifeMax2 += 10;
			player.GetDamage(DamageClass.Generic) *= 0.9f;
		
			
		
			
		}
		// IsArmorSet determines what armor pieces are needed for the setbonus to take effect
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<MardenthRobe>() && legs.type == ModContent.ItemType<MardenthPants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Increases life regen" + "\nTake 10% Less damage"); // This is the setbonus tooltip
			player.lifeRegen += 1;
            player.endurance += 0.10f;

        }

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	}
}