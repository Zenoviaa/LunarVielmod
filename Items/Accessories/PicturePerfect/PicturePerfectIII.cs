using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.PicturePerfect
{
    public class PicturePerfectIII : ModItem
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
			Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Cyan;
			Item.accessory = true;


		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 20);
			recipe.AddIngredient(ModContent.ItemType<ArtisanBar>(), 3);
			recipe.AddIngredient(ModContent.ItemType<PicturePerfectII>(), 1);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 100);
			recipe.AddIngredient(ItemID.BeetleHusk, 25);
			recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 500);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{

			Main.LocalPlayer.GetModPlayer<MyPlayer>().Cameraaa = true;
			player.GetModPlayer<MyPlayer>().PPDMG= 20;
			player.GetModPlayer<MyPlayer>().PPDefense = 25;
			player.GetModPlayer<MyPlayer>().PPCrit = 15;
			player.GetModPlayer<MyPlayer>().PPSpeed = 1f;
			player.GetModPlayer<MyPlayer>().PPPaintI = true;
			player.GetModPlayer<MyPlayer>().PPPaintII = true;
			player.GetModPlayer<MyPlayer>().PPPaintIII = true;
			player.GetModPlayer<MyPlayer>().PPPaintDMG = 1.75f;
			player.GetModPlayer<MyPlayer>().PPPaintDMG2 = 50;
			player.GetModPlayer<MyPlayer>().PPPaintTime = 240;
			player.GetModPlayer<MyPlayer>().PPFrameTime = 8;
		}




	}
}