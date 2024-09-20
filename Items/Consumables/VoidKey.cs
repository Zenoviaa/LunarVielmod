using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class VoidKey : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("I thank you for your contribution, return to me, I await your arrival " +
				"\n at the top of my palace, we will dance soon <3"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 32;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Orange;
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 10);
			recipe.AddIngredient(ItemID.HellstoneBar, 5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}