using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class HikersBackpack : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hiker's Backpack");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Drops stumps on the ground as you walk! "); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.sellPrice(silver: 12);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;


		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 3);
			recipe.AddIngredient(ModContent.ItemType<Morrowshroom>(), 3);
			recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 5);
			recipe.AddIngredient(ItemID.Wood, 50);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
			player.GetDamage(DamageClass.Generic) += 0.03f; // Increase ALL player damage by 100%
			player.GetModPlayer<MyPlayer>().HikersBCooldown--;
			player.lifeRegen += 1;


		}




	}
}