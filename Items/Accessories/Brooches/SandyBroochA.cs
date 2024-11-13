using Microsoft.Xna.Framework;

using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class SandyBroochA : BaseBrooch
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
			Item.buffType = ModContent.BuffType<SandyB>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BlankBrooch>(), 1);
			recipe.AddIngredient(ItemID.AntlionMandible, 5);
			recipe.AddIngredient(ItemID.Cactus, 10);
			recipe.AddIngredient(ModContent.ItemType<WanderingFlame>(), 5);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}
	}
}