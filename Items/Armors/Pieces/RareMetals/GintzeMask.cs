using Stellamod.Helpers;
using Stellamod.Items.Armors.HeavyMetal;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Pieces.RareMetals
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class GintzeMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Gintzi Mask");
			/* Tooltip.SetDefault("Rare Iron Mask!"
				+ "\n+2 summons!" +
				"\n+50 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
		
			player.statLifeMax2 += 50;
			player.maxMinions += 2;

		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		public override void UpdateArmorSet(Player player)
		{
			player.maxMinions += 1;
			Main.LocalPlayer.GetModPlayer<MyPlayer>().HMArmor = true;
			player.setBonus = LangText.SetBonus(this);//"2 Gintze Guards come to fight for you" + "\n+1 Summons!");  // This is the setbonus tooltip
		}
	}
}