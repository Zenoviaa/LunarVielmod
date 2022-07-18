
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

namespace Stellamod.NPCs.Bosses.StarrVeriplant
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    public class StarrVeriplant : ModNPC
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
			TeleportSlam

		}
		// Current state

		public ActionState State = ActionState.Start;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starr Veriplant");
			
			Main.npcFrameCount[Type] = 64;

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
				CustomTexturePath = "Stellamod/NPCs/Bosses/StarrVeriplant/StarrPreview",
				PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 89;
			NPC.damage = 420;
			NPC.defense = 1;
			NPC.lifeMax = 8000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
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
			NPC.BossBar = ModContent.GetInstance<VeriBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/StarrVeriplant");
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The beloved plant, a wonder across the stars, infected by the gild and serves as a protector to the Morrow and Veribloom.")
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

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {









;
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;

			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)






		

			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Rectangle rect;

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
			/// 72 width
			/// 84 height

			switch (State)
			{
				case ActionState.Start:
					rect = new(0, 1 * 89, 80, 1 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.TeleportPulseIn:
					rect = new Rectangle(0, 2 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.TeleportWindUp:
					rect = new Rectangle(0, 27 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				

				case ActionState.TeleportSlam:
					rect = new Rectangle(0, 46 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.TeleportPulseOut:
					rect = new(0, 20 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.Dash:
					rect = new(0, 39 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.Slam:
					rect = new(0, 53 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.WindUp:
					rect = new(0, 34 * 89, 80, 5 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.WindUpSp:
					rect = new(0, 34 * 89, 80, 5 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Spin:
					rect = new(0, 59 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Pulse:
					rect = new(0, 9 * 89, 80, 11 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;



				

			}

			
			

			return false;


		}
		
		public override void AI()
		{

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
				direction.Normalize();
				
				


			}
			NPC.TargetClosest();

			switch (State)
			{
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
				
				
				case ActionState.Slam:
					NPC.damage = 0;
					counter++;
					Slam();
					break;

				case ActionState.Start:
					NPC.damage = 0;
					counter++;
					Start();
					break;

				case ActionState.Dash:
					NPC.damage = 250;
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

        private void PulseIn()
        {
			timer++;

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
						NPC.position.X = player.Center.X ;
						NPC.position.Y = player.Center.Y + (int)(distanceY);
				
				}

						if (timer == 27)
			{
				State = ActionState.Slam;

				ResetTimers();
			}

			
		}



		private void TeleportWindUp()
		{
			timer++;


			Player player = Main.player[NPC.target];

			if (timer == 1) { 
				switch (Main.rand.Next(2))
				{
					case 0:
						int distance = Main.rand.Next(100, 100);
						int distanceY = Main.rand.Next(-170, -170);
						NPC.position.X = player.Center.X + (int)(distance);
						NPC.position.Y = player.Center.Y + (int)(distanceY);

						break;


					case 1:
						int distance2 = Main.rand.Next(-100, -100);
						int distanceY2 = Main.rand.Next(-170, -170);
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
			
			if (timer < 5)
            {
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
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

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
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
				

			}

			
        }



        private void Start()
        {
			timer++;
			if (timer == 20)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.TeleportSlam;
						ResetTimers();
						break;
					case 1:
						State = ActionState.TeleportSlam;
						break;
				
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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedX + 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), (int)(20), 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedXB - 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), (int)(20), 0f, 0, 0f, 0f);

					

				}
				if (NPC.velocity.Y == 0)
				{
					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
					float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					ShakeModSystem.Shake = 8;
				}
			}
			

			if (timer == 27)
				{
					State = ActionState.WindUpSp;
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


				switch (Main.rand.Next(5))
				{
					case 0:
						//Summon

						break;


					case 1:
						float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedX -2 * 2, speedY - 2 * 2, ModContent.ProjectileType<SmallRock>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedXB + 2 * 1, speedY - 2 * 1 , ModContent.ProjectileType<SmallRock2>(), (int)(10), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedX - 2 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedXB + 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Rock>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedX - 1 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock2>(), (int)(20), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 170, speedX * 0.1f, speedY - 1 * 1, ModContent.ProjectileType<BigRock>(), (int)(40), 0f, 0, 0f, 0f);

						break;


					case 2:
						//Petal fall

						break;


					case 3:
						float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 170, speedXa - 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), (int)(5), 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 170, speedXBa + 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), (int)(5), 0f, 0, 0f, 0f);


				
						break;
					case 4:
				

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




	}
}
