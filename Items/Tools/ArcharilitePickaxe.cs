using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Tools
{
	public class ArcharilitePickaxe : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Archarilite Pickaxe");
		}

		public override void SetDefaults() 
		{
			Item.damage = 6;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.pick = 30;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}