using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Buffs.Charms;
using Stellamod.Items.Armors.Lovestruck;
using Stellamod.Items.Armors.Verl;
using Stellamod.Items.Consumables;
using Stellamod.Particles;
using Stellamod.Projectiles;

using System.Collections.Generic;
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
		public bool AdvancedBrooches;
		public bool HikersBSpawn;
		public bool PlantH;
		public bool Dice;
		public bool PlantHL;
		public int increasedLifeRegen;
		public int TAuraCooldown = 600;
		public int HikersBCooldown = 30;
		public int DiceCooldown = 0;
		public bool ArcaneM;
		public bool ThornedBook;
		public int ArcaneMCooldown = 0;
		public bool ZoneMorrow = false;
		public int Timer = 0;
		public bool NotiaB;
		public int NotiaBCooldown = 0;
		public int SwordCombo;
		public int SwordComboR;
		public int lastSelectedI;
		public bool Lovestruck;
		public int LovestruckBCooldown = 0;
		public bool ADisease;
		public bool ZoneFable = false;
		private Vector2 RandomOrig;
		private Vector2 RandomOrig2;
		private Vector2 RandomOrig3;
		public int GoldenRingCooldown = 0;
		public int GoldenSparkleCooldown = 0;
		public int RayCooldown = 0;
		public int VerliaBDCooldown = 5;














		//---------------------------------------------------------------------------------------------------------------
		// Brooches
		public bool BroochSpragald;
		public int SpragaldBCooldown = 1;
		public bool BroochFrile;
		public int FrileBCooldown = 1;
		public int FrileBDCooldown = 1;
		public bool BroochFlyfish;
		public int FlyfishBCooldown = 1;
		public bool BroochMorrow;
		public int MorrowBCooldown = 1;
		public bool BroochSlime;
		public int SlimeBCooldown = 1;
		public bool BroochDiari;
		public int DiariBCooldown = 1;
		public bool BroochVerlia;
		public int VerliaBCooldown = 1;
		public bool BroochAmethyst;
		public int AmethystBCooldown = 1;
		public bool BroochAmber;
		public int AmberBCooldown = 1;
		public bool BroochLonelyBones;
		public int LonelyBonesBCooldown = 1;
		public bool BroochMagesticWood;
		public int MagesticWoodBCooldown = 1;
		public bool BroochFamiliarWood;
		public int FamiliarWoodBCooldown = 1;
		public bool BroochMerchantsCoat;
		public int MerchantsCoatBCooldown = 1;
		public bool BroochMorrowedJellies;
		public int MorrowedJelliesBCooldown = 1;
		public bool BroochAllEye;
		public int AllEyeBCooldown = 1;
		public bool BroochMOS;
		public int MOSBCooldown = 1;
		public bool BroochBonedEye;
		public int BonedEyeBCooldown = 1;
		//---------------------------------------------------------------------------------------------------------------
		public override void ResetEffects()
		{
			// Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
			TAuraSpawn = false;
			HikersBSpawn = false;
			Player.lifeRegen += increasedLifeRegen;
			increasedLifeRegen = 0;
			ArcaneM = false;
			PlantH = false;
			ThornedBook = false;
			Dice = false;
			NotiaB = false;
			Lovestruck = false;
			ADisease = false;


		BroochSpragald = false;
			BroochFrile = false;
			BroochFlyfish = false;
			BroochMorrow = false;
			BroochSlime = false;
			BroochDiari = false;
			BroochVerlia = false;















			if (SwordComboR <= 0)
			{
				SwordCombo = 0;
				SwordComboR = 0;
			}
			else
			{
				SwordComboR--;
			}
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




		public override void PostUpdateMiscEffects()
		{

			bool fable = (Player.ZoneOverworldHeight && ZoneFable);
			Player.ManageSpecialBiomeVisuals("Stellamod:GovheilSky", ZoneFable);



		}



		public Vector2 focusPoint;
		public float focusTransition;
        private Vector2 startPoint;
        public float focusLength;
		public bool shouldFocus;
		public override void ModifyScreenPosition()
		{
			if (this.shouldFocus)
			{
				if (this.focusLength > 0f)
				{
					if (this.focusTransition <= 1f)
					{
						Main.screenPosition = Vector2.SmoothStep(this.startPoint, this.focusPoint, this.focusTransition += 0.05f);
					}
					else
					{
						Main.screenPosition = this.focusPoint;
					}
					this.focusLength -= 0.05f;
				}
				else if (this.focusTransition >= 0f)
				{
					Main.screenPosition = Vector2.SmoothStep(base.Player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), this.focusPoint, this.focusTransition -= 0.05f);
				}
				else
				{
					this.shouldFocus = false;
				}
			}
			if (this.shakeDrama > 0.5f)
			{
				this.shakeDrama *= 0.92f;
				Vector2 shake = new Vector2(Main.rand.NextFloat(this.shakeDrama), Main.rand.NextFloat(this.shakeDrama));
				Main.screenPosition += shake;
			}
		}
		public void FocusOn(Vector2 pos, float length)
		{
			if (base.Player.Center.Distance(pos) < 2000f)
			{
				this.focusPoint = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
				this.focusTransition = 0f;
				this.startPoint = Main.screenPosition;
				this.focusLength = length;
				this.shouldFocus = true;
			}
		}


		public static SpriteBatch spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{

			return (IEnumerable<Item>)(object)new Item[1]
			{
				new Item(ModContent.ItemType<SirestiasStarterBag>(), 1, 0),
	
			};
		}
		public override void PostUpdate()
		{
			//player.extraAccessorySlots = extraAccSlots; dont actually use, it'll fuck things up


			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Brooches
			if (BroochSpragald && SpragaldBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<SpragaldBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Spragald>(), 1000);
				SpragaldBCooldown = 1000;
			}

			if (BroochFrile && FrileBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<FrileBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<IceBrooch>(), 1000);
				FrileBCooldown = 1000;
			}


			if (BroochFlyfish && FlyfishBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<FlyfishBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Flyfish>(), 1000);
				FlyfishBCooldown = 1000;
			}

			if (BroochMorrow && MorrowBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<MorrowedBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Morrow>(), 1000);
				MorrowBCooldown = 1000;
			}


			if (BroochSlime && SlimeBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<SlimeBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Slimee>(), 1000);
				SlimeBCooldown = 1000;
			}

			if (BroochDiari && DiariBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<DiariBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<Diarii>(), 1000);
				DiariBCooldown = 1000;
			}

			if (BroochVerlia && VerliaBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f, ModContent.ProjectileType<VerliaBrooch>(), 0, 1f, Player.whoAmI);

				Player.AddBuff(ModContent.BuffType<VerliaBroo>(), 1000);
				VerliaBCooldown = 1000;
			}


			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~








			if (HikersBSpawn && HikersBCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1.1f, ModContent.ProjectileType<Stump>(), 10, 1f, Player.whoAmI);
				HikersBCooldown = 30;

			}




			if (NotiaB && NotiaBCooldown == 301)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
				for (int j = 0; j < 1; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
					Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, ModContent.ProjectileType<Noti>(), 120, 1f, Player.whoAmI);
				}


			}
			if (NotiaB && NotiaBCooldown > 300)
			{
				Player.GetDamage(DamageClass.Magic) *= 2f;
				Player.GetDamage(DamageClass.Ranged) *= 2f;

			}
			if (NotiaB && NotiaBCooldown == 420)
			{
				NotiaBCooldown = 0;


			}






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








			if (Player.InModBiome<FableBiome>())
			{
				Main.GraveyardVisualIntensity = 0.4f;
				Main.windPhysicsStrength = 50;


				GoldenRingCooldown++;

				GoldenSparkleCooldown++;
				RayCooldown++;



				for (int j = 0; j < 3; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig, speed2 * 3, ParticleManager.NewInstance<FabledParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}


				for (int j = 0; j < 4; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig2, speed * 2, ParticleManager.NewInstance<SparkleTrailParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				for (int j = 0; j < 2; j++)
				{
					RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
					RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
					RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
					ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 0.5f, ParticleManager.NewInstance<FabledParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				if (GoldenRingCooldown > 2)
                {
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig3, speed * 1, ParticleManager.NewInstance<GoldRingParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenRingCooldown = 0;
					}
				}

				if (GoldenSparkleCooldown > 100)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-900f, 900f), (Main.rand.NextFloat(-600f, 600f)));
						RandomOrig2 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1600f, 1600f), (Main.rand.NextFloat(-900f, 900f)));
						RandomOrig = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-1800f, 1800f), (Main.rand.NextFloat(-1200f, 1200f)));

						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);
						ParticleManager.NewParticle(Player.Center - RandomOrig2, speed2 * 3, ParticleManager.NewInstance<GoldSparkleParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));

						GoldenSparkleCooldown = 0;
					}
				}


				


					

					if (RayCooldown > 1000)
					{
						for (int j = 0; j < 1; j++)
						{
							RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-3000f, 3000f), (Main.rand.NextFloat(700f, 700f)));
		

							Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
							Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

							Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, ModContent.ProjectileType<FabledSunray>(), 1, 1f, Player.whoAmI);

							RayCooldown = 0;
						}
					}

				if (RayCooldown == 500)
				{
					for (int j = 0; j < 1; j++)
					{
						RandomOrig3 = new Vector2(Player.width / 2, Player.height / 2) + new Vector2(Main.rand.NextFloat(-3000f, 3000f), (Main.rand.NextFloat(700f, 800f)));


						Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
						Vector2 speed2 = Main.rand.NextVector2Circular(0.1f, 0.1f);

						Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - RandomOrig3, speed2 * 1, ModContent.ProjectileType<FabledColoredSunray>(), 1, 1f, Player.whoAmI);

						
					}
				}

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



		public int Shake = 0;
        private float shakeDrama;

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
		{
			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;

			}

			if (BroochFrile && FrileBDCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<FrileBroochP>(), 4, 1f, Player.whoAmI);
				FrileBDCooldown = 1;

			}

			if (Lovestruck)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);


			}
			if (BroochVerlia && VerliaBDCooldown <= 0)
			{
				
				for (int d = 0; d < 4; d++)
				{
					float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
					float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<VerliaBroochP3>(), 20, 1f, Player.whoAmI);
				}

				VerliaBDCooldown = 220;
			}

		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
		{
			if (Lovestruck && LovestruckBCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);
				LovestruckBCooldown = 30;

			}

			if (Player.HeldItem.DamageType == DamageClass.Ranged && TAuraSpawn && TAuraCooldown <= 0)
			{


				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -4, ProjectileID.SpikyBall, 30, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 4, ProjectileID.SpikyBall, 20, 1f, Player.whoAmI);
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ProjectileID.SpikyBall, 50, 1f, Player.whoAmI);
				TAuraCooldown = 600;

			}


			if (BroochFrile && FrileBDCooldown <= 0)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<FrileBroochP>(), 3, 1f, Player.whoAmI);
				FrileBDCooldown = 3;

			}

			if (BroochVerlia && VerliaBDCooldown <= 0)
			{

				for (int d = 0; d < 4; d++)
				{
					float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
					float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);

					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
					Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<VerliaBroochP3>(), 20, 1f, Player.whoAmI);
				}

				VerliaBDCooldown = 220;
			}

		}


		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			if (ThornedBook)
			{
				npc.SimpleStrikeNPC(hurtInfo.Damage * 7, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
			}

			if (Lovestruck)
			{

				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);

				npc.SimpleStrikeNPC(hurtInfo.Damage * 5, hurtInfo.HitDirection, crit: false, hurtInfo.Knockback);
			
			}


			if (ADisease)
            {
				switch (Main.rand.Next(8))
				{


					case 0:

						npc.AddBuff((BuffID.Poisoned), 120);

						break;
					case 1:

						npc.AddBuff((BuffID.Slow), 120);

						break;
					case 2:

						npc.AddBuff((BuffID.OnFire3), 240);

						break;
					case 3:

						npc.AddBuff((BuffID.OnFire), 120);

						break;
					case 4:

						npc.AddBuff((BuffID.Frostburn2), 240);

						break;
					case 5:

						npc.AddBuff((BuffID.BrainOfConfusionBuff), 240);

						break;

					case 6:

						npc.AddBuff((BuffID.Lovestruck), 240);

						break;
					case 7:

						npc.AddBuff((ModContent.BuffType<Wounded>()), 240);

						break;
				}
			}
		}









		public override bool PreItemCheck()
		{
			if (Player.selectedItem != lastSelectedI)
			{
				SwordComboR = 0;
				SwordCombo = 0;
				lastSelectedI = Player.selectedItem;
			}
			if (SwordComboR > 0)
			{
				SwordComboR--;
				if (SwordComboR == 0)
				{
					SwordCombo = 0;
				}
			}




			return true;
		}

	}
}
