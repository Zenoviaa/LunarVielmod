using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Stone
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class StoniaChestplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			// DisplayName.SetDefault("Stonia Stoneplate");
			/* Tooltip.SetDefault("Weak Stone?"
				+ "\n+1% Damage!" +
				"\n+2 Penetration" +
				"\n+2 Critical Strike Chance!"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(silver: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Blue; // The rarity of the item
			Item.defense = 1; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
		{

			player.GetArmorPenetration(DamageClass.Generic) += 2f; ;
			player.GetDamage(DamageClass.Generic) += 0.01f;
			player.GetCritChance(DamageClass.Generic) += 2f;

		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 6);
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();




		}
	}
}