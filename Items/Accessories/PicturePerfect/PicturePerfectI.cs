using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class PicturePerfectI : ModItem
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
			Item.value = Item.sellPrice(gold: 10);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;


		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
			recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 2);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 15);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			Main.LocalPlayer.GetModPlayer<MyPlayer>().Cameraaa = true;
			player.GetModPlayer<MyPlayer>().PPDMG = 10;
			player.GetModPlayer<MyPlayer>().PPDefense = 5;
			player.GetModPlayer<MyPlayer>().PPCrit = 5;
			player.GetModPlayer<MyPlayer>().PPSpeed = 0.5f;
			player.GetModPlayer<MyPlayer>().PPPaintI = true;
			player.GetModPlayer<MyPlayer>().PPPaintDMG = 0.5f;
			player.GetModPlayer<MyPlayer>().PPPaintDMG2 = 5;
			player.GetModPlayer<MyPlayer>().PPPaintTime = 60;
			player.GetModPlayer<MyPlayer>().PPFrameTime = 2;
		}




	}
}