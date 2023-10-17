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
using Stellamod.NPCs.Bosses.Verlia.Projectiles.Sword;
using Stellamod.NPCs.Projectiles;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.WorldG;
using Stellamod.NPCs.Bosses.Daedus;
using Stellamod.Buffs;
namespace Stellamod.NPCs.Bosses.GothiviaNRek.Reks
{

	public class Train2 : ModNPC
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


			Idle,
			StartGothivia,
			StartRollLeft,
			RollLeft,
			StartRollRight,
			RollRight,
			PunchingFirstPhaseLaserBomb,
			Jump,
			Fall,
			JumpToMiddle,
			PunchingSecondPhaseFlameBalls,
			PunchingSecondPhaseStopSign,
			Land,
			FallToMiddle,
			LandToMiddle,



			Fallslowly,
			Fallslowly2,
			Dashright,
			Dashright2,
			Dashleft,
			Dashleft2,
			Riseslowly,
			Across,
			StopRight,
			StopLeft,
			Acrossfinish,




		}
		// Current state

		public ActionState State = ActionState.Fallslowly;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;


		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia of The Moon");

			Main.npcFrameCount[Type] = 1;

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
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{

				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,

			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(45, 45);
			NPC.damage = 0;
			NPC.defense = 9999;
			NPC.lifeMax = 99999;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.dontTakeDamageFromHostiles = true;
			NPC.noTileCollide = true;
			NPC.scale = 2.5f;
			NPC.friendly = true;








			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar

			// The following code assigns a music track to the boss in a simple way.

		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A part of gothivia's largest creations")
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
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 20; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
			}

		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.player[NPC.target];

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

			SpriteEffects effects = SpriteEffects.None;




			Rectangle rect;
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(0, -20);

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

			/// 1 = Idle
			/// 1 - 4 Jump Startup
			/// 5 - 8 Jump
			/// 9 - 12 land
			/// 13 - 29 Doublestart
			/// 30 - 42 Tiptoe






			return true;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame

		float HomeY = 330f;
		int bee = 220;
		private Vector2 originalHitbox;
		int moveSpeed = 0;
		int moveSpeedY = 0;
		int Timer2 = 0;

		public override void AI()
		{

			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);


			NPC.HasBuff<Rekin>();

			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC ba = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				if (!ba.active && ba.type == ModContent.NPCType<Rek>())
				{



					#region Active check
					// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not

					#endregion
					
						NPC.Kill();
					

				}
			}

					if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);


			Player player = Main.player[NPC.target];

			NPC.TargetClosest();

			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest();
			}



			if (player.dead)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y += 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(2);
			}


			//	if (player.dead)
			//	{
			// If the targeted player is dead, flee
			//		NPC.velocity.Y -= 0.5f;
			//		NPC.noTileCollide = true;
			//		NPC.noGravity = false;
			// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
			//		NPC.EncourageDespawn(2);
			//	}
			switch (State)
			{
				case ActionState.Fallslowly:
					NPC.damage = 0;
					counter++;
					NPC.velocity.Y *= 0.7f;
					Fallslowly();
					NPC.noTileCollide = true;
					NPC.aiStyle = -1;
					break;

				case ActionState.Fallslowly2:
					NPC.damage = 0;
					counter++;
					NPC.velocity.Y *= 0.93f;
					NPC.noTileCollide = true;
					Fallslowly2();
					NPC.aiStyle = -1;
					break;


				case ActionState.Riseslowly:
					NPC.damage = 0;
					counter++;
					NPC.velocity.Y *= 0.93f;
					NPC.noTileCollide = true;
					Riseslowly();
					NPC.aiStyle = -1;
					break;



				case ActionState.Dashright:
					NPC.damage = 600;
					counter++;
					NPC.velocity.X *= 0.98f;
					Dashright();
					NPC.noTileCollide = false;
					NPC.aiStyle = -1;
					break;

				case ActionState.Dashright2:
					NPC.damage = 600;
					counter++;
					NPC.velocity.X *= 0.97f;
					NPC.noTileCollide = false;
					Dashright2();
					NPC.aiStyle = -1;
					break;

				case ActionState.StopRight:
					NPC.damage = 600;
					counter++;
					NPC.velocity.X *= 0.97f;
					NPC.velocity.Y *= 0f;
					StopRight();
					NPC.aiStyle = -1;
					break;

				case ActionState.StopLeft:
					NPC.damage = 600;
					counter++;
					NPC.velocity.X *= 0.96f;
					NPC.velocity.Y *= 0f;
					StopLeft();
					NPC.aiStyle = -1;
					break;

				case ActionState.Dashleft:
					NPC.damage = 600;
					NPC.noTileCollide = false;
					counter++;
					NPC.velocity.X *= 0.98f;
					Dashleft();
					NPC.aiStyle = -1;
					break;

				case ActionState.Dashleft2:
					NPC.damage = 600;
					counter++;
					NPC.noTileCollide = false;
					NPC.velocity.X *= 0.98f;
					Dashleft2();
					NPC.aiStyle = -1;
					break;

				case ActionState.Across:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.velocity.X *= 0.985f;
					Across();
					NPC.aiStyle = -1;
					break;

				case ActionState.Acrossfinish:
					NPC.damage = 0;
					counter++;
					NPC.velocity.X *= 0.94f;
					NPC.noTileCollide = false;
					NPC.velocity.Y *= 0f;
					Acrossfinish();
					NPC.aiStyle = -1;
					break;






					//case ActionState.HandsoutVoid:
					/*	NPC.damage = 0;
                        counter++;
                        NPC.noGravity = true;

                        if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                        {
                            Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

                        }

                        if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
                        {
                            float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                            Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
                        }

                        SummonVoid();
                        NPC.aiStyle = -1;
                    break;
                    */


					////////////////////////////////////////////////////////////////////////////////////
					///

			}
		}



		private void Fallslowly()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				var entitySource = NPC.GetSource_FromThis();
				Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				float offsetX = Main.rand.Next(-50, 50) * 0.01f;





				NPC.spriteDirection *= -1;
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer == 3)
			{
				NPC.velocity = new Vector2(NPC.direction * 2, +15f);

			}




			if (timer == 40)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Across;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}

		private void StopRight()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}




			if (timer == 50)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Dashright2;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}


		private void StopLeft()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}




			if (timer == 50)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Dashleft2;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}


		private void Acrossfinish()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}




			if (timer == 50)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Dashright2;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}



		private void Fallslowly2()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{

				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer == 20)
			{
				NPC.velocity = new Vector2(NPC.direction * 2, +22f);

			}




			if (timer == 40)
			{
				NPC.spriteDirection *= -1;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Dashleft;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}


		private void Riseslowly()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{

				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer == 20)
			{
				NPC.velocity = new Vector2(NPC.direction * 2, -22f);

			}




			if (timer == 40)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.spriteDirection *= -1;
				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Across;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}



		private void Dashright()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 3)
			{

				int distance = Main.rand.Next(-15, -15);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = -dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}




			if (timer == 40)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.StopRight;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}


		private void Dashright2()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 3)
			{

				int distance = Main.rand.Next(-15, -15);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = -dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}




			if (timer == 40)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Fallslowly2;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}













		private void Dashleft()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 3)
			{

				int distance = Main.rand.Next(-15, -15);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}




			if (timer == 40)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.StopLeft;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}



		private void Dashleft2()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 3)
			{

				int distance = Main.rand.Next(-15, -15);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.velocity.Y = 0;
				ShakeModSystem.Shake = 3;
			}




			if (timer == 40)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Riseslowly;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}







		private void Across()
		{
			timer++;

			Player player = Main.player[NPC.target];
			float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
			float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
			float speed = 25f;
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer < 30)
			{
				NPC.velocity.Y *= 0f;
				NPC.velocity.X += 0.3f;
			}


			if (timer == 10)
			{
				

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(),NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));
				
			}

			if (timer == 30)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));

			}

			if (timer == 50)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));

			}

			if (timer == 70)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));

			}

			if (timer == 90)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));

			}

			if (timer == 110)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, -10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXb * 0, 10, ProjectileID.EyeBeam, (int)(14), 0f, 0, 0f, 0f);

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcharilitDrone1"));

			}


			if (timer == 120)
			{
				NPC.damage = 0;
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Acrossfinish;
						break;

				}
				ResetTimers();

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

			}


		}







		
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
			Timer2 = 0;
		}


		public override void OnKill()
		{

			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}

		}

	}
}
