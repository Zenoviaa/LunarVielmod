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
using Stellamod.Projectiles.Swords;

namespace Stellamod.Items.Weapons.Melee
{
	public class DragonWaveFury : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Dragon Wave Fury"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults() 
		{
			Item.damage = 16;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 25;
			Item.height = 25;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = 1;
			Item.knockBack = 8;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.rare = 2;
			Item.UseSound = SoundID.Item117;
			Item.autoReuse = true;
			Item.shoot = ProjectileType<DragonWaveFriendly>();
			Item.shootSpeed = 8f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numberProjectiles = 1 + Main.rand.Next(1
				); // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(3)); // 30 degree spread.
																											   // If you want to randomize the speed to stagger the projectiles
																											   // float scale = 1f - (Main.rand.NextFloat() * .3f);
																											   // perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}