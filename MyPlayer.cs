using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
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
		public bool PlantH;
		public bool PlantHL;
		public int increasedLifeRegen;
		public int TAuraCooldown = 600;
		public bool ArcaneM;
		public bool ThornedBook;
		public int ArcaneMCooldown = 0;
		public bool ZoneMorrow = false;
		public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			TAuraSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;
			ArcaneM = false;
			PlantH = false;
			ThornedBook = false;




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

			if ( ArcaneM && ArcaneMCooldown == 601)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 7; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Player.Center, speed * 3, ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

				}
				

			}
			if ( ArcaneM && ArcaneMCooldown > 600)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;


			}
			if ( ArcaneM && ArcaneMCooldown == 720)
			{
				ArcaneMCooldown = 0;


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


        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
			if (ThornedBook)
            {
				npc.StrikeNPC(damage * 12, 1, 1, false, false, true);
			}
           
        }
    }


}
