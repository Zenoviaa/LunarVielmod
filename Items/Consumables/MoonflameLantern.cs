using Stellamod.Helpers;
using Stellamod.Items.Materials;
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
			/* Tooltip.SetDefault("I thank you for your contribution, return to me, I await your arrival " +
				"\n at the top of my palace, we will dance soon <3"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			

			
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
			Item.rare = ModContent.RarityType<DefaultSpecialRarity>();
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 1);
            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 10);
			recipe.AddIngredient(ItemID.ObsidianRose, 1);
			recipe.AddIngredient(ItemID.HellstoneBar, 5);
			recipe.AddIngredient(ModContent.ItemType<EmptyMoonflameLantern>(), 1);
			recipe.AddIngredient(ModContent.ItemType<VoidLantern>(), 1);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}

	}
}