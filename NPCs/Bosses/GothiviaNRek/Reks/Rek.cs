using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.GothiviaNRek.Gothivia;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.NPCs.Bosses.GothiviaNRek.Reks

{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class Rek : ModNPC
	{
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
		private bool _resetTimers;
		private ActionState _state = ActionState.Fallslowly;
		public ActionState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				if(StellaMultiplayer.IsHost)
					NPC.netUpdate = true;
			}
		}
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
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{			
				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(85, 45);
			NPC.damage = 600;
			NPC.defense = 20;
			NPC.lifeMax = 6000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
		
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2.5f;
			
			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

			// The following code assigns a music track to the boss in a simple way.			
		}
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A loving little pet of the sun goddess Gothivia"))
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)_state);
			writer.Write(attackCounter);
			writer.Write(timeBetweenAttacks);
			writer.WriteVector2(dashDirection);
			writer.Write(dashDistance);

			writer.Write(_resetTimers);
        }
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
            attackCounter = reader.ReadInt32();
			timeBetweenAttacks = reader.ReadInt32();
			dashDirection = reader.ReadVector2();
			dashDistance = reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
        }

		int attackCounter;
		int timeBetweenAttacks = 120;
		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 20; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			/*
			Player player = Main.player[NPC.target];
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);*/
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2.5f) + new Vector2(0, -60);

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
		int bee = 220;
		private Vector2 originalHitbox;
		public override void AI()
		{

			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC ba = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				if (!ba.active && ba.type == ModContent.NPCType<Gothiviab>())
				{



					#region Active check
					// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not

					#endregion
				
						NPC.Kill();
					

				}
			}

			NPC.HasBuff<Rekin>();
			

			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
			
			NPC.TargetClosest();
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest();
			}

            Player player = Main.player[NPC.target];
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
			FinishResetTimers();
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
			}
		}

		

		private void Fallslowly()
		{
			timer++;

			Player player = Main.player[NPC.target];
			if (timer == 1)
			{
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				var entitySource = NPC.GetSource_FromThis();
				Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				float offsetX = Main.rand.Next(-50, 50) * 0.01f;

				if (StellaMultiplayer.IsHost)
                {
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<RekGreek1>(), 0, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<RekEye2>(), 0, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<RekLava3>(), 0, 0f, Owner: Main.myPlayer);
				}
			


				NPC.spriteDirection *= -1;
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}
			if (timer == 3)
			{
				NPC.velocity = new Vector2(NPC.direction * 2, +15f);

			}


			if (timer == 40)
			{
                ResetTimers();
                State = ActionState.Across;
            }
		}

		private void StopRight()
		{
			timer++;

			Player player = Main.player[NPC.target];
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}

			if (timer == 50)
			{
                ResetTimers();
                State = ActionState.Dashright2;
            }
		}


		private void StopLeft()
		{
			timer++;

			Player player = Main.player[NPC.target];
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}

			if (timer == 50)
			{
                ResetTimers();
                State = ActionState.Dashleft2;
            }
		}


		private void Acrossfinish()
		{
			timer++;

			Player player = Main.player[NPC.target];
			if (timer == 1)
			{
				// SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}

			if (timer == 50)
			{
                ResetTimers();
                State = ActionState.Dashright2;
            }
		}



		private void Fallslowly2()
		{
			timer++;

			Player player = Main.player[NPC.target];
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
                ResetTimers();
                NPC.spriteDirection *= -1;
                State = ActionState.Dashleft;
            }
		}


		private void Riseslowly()
		{
			timer++;

			Player player = Main.player[NPC.target];
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
                ResetTimers();
                NPC.spriteDirection *= -1;
                State = ActionState.Across;
            }
		}



		private void Dashright()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/RekRoar"));
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
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.StopRight;
            }
		}


		private void Dashright2()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/RekRoar"));
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
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.Fallslowly2;
            }
		}













		private void Dashleft()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/RekRoar"));
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
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.StopLeft;
            }
		}



		private void Dashleft2()
		{
			timer++;

			Player player = Main.player[NPC.target];

			float speed = 25f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/RekRoar"));
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
                ResetTimers();
                NPC.damage = 0;
				State = ActionState.Riseslowly;
            }
		}







		private void Across()
		{
			timer++;
			if (timer < 30)
			{
				NPC.velocity.Y *= 0f;
				NPC.velocity.X += 0.3f;
			}

			if (timer == 120)
            {
                ResetTimers();
                NPC.damage = 0;
				State = ActionState.Acrossfinish;
            }
		}

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                timer = 0;
                frameCounter = 0;
                frameTick = 0;
                _resetTimers = false;
            }
        }

        public void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
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
