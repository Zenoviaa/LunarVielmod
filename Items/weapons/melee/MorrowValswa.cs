using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Swords;

using Terraria.Audio;
using Terraria.DataStructures;
using Stellamod.Projectiles;

namespace Stellamod.Items.Weapons.Melee
{
    public class MorrowValswa : ClassSwapItem
	{
		public int dir;
		public override DamageClass AlternateClass => DamageClass.Magic;

		public override void SetClassSwappedDefaults()
		{
			Item.damage = 20;
			Item.mana = 6;
		}
		public override void SetDefaults()
		{
			Item.channel = true;
			Item.damage = 7;
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.crit = 4;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 8;
			Item.useTurn = false;
			Item.value = Terraria.Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.shoot = ModContent.ProjectileType<MVCustomSwingProjectile>();
		}
		public override bool CanUseItem(Player player)
		{
			return base.CanUseItem(player);
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
			int Sound = Main.rand.Next(1, 3);

			float numberProjectiles = 2;
			float rotation = MathHelper.ToRadians(20);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<FabelFire>(), damage, Item.knockBack, player.whoAmI);
			}

			if (dir == -1)
			{
				dir = 1;
			}
			else if (dir == 1)
			{
				dir = -1;
			}
			else
			{
				dir = 1;
			}

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, dir);
			return false; // return false to prevent original projectile from being shot
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 25);
		}
	}
}