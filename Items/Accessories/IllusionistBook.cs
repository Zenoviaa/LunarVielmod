using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class IllusionistBook : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Book of Wooden Illusion");
			/* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +3% damage" +
				"\n Increases crit strike change by 5% "); */

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
			recipe.AddIngredient(ItemID.Wood, 50);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 1);
			recipe.AddIngredient(ItemID.Cobweb, 30);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Generic) += 0.03f;
            player.GetCritChance(DamageClass.Generic) += 3f;
            player.lifeRegen += 1;
		}
	}
}