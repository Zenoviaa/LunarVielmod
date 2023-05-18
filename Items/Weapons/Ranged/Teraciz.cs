using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Teraciz : ModItem
	{
		public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
			// DisplayName.SetDefault("Teraciz");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 19;
			Item.knockBack = 0;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<MeatBullet>();
			Item.shootSpeed = 15f;
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

			type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<MeatBullet3>(), ModContent.ProjectileType<MeatBullet2>() });
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.HellstoneBar, 22);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Hlos>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Stick>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 35);
			recipe.AddIngredient(ModContent.ItemType<MorrowRocks>(), 50);
			recipe.AddIngredient(ItemID.RedPaint, 200);
			recipe.AddIngredient(ItemID.BlackPaint, 200);
			recipe.AddIngredient(ItemID.PurplePaint, 200);

			recipe.Register();
		}
	}
}