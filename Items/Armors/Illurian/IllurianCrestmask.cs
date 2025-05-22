using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Illurian
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class IllurianCrestmask : ModItem
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
			Item.defense = 17; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{
            player.statLifeMax2 -= 50;
            player.GetDamage(DamageClass.Summon) *= 1.12f;
			player.GetDamage(DamageClass.Magic) *= 1.12f;
			player.GetCritChance(DamageClass.Generic) += 10f;
			player.autoReuseGlove = true;

		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<IllurianCrestplate>() && legs.type == ModContent.ItemType<IllurianCrestpants>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = LangText.SetBonus(this);//"Enemies are less likely to target you!");  // This is the setbonus tooltip
			player.aggro *= 2;



		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}
}