using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
	internal class LanternOfAFallen : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pearlescent Ice Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of light!");
		}
		public override void SetDefaults()
		{
			Item.damage = 62;
			Item.mana = 30;
			Item.width = 29;
			Item.height = 31;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.scale = 0.5f;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_DarkMageSummonSkeleton;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.ChlorophyteBullet;
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 22;
			
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			
			Projectile.NewProjectile(source, position, velocity, ProjectileID.ChlorophyteBullet, damage, knockback, player.whoAmI);
			float numberProjectiles = 3;
			float rotation = MathHelper.ToRadians(15);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.ChlorophyteBullet, damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<GreekLantern>(), 1);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 5);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 20);
		}
	}
}









