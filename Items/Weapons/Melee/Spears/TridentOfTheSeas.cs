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
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Spears;

namespace Stellamod.Items.Weapons.Melee.Spears
{
	public class TridentOfTheSeas : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Trident Of The Former Seas"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.staff[Item.type] = true;
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 28;
			Item.useAnimation = 1;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ProjectileType<TridentOfTheSeasProg>();
			Item.shootSpeed = 10f;
			

		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemType<DarkEssence>(), 30);
			recipe.AddIngredient(ItemType<DreadFoil>(), 16);
			recipe.AddIngredient(ItemID.Spear, 1);
			recipe.AddIngredient(ItemID.Trident, 1);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}
