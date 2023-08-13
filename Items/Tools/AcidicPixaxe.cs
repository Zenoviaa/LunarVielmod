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
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Tools
{
	public class AcidicPixaxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IrradiPixaxe"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
			Item.pick = 80;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Wood, 8);
			recipe.AddIngredient(ItemType<VirulentPlating>(), 14);
			recipe.AddIngredient(ItemType<LostScrap>(), 14);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

	}
}