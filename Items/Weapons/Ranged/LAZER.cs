using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Ranged
{
	public class LAZER : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("L A Z E R"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VoidBlaster>(), 1);
            recipe.AddIngredient(ItemID.IllegalGunParts, 1);
            recipe.AddIngredient(ItemID.HallowedBar, 15);
            recipe.AddIngredient(ItemType<RangerDrive>(), 1);
            recipe.AddIngredient(ItemType<UnknownCircuitry>(), 15);
            recipe.AddIngredient(ItemType<DeathShot>(), 1);
			recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 4;
			Item.useAnimation = 4;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = true;
			Item.shoot = ProjectileType<DeathShotProj>();
			Item.shootSpeed = 15f;
			Item.mana = 5;

		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numberProjectiles = 1 + Main.rand.Next(1); // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(10)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
																												// float scale = 1f - (Main.rand.NextFloat() * .3f);
																												// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}