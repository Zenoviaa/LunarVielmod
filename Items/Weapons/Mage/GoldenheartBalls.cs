﻿using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Quest.Merena;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
	public class GoldenHeartBalls : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noUseGraphic = true;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.DD2_DarkMageAttack;
			Item.autoReuse = true;
			Item.shoot = ProjectileType<GoldenHBalls>();
			Item.shootSpeed = 8f;
			Item.mana = 5;


		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 60);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 15);
			recipe.AddIngredient(ModContent.ItemType<OrbOfTheMorrow>(), 1);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}