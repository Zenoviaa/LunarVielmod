using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Witchen
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class WitchenHat : ModItem
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
			Item.value = Item.buyPrice(gold: 10); // How many coins the item is worth
			Item.rare = ItemRarityID.Pink; // The rarity of the item
			Item.defense = 9; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetDamage(DamageClass.Magic) *= 1.03f;
			player.GetCritChance(DamageClass.Magic) += 10f;
		
			player.statLifeMax2 += 30;



		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<WitchenRobe>() && legs.type == ModContent.ItemType<WitchenPants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Truly a giver of society! The witches respect you :P" + "\nGain the effects of a mana flower, magic cuffs" + "\nExtreme mana regeneration" + "\nMana costs are reduced by 50%" + "\nFlowery Rhythm!");  // This is the setbonus tooltip
			player.manaCost *= 0.5f;
			player.manaRegen += 70;
			player.magicCuffs = true;
			player.manaFlower = true;
			player.flowerBoots = true; 


		}

		
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}