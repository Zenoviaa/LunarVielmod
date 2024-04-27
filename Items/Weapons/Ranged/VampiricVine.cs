using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Particles;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class VampiricVine : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 84;
			Item.height = 36;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2");
			Item.autoReuse = true;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Arrow;
			Item.UseSound = SoundID.Item5;
			Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numProjectiles = Main.rand.Next(3, 8);
			for (int p = 0; p < numProjectiles; p++)
			{
				// Rotate the velocity randomly by 30 degrees at max.
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}

			if (Main.rand.NextBool(4))
			{
				int numProjectiles2 = Main.rand.Next(2, 5);
				for (int p = 0; p < numProjectiles2; p++)
				{
					// Rotate the velocity randomly by 30 degrees at max.
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);
					Projectile.NewProjectileDirect(source, position, newVelocity, ModContent.ProjectileType<VampiricArrowProj>(), damage, knockback, player.whoAmI);
				}
				
			}

			int count = 32;
			for (int k = 0; k < count; k++)
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);
				ParticleManager.NewParticle(position, newVelocity * 0.5f, ParticleManager.NewInstance<Ink3>(), default(Color), Main.rand.NextFloat(0.2f, 0.8f));
				
			}

			//Dust Burst in Circle at Muzzle
			float degreesPer = 360 / (float)count;
			for (int k = 0; k < count; k++)
			{
				float degrees = k * degreesPer;
				Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
				Vector2 vel = direction * 8;
				ParticleManager.NewParticle(position, vel * 0.5f, ParticleManager.NewInstance<Ink3>(), default(Color), Main.rand.NextFloat(0.2f, 0.8f));
			}

			return true; // return false because we don't want tmodloader to shoot projectile
		}

		
	}
}