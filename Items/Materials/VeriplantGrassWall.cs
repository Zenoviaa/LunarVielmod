using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class VeriplantGrassWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("silky walls!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
		}
		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = ModContent.WallType<Tiles.VeriplantWall>();
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(4);
			recipe.AddIngredient(ModContent.ItemType<VeriplantGrassBlock>(), 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}