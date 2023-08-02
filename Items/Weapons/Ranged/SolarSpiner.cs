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
using Stellamod.Tiles;
using Stellamod.Projectiles.Gun;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.HardMode;

namespace Stellamod.Items.Weapons.Ranged
{
	public class SolarSpiner : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Tychine Gun");
			// Tooltip.SetDefault("Chance to shoot sharks dealing three times the normal damage");
		}

		public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.damage = 35;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 6;
			Item.useAnimation = 6;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bullet;
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Bullet;


        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VoidBlaster>(), 1);
            recipe.AddIngredient(ItemID.IllegalGunParts, 1);
            recipe.AddIngredient(ModContent.ItemType<PrimalTech>(), 10);
            recipe.AddIngredient(ModContent.ItemType<SolaiumBar>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numberProjectiles = 1 + Main.rand.Next(1); // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(5)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			if (Main.rand.Next(4) == 1)
			{
                velocity.Y = velocity.Y / 2;
				damage = Item.damage * 2;
                velocity.X = velocity.X / 2;
				SoundEngine.PlaySound(SoundID.Item90, player.position);
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 2, velocity.Y * 2, ProjectileType<SSBolt>(), damage, Item.knockBack, player.whoAmI, -8f, -8f);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}