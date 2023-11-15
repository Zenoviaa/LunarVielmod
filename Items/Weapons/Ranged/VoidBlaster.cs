using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class VoidBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Tychine Gun");
			// Tooltip.SetDefault("Chance to shoot sharks dealing three times the normal damage");
		}

        public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bullet;
			Item.shootSpeed = 15f;
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
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(10)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				//float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}

			if (Main.rand.NextBool(4))
			{
                velocity.Y = velocity.Y / 2;
				damage = Item.damage * 2;
                velocity.X = velocity.X / 2;
				SoundEngine.PlaySound(SoundID.Item91, player.position);
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ProjectileType<GraniteMagmumProj>(), damage, Item.knockBack, player.whoAmI, -8f, -8f);
			}

			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}