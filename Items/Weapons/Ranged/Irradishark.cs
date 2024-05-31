using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Irradishark : ModItem
	{
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 35f;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
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
				Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				// float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, Item.knockBack, player.whoAmI);
			}

			if (Main.rand.NextBool(6))
			{
				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Item.Center, 512f, 12f);
				velocity = velocity / 2;
				damage = damage * 3;
				SoundEngine.PlaySound(SoundID.Item84, player.position);
				Projectile.NewProjectile(source, position, velocity, ProjectileType<SharkAcid>(), damage, Item.knockBack, player.whoAmI, -8f, -8f);
			}

			return false; // return false because we don't want tmodloader to shoot projectile
		}

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Megashark);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 5);
			recipe.AddIngredient(ModContent.ItemType<TychineGun>());
			recipe.AddIngredient(ModContent.ItemType<BasicGunParts>());
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 25);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
        }
    }
}