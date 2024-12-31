using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Winterborn
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class WinterbornBody : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Vextin Robe");
			/* Tooltip.SetDefault("Roam the wonderous sands"
				+
				"\n+40 Health"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Blue; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.statLifeMax2 += 20;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BorealWood, 8);
			recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}