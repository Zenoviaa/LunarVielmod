using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Ores
{
    public class ArtisanBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Frozen to the core, an essense of the moon and ice.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightPurple;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(2);
			recipe.AddIngredient(ModContent.ItemType<KaleidoscopicInk>(), 5);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 1);
			recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 1);
			recipe.AddTile(TileID.AdamantiteForge);
			recipe.Register();
		}
	}
}