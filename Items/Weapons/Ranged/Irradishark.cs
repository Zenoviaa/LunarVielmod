using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Irradishark : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Irradishark"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 35f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numberProjectiles = 1 + Main.rand.Next(2); // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.X).RotatedByRandom(MathHelper.ToRadians(10)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			if (Main.rand.Next(6) == 1)
			{
				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Item.Center, 512f, 32f);
                velocity.Y = velocity.Y / 2;
				damage = Item.damage * 3;
                velocity.X = velocity.Y / 2;
				SoundEngine.PlaySound(SoundID.Item84, player.position);
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.X, ProjectileType<SharkAcid>(), damage, Item.knockBack, player.whoAmI, -8f, -8f);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Megashark, 1);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
			recipe.AddIngredient(ItemType<TychineGun>(), 1);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
			recipe.AddIngredient(ItemType<VirulentPlating>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

	}
}