using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Particles;
using System.Threading;
using Terraria.ModLoader.Utilities;
using System.IO;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;
using Stellamod.UI.Systems;
using Terraria.Graphics.Effects;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;

namespace Stellamod.NPCs.Bosses.Verlia
{
	[AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class VerliaB : ModNPC
	{
		public Vector2 FirstStageDestination
		{
			get => new Vector2(NPC.ai[1], NPC.ai[2]);
			set
			{
				NPC.ai[1] = value.X;
				NPC.ai[2] = value.Y;
			}
		}

		// Auto-implemented property, acts exactly like a variable by using a hidden backing field
		public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

		// This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
		// Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
		public bool SpawnedHelpers
		{
			get => NPC.localAI[0] == 1f;
			set => NPC.localAI[0] = value ? 1f : 0f;
		}

		public enum ActionState
		{
			StartVerlia,
			SummonStartup,
			SummonIdle,
			Unsummon,
			HoldUP,
			SwordUP,
			SwordSimple,
			SwordHold,
			TriShot,
			Explode,
			In,
			CutExplode,
			IdleInvis,
			InvisCut,
			
			
			
			
			
			
			
			
			
			Dash,
			Slam,
			Pulse,
			Spin,
			Start,
			WindUp,
			WindUpSp,
			TeleportPulseIn,
			TeleportPulseOut,
			TeleportWindUp,
			TeleportSlam,
			SpinLONG,
			TeleportBIGSlam,
			BIGSlam,
			BIGLand
		}
		// Current state

		public ActionState State = ActionState.StartVerlia;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public int rippleCount = 20;
		public int rippleSize = 20;
		public int rippleSpeed = 25;
		public float distortStrength = 300f;


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Verlia of The Moon");

			Main.npcFrameCount[Type] = 80;

			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,

					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				CustomTexturePath = "Stellamod/NPCs/Bosses/Verlia/VerliaPreview",
				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,

			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(24, 42);
			NPC.damage = 1;
			NPC.defense = 20;
			NPC.lifeMax = 6000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 40);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;
		
		






			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<BossBarTest>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Verlia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman.")
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(attackCounter);
			writer.Write(timeBetweenAttacks);
			writer.WriteVector2(dashDirection);
			writer.Write(dashDistance);

		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			attackCounter = reader.ReadInt32();
			timeBetweenAttacks = reader.ReadInt32();


			dashDirection = reader.ReadVector2();
			dashDistance = reader.ReadSingle();

		}

		int attackCounter;
		int timeBetweenAttacks = 120;
		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
		Vector2 TeleportPos = Vector2.Zero;
		bool boom = false;
		float turnMod = 0f;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.player[NPC.target];

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

			SpriteEffects effects = SpriteEffects.None;

