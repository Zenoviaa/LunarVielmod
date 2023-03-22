
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Ores
{
	public class FrileBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Frozen to the core, an essense of the moon and ice.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
			ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

			
		}
		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
		
			
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileOre>(), 5);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}

	}
}