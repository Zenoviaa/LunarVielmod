
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
	public class MoonflameLantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("I thank you for your contribution, return to me, I await your arrival " +
				"\n at the top of my palace, we will dance soon <3");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

			
		}
		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
		
		
			
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 40);
			recipe.AddIngredient(ItemID.ObsidianRose, 1);
			recipe.AddIngredient(ItemID.HellstoneBar, 5);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}

	}
}