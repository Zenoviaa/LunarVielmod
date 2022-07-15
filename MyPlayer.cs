using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod
{
	public class MyPlayer : ModPlayer
	{
		public bool Bossdeath = false;
		public bool Boots = false;
		public int extraSlots;
		public bool TAuraSpawn;
		public int increasedLifeRegen;
		public int TAuraCooldown = 600;
		public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			TAuraSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;






		}




		

		public override void UpdateDead()
		{
			ResetStats();
		}
		public void ResetStats()
		{
			Bossdeath = false;
			Boots = false;



		}
		
		public static SpriteBatch spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

		public override void PostUpdate()
		{
			//player.extraAccessorySlots = extraAccSlots; dont actually use, it'll fuck things up

			if (Boots)
			{
				if (Player.controlJump)
				{
					const float jumpSpeed = 6.01f;
					if (Player.gravDir == 1)
					{
						Player.velocity.Y -= Player.gravDir * 1f;
						if (Player.velocity.Y <= -jumpSpeed) Player.velocity.Y = -jumpSpeed;
						Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height), Player.width, 0, ModContent.DustType<Dusts.Sparkle>());
					}
					else
					{
						Player.velocity.Y += Player.gravDir * 0.5f;
						if (Player.velocity.Y >= jumpSpeed) Player.velocity.Y = jumpSpeed;
					}
				}
			}

			if (Player.InModBiome<MarrowSurfaceBiome>() && !Main.dayTime)
			{
				 MusicLoader.GetMusicSlot(Mod, "Assets/Music/morrownight");
			}



		}
		public const int CAMO_DELAY = 100;

		internal static bool swingingCheck;
		internal static Item swingingItem;

		public int Shake = 0;

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;

			}






		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{


			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{


				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;

			}






		}
	}


}
