using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Vanity.Nyxia
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class NyxiaThighs : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astolfo's skirt!");
			/* Tooltip.SetDefault("Woa it even smells like water!"
				+ "\n5% increased movement speed" +
				"\nRizz Thighs"); */

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

		

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}