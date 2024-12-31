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
    [AutoloadEquip(EquipType.Legs)]
	public class WitchenPants : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Verl Leggings");
			/* Tooltip.SetDefault("Look at them thighs go!"
				+ "\n+7% Magic and Ranged Damage!" +
				"\n+Increased Movement speed" +
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
			player.moveSpeed += 0.3f;
			player.maxRunSpeed += 0.3f;
			player.statLifeMax2 += 20;
		}

		

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}