			if (player.Center.X > NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			

			Rectangle rect;
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(140, 0);

			///Animation Stuff for Verlia
			/// 1 - 2 Summon Start
			/// 3 - 7 Summon Idle / Idle
			/// 8 - 11 Summon down
			/// 12 - 19 Hold UP
			/// 20 - 30 Sword UP
			/// 31 - 35 Sword Slash Simple
			/// 36 - 45 Hold Sword
			/// 46 - 67 Barrage 
			/// 68 - 75 Explode
			/// 76 - 80 Appear
			/// 133 width
			/// 92 height


			///Animation Stuff for Veribloom
			/// 1 = Idle
			/// 2 = Blank
			/// 2 - 8 Appear Pulse
			/// 9 - 19 Pulse Buff Att
			/// 20 - 26 Disappear Pulse
			/// 27 - 33 Appear Winding
			/// 34 - 38 Wind Up
			/// 39 - 45 Dash
			/// 46 - 52 Slam Appear
			/// 53 - 58 Slam
			/// 59 - 64 Spin
			/// 80 width
			/// 89 height
			/// 



			switch (State)
			{
				case ActionState.StartVerlia:
					rect = new(0, 1 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.SummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.SummonIdle:
					rect = new Rectangle(0, 3 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.Unsummon:
					rect = new Rectangle(0, 8 * 92, 133, 4 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 4, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HoldUP:
					rect = new Rectangle(0, 12 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.SwordUP:
					rect = new Rectangle(0, 20 * 92, 133, 11 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.SwordSimple:
					rect = new(0, 31 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.SwordHold:
					rect = new(0, 36 * 92, 133, 10 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 10, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.TriShot:
					rect = new(0, 46 * 92, 133, 22 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 22, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Explode:
					rect = new(0, 68 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.CutExplode:
					rect = new(0, 70 * 92, 133, 6 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.In:
					rect = new(0, 76 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.IdleInvis:
					rect = new(0, 74 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.InvisCut:
					rect = new(0, 74 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;



















			}


					return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame
		

        int bee = 220;
		private Vector2 originalHitbox;

		public override void AI()
		{
			NPC.velocity *= 0.97f;
			bee--;

			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.Center, RGB.X, RGB.Y, RGB.Z);
			
			Player player = Main.player[NPC.target];

			NPC.TargetClosest();

			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest();
			}



			if (player.dead)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y -= 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(2);
			}
			switch (State)
			{




				case ActionState.StartVerlia:
					NPC.damage = 0;
					counter++;
					StartVerlia();
					break;

				case ActionState.SummonStartup:
					NPC.damage = 0;
					counter++;
					StartSummonVerlia();
					break;

				case ActionState.SummonIdle:
					NPC.damage = 0;
					counter++;
					IdleSummonVerlia();
					break;

				case ActionState.Unsummon:
					NPC.damage = 0;
					counter++;
					UnSummonVerlia();
					break;

				case ActionState.HoldUP:
					NPC.damage = 0;
					counter++;
					HoldUPVerlia();
					break;

				case ActionState.TriShot:
					NPC.damage = 0;
					counter++;
			
					BarrageVerlia();
					break;

				case ActionState.Explode:
					NPC.damage = 0;
					counter++;
					ExplodeVerlia();
					break;

				case ActionState.CutExplode:
					NPC.damage = 0;
					counter++;
					CutExplodeVerlia();
					break;

				case ActionState.In:
					NPC.damage = 0;
					counter++;
					Verliasinsideme();
					break;

				case ActionState.SwordUP:
					NPC.damage = 0;
					counter++;
			
					SwordUPVerlia();
					break;

				case ActionState.SwordSimple:
					NPC.damage = 0;
					counter++;
					SwordSimpleVerlia();
					break;

				case ActionState.SwordHold:
					NPC.damage = 0;
					counter++;
					SwordHoldVerlia();
					break;

				case ActionState.IdleInvis:
					NPC.damage = 0;
					counter++;
					InvisVerlia();
					break;

				case ActionState.InvisCut:
					NPC.damage = 0;
					counter++;
					InvisCut();
					break;


				/////////////////////////////////////////////////////////////////////////////
				///


				case ActionState.Pulse:
					NPC.damage = 0;
					counter++;
					Pulse();
					break;


				case ActionState.WindUp:
					NPC.damage = 0;
					counter++;
					WindUp();
					break;

				case ActionState.WindUpSp:
					NPC.damage = 0;
					counter++;
					WindUpSp();
					break;

				case ActionState.Spin:
					NPC.damage = 0;
					counter++;
					Spin();
					break;

				case ActionState.SpinLONG:
					NPC.damage = 0;
					counter++;
					SpinLONG();
					break;

				case ActionState.Slam:
					NPC.damage = 0;
					counter++;
					Slam();
					break;

				case ActionState.BIGLand:
					NPC.damage = 0;
					counter++;
					BIGLand();
					break;


				case ActionState.Dash:

					NPC.velocity *= 0.8f;

					counter++;
					Dash();
					break;



				case ActionState.TeleportPulseIn:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					PulseIn();
					break;

				case ActionState.TeleportPulseOut:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					PulseOut();
					break;

				case ActionState.TeleportSlam:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportSlam();
					break;

				case ActionState.TeleportBIGSlam:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportBIGSlam();
					break;

				case ActionState.BIGSlam:
					NPC.damage = 0;

					NPC.velocity *= 2;
					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}
					counter++;
					BIGSlam();
					break;

				case ActionState.TeleportWindUp:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportWindUp();
					break;

				default:
					break;
			}
		}


		private void StartVerlia()
		{
			timer++;
			if (timer == 2)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.SummonStartup;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void StartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
            {
				

				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), (int)(0), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), (int)(0), 0f, 0, 0f, 0f);
			}
			if (timer > 5)
			{
				switch (Main.rand.Next(1))
				{
					case 0:
						if (timer == 6)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), (int)(5), 0f, 0, 0f, 0f);
						}

						if (timer == 20)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 80, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), (int)(5), 0f, 0, 0f, 0f);


						}

						if (timer == 25)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 120, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), (int)(5), 0f, 0, 0f, 0f);

						}

						if (timer == 35)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 140, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), (int)(5), 0f, 0, 0f, 0f);

						}

						if (timer == 45)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 170, NPC.position.Y + speedY - 130 , speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), (int)(5), 0f, 0, 0f, 0f);

						}

						if (timer == 54)
						{
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 200, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), (int)(5), 0f, 0, 0f, 0f);

						}
						break;











				//	case 1:

					//	break;













					//case 2:

					//	break;
				}

			}
				if (timer == 55)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.SummonIdle;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void IdleSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 200)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Unsummon;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void UnSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.HoldUP;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void HoldUPVerlia()
		{
			timer++;
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(1))
					{
						case 0:
							State = ActionState.SwordUP;
							ResetTimers();
							break;
						

					}





				}
                else
                {
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.TriShot;
							ResetTimers();
							break;
						case 1:
							State = ActionState.SwordUP;
							ResetTimers();
							break;

					}
				}
				

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		//if (NPC.life<NPC.lifeMax / 2)
		private void BarrageVerlia()
		{
			timer++;
			if (NPC.life < NPC.lifeMax / 2)
			{

				float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
				if (timer == 25)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), (int)(10), 0f, 0, 0f, 0f);
				}
				
				if (timer == 45)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), (int)(10), 0f, 0, 0f, 0f);
				}
			
				if (timer == 65)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), (int)(10), 0f, 0, 0f, 0f);
				}
				
				
			}

			else
            {
				float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
				if (timer == 15)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot>(), (int)(15), 0f, 0, 0f, 0f);
				}
				
				
				if (timer == 45)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot>(), (int)(15), 0f, 0, 0f, 0f);
				}
				
			}
			
			if (timer == 88)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Explode;
						ResetTimers();
						break;
					

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void ExplodeVerlia()
		{
			timer++;
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.IdleInvis;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void InvisVerlia()
		{
			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-125, -125);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + (int)(distanceY);
				
			}
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.In;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void InvisCut()
		{
			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-30, -30);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + (int)(distanceY);

			}
			if (timer == 8)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.In;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void Verliasinsideme()
		{
			timer++;

			
			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)
				{

					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.HoldUP;
							ResetTimers();
							break;


						case 1:
							State = ActionState.CutExplode;
							ResetTimers();
							break;

					}
				}

                else
                {
					switch (Main.rand.Next(1))
					{
						case 0:
							State = ActionState.SummonStartup;
							ResetTimers();
							break;


					}
				}
			

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}



		private void CutExplodeVerlia()
		{
			timer++;
			if (timer == 22)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.InvisCut;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}



		private void SwordUPVerlia()
		{
			timer++;
			if (timer == 41)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.SwordSimple;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void SwordSimpleVerlia()
		{
			timer++;
			Player player = Main.player[NPC.target];

			float speed = 12f;
			if (timer == 1)
            {
                if (player.Center.X > NPC.Center.X)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashRight>(), (int)(20), 0f, 0, 0f, 0f);
                }
                else
                {
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashLeft>(), (int)(20), 0f, 0, 0f, 0f);
				}
				
				
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 3)
			{
				
				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}
			if (timer == 19)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.SwordHold;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		//npc.whoAmI
		private void SwordHoldVerlia()
		{
			float ai1 = NPC.whoAmI;
			timer++;
			Player player = Main.player[NPC.target];
			float speed = 6f;
			if (timer == 1)
			{
				
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashHold>(), (int)(100), 0f, Main.myPlayer, 0f, ai1);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 30)
			{
				
				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer == 38)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.TriShot;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}





































		private void SpinLONG()
		{
			timer++;
			var entitySource = NPC.GetSource_FromAI();
			if (timer == 3)
			{
				ShakeModSystem.Shake = 3;
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verispin"));

				switch (Main.rand.Next(3))
				{



					case 0:
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY - 1 * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 3, speedY * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY * 1f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 2, speedY - 3 * 1.5f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 1, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY - 2 * 2f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 3, speedY - 1 * 1f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 3, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 1 * 3f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 3, speedY * 2f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 3, speedY - 2 * 1f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"));
						break;

					case 1:

						break;

					case 2:
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa - 1 * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 3, speedYa * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa * 1f, ModContent.ProjectileType<SineButterfly>(), (int)(15), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa * 1f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 2, speedYa - 3 * 1.5f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 1, speedYa - 1, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa - 2 * 2f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 1 * 3, speedYa - 1 * 1f, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 1, speedYa - 3, ProjectileID.DandelionSeed, (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 2, speedYa - 1 * 3f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 3, speedYa * 2f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 1 * 3, speedYa - 2 * 1f, ModContent.ProjectileType<CosButterfly>(), (int)(9), 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"));
						break;

				}
			}



			if (timer == 23)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.TeleportPulseIn;
						break;
					case 1:
						State = ActionState.TeleportPulseIn;
						break;

				}
				ResetTimers();
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
		}

		private void PulseIn()
		{
			timer++;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));
			}
			if (timer == 27)
			{
				State = ActionState.Pulse;

				ResetTimers();
			}

		}
		private void PulseOut()
		{
			timer++;

			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						State = ActionState.TeleportWindUp;
						break;
					case 1:
						State = ActionState.TeleportWindUp;
						break;
					case 2:
						State = ActionState.TeleportWindUp;
						break;
					case 3:

						State = ActionState.TeleportWindUp;
						break;
				}



				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

				ResetTimers();
			}

		}
		private void TeleportSlam()
		{

			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)
			{
				int distanceY = Main.rand.Next(-250, -250);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + (int)(distanceY);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));
			}

			if (timer == 27)
			{
				State = ActionState.Slam;

				ResetTimers();
			}


		}


		private void TeleportBIGSlam()
		{

			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));
				int distanceY = Main.rand.Next(-300, -300);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + (int)(distanceY);

			}

			if (timer == 27)
			{
				State = ActionState.BIGSlam;

				ResetTimers();
			}


		}
		private void TeleportWindUp()
		{
			timer++;


			Player player = Main.player[NPC.target];

			if (timer == 1)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));

				switch (Main.rand.Next(2))
				{
					case 0:
						int distance = Main.rand.Next(20, 20);
						int distanceY = Main.rand.Next(-110, -110);
						NPC.position.X = player.Center.X + (int)(distance);
						NPC.position.Y = player.Center.Y + (int)(distanceY);

						break;


					case 1:
						int distance2 = Main.rand.Next(-120, -120);
						int distanceY2 = Main.rand.Next(-110, -110);
						NPC.position.X = player.Center.X + (int)(distance2);
						NPC.position.Y = player.Center.Y + (int)(distanceY2);

						break;
				}

			}

			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				State = ActionState.WindUp;
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

				ResetTimers();
			}


		}



		private void Dash()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 5)
			{
				NPC.damage = 250;
				int distance = Main.rand.Next(3, 3);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}




			if (timer == 20)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.TeleportWindUp;
							break;
						case 1:
							State = ActionState.TeleportSlam;

							break;

					}
					ResetTimers();
				}

				if (NPC.life > NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							State = ActionState.TeleportWindUp;
							break;
						case 1:
							State = ActionState.TeleportSlam;

							break;

						case 2:
							State = ActionState.TeleportBIGSlam;

							break;
					}
					ResetTimers();
				}
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}


		}



		



		private void Slam()
		{
			timer++;
			if (timer == 3)
			{
				NPC.velocity = new Vector2(NPC.direction * 0, 70f);

			}

			if (timer > 10)
			{

				if (NPC.velocity.Y == 0)
				{

					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
					float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), (int)(20), 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB - 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), (int)(20), 0f, 0, 0f, 0f);



					ShakeModSystem.Shake = 8;

				}
			}
			if (timer == 10)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifall"));
			}


			if (timer == 27)
			{

				State = ActionState.WindUpSp;


				ResetTimers();
			}





		}

		private void BIGLand()
		{
			timer++;

			if (timer == 1)
			{
				ShakeModSystem.Shake = 8;
				float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
				float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX * 0, speedY * 0, ProjectileID.DD2OgreSmash, (int)(0), 0f, 0, 0f, 0f);


			}



			if (timer == 24)
			{
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

				State = ActionState.WindUpSp;


				ResetTimers();
			}





		}
		private void BIGSlam()
		{
			timer++;
			if (timer < 20)
			{
				NPC.velocity = new Vector2(NPC.direction * 0, 70f);

			}

			if (timer == 20)
			{
				NPC.velocity *= 0f;
				if (NPC.velocity.Y == 0)
				{

					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
					float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 2 * 6, speedY, ModContent.ProjectileType<StarBullet>(), (int)(10), 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB - 2 * 6, speedY, ModContent.ProjectileType<StarBullet>(), (int)(10), 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifallstar"));












				}
			}

			if (timer > 25)
			{
				State = ActionState.BIGLand;


				ResetTimers();
			}









		}





		private void Spin()
		{
			timer++;
			var entitySource = NPC.GetSource_FromAI();
			if (timer == 3)
			{
				ShakeModSystem.Shake = 3;
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verispin"));

				switch (Main.rand.Next(4))
				{
					case 0:
						//Summon

						break;


					case 1:
						float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<SmallRock>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 1, speedY - 2 * 1, ModContent.ProjectileType<SmallRock2>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock2>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 80, speedX * 0.1f, speedY - 1 * 1, ModContent.ProjectileType<BigRock>(), (int)(40), 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
						break;


					case 2:
						float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SmallRock>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXBb + 2 * 1, speedYb - 2 * 1, ModContent.ProjectileType<SmallRock2>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 2 * 2, speedYb - 2 * 1, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXBb + 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 1 * 2, speedYb - 2 * 1, ModContent.ProjectileType<Rock2>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 80, speedXb * 0.1f, speedYb - 1 * 1, ModContent.ProjectileType<BigRock>(), (int)(40), 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
						break;

					case 3:
						float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), (int)(5), 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Flowers"));


						break;

				}
			}


			if (timer == 23)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.TeleportPulseIn;
						break;
					case 1:
						State = ActionState.TeleportPulseIn;
						break;

				}
				ResetTimers();
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
		}



		private void WindUp()
		{
			timer++;
			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)

				{
					switch (Main.rand.Next(4))
					{

						case 0:
							State = ActionState.Spin;
							break;
						case 1:
							State = ActionState.Dash;
							break;
						case 2:
							State = ActionState.Dash;
							break;
						case 3:

							State = ActionState.Spin;
							break;

					}
				}
				if (NPC.life > NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(4))
					{

						case 0:
							State = ActionState.SpinLONG;
							break;
						case 1:
							State = ActionState.Dash;
							break;
						case 2:
							State = ActionState.Dash;
							break;
						case 3:

							State = ActionState.SpinLONG;
							break;

					}
				}
				ResetTimers();
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
		}

		private void WindUpSp()
		{
			timer++;
			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(4))
					{
						case 0:
							State = ActionState.Spin;
							break;
						case 1:
							State = ActionState.Spin;
							break;
						case 2:
							State = ActionState.Spin;
							break;
						case 3:

							State = ActionState.Spin;
							break;
					}
				}
				if (NPC.life > NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(4))
					{
						case 0:
							State = ActionState.SpinLONG;
							break;
						case 1:
							State = ActionState.SpinLONG;
							break;
						case 2:
							State = ActionState.SpinLONG;
							break;
						case 3:

							State = ActionState.SpinLONG;
							break;
					}
				}
				ResetTimers();
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
		}

		private void Pulse()
		{
			timer++;
			Player player = Main.player[NPC.target];
			if (timer == 7)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veripulse"));
				switch (Main.rand.Next(4))
				{
					case 0:
						CombatText.NewText(NPC.getRect(), Color.YellowGreen, "Slowness!", true, false);
						player.AddBuff(BuffID.Slow, 180);
						break;


					case 1:
						CombatText.NewText(NPC.getRect(), Color.MistyRose, "Armor Broke!", true, false);
						player.AddBuff(BuffID.BrokenArmor, 120);
						break;


					case 2:
						CombatText.NewText(NPC.getRect(), Color.Coral, "Player Wrath UP!", true, false);
						player.AddBuff(BuffID.Wrath, 300);
						break;


					case 3:

						CombatText.NewText(NPC.getRect(), Color.Purple, "Player Speed UP!", true, false);
						player.AddBuff(BuffID.Swiftness, 600);
						break;
				}

			}

			if (timer == 43)
			{
				State = ActionState.TeleportPulseOut;

				ResetTimers();
			}

		}



		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			//	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




			// ItemDropRule.MasterModeCommonDrop for the relic
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.VeriBossRel>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<VeribossBag>()));
			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			//npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

			// All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

			// Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
			// Boss masks are spawned with 1/7 chance
			//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

			// This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
			// We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
			// which requires these parameters to be defined
			//int itemType = ModContent.ItemType<Gambit>();
			//var parameters = new DropOneByOne.Parameters()
			//{
			//	ChanceNumerator = 1,
			//	ChanceDenominator = 1,
			//	MinimumStackPerChunkBase = 1,
			//	MaximumStackPerChunkBase = 1,
			//	MinimumItemDropsCount = 1,
			//	MaximumItemDropsCount = 3,
			//};

			//notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

			// Finally add the leading rule
			npcLoot.Add(notExpertRule);
		}
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}


		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedVeriBoss, -1);

		}

	}
}
