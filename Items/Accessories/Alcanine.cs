using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class Alcanine : ModItem
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
			Item.value = Item.sellPrice(silver: 25);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;


		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CharmofRot>(), 1);
            recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetDamage(DamageClass.Summon) += 0.10f; 
			player.maxMinions += 2;
		}
	}
}