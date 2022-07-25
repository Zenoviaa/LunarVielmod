using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Consumables;
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
		public bool Dice;
		public bool PlantHL;
		public int increasedLifeRegen;
		public int TAuraCooldown = 600;
		public int DiceCooldown = 0;
		public bool ArcaneM;
		public bool ThornedBook;
		public int ArcaneMCooldown = 0;
		public bool ZoneMorrow = false;
		public int Timer = 0;
		public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			TAuraSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;
			ArcaneM = false;
			PlantH = false;
			ThornedBook = false;
			Dice = false;




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

			if (ArcaneM && ArcaneMCooldown == 601)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 7; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Player.Center, speed * 3, ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

				}


			}
			if (ArcaneM && ArcaneMCooldown > 600)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;


			}
			if (ArcaneM && ArcaneMCooldown == 720)
			{
				ArcaneMCooldown = 0;


			}

			if (Dice)
			{
				Timer++;
				if (Timer == 90 || DiceCooldown == 90)
				{
					Player player = Player;
					var entitySource = player.GetSource_FromThis();
		
					switch (Main.rand.Next(5))
					{

						case 0:


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Wohooo", true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 1));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 1:


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Omg, its something!", true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 2));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 2:


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Are you disappointed? You should be.", true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(0, 1));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

						case 3:


							CombatText.NewText(player.getRect(), Color.YellowGreen, "Wow, you have no maidens and no luck..", true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();

									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));

									Dice = false;
									break;

								}
							}
							break;


						case 4:

							CombatText.NewText(player.getRect(), Color.YellowGreen, "Sooo lucky!", true, false);
							for (int i = 0; i < player.inventory.Length; i++)

							{

								if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

								{
									Item item = new Item();
									player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(2, 2));
									player.inventory[i].TurnToAir();
									player.inventory[i] = item;
									SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


									Dice = false;
									break;

								}
							}
							break;

					}
					Timer = 0;

				}

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
				npc.StrikeNPC(damage * 6, 1, 1, false, false, true);
			}
           
        }
    }


}
