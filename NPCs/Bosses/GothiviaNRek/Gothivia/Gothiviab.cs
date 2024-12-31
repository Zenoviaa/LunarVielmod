using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gothivia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.GothiviaNRek.Reks;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Gothivia
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class Gothiviab : ModNPC
	{
		private bool _resetTimers;
		public enum ActionState
		{


			Idle,
			ReallyIdle,
			Idle2,
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




		}
		private ActionState _state = ActionState.ReallyIdle;
		// Current state
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
        public float GothiviaStartPosTime;
        public Vector2 GothiviaStartPos;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia of The Moon");

			Main.npcFrameCount[Type] = 41;
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
			NPC.Size = new Vector2(40, 45);
			NPC.damage = 1;
			NPC.defense = 20;
			NPC.lifeMax = 8500;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 60);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;
			NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();
			NPC.aiStyle = 0;

			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/IrradiaNHavoc");
			}
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "This person isn't the real gothivia, where is she?"))
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)_state);
			writer.Write(_resetTimers);
        }

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
        }

		bool axed = false;
        bool p2 = false;
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
			SpriteEffects effects = SpriteEffects.None;

			if (player.Center.X > NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Rectangle rect;
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(0, -35);

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



			switch (State)
			{
				case ActionState.Idle:
					rect = new(0, 1 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 100, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.ReallyIdle:
					rect = new(0, 1 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 300, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Idle2:
					rect = new(0, 1 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 100, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.StartGothivia:
					rect = new(0, 1 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 100, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.StartRollLeft:
					rect = new(0, 29 * 52, 56, 9 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 9, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.StartRollRight:
					rect = new(0, 29 * 52, 56, 9 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 9, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.RollLeft:
					rect = new(0, 38 * 52, 56, 3 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.RollRight:
					rect = new(0, 38 * 52, 56, 3 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.PunchingFirstPhaseLaserBomb:
					rect = new(0, 1 * 52, 56, 16 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 16, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.PunchingSecondPhaseFlameBalls:
					rect = new(0, 1 * 52, 56, 16 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 16, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.PunchingSecondPhaseStopSign:
					rect = new(0, 1 * 52, 56, 16 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 16, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Fall:
					rect = new(0, 24 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 300, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Land:
					rect = new(0, 25 * 52, 56, 3 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Jump:
					rect = new(0, 17 * 52, 56, 6 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.JumpToMiddle:
					rect = new(0, 17 * 52, 56, 6 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 10, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.FallToMiddle:
					rect = new(0, 24 * 52, 56, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 300, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.LandToMiddle:
					rect = new(0, 25 * 52, 56, 3 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
			}


			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame
		int bee = 220;
		private Vector2 originalHitbox;
		//int Timer2 = 0;
		float timert = 0;
		public float Spawner = 0;
		public override void AI()
		{
			p2 = NPC.life < NPC.lifeMax * 0.5f;
            bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);

			GothiviaStartPosTime++;

            if (GothiviaStartPosTime <= 1)
			{

				GothiviaStartPos = NPC.position;

            }


			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC ba = Main.npc[k];
				// Check if NPC able to be targeted. It means that NPC is
				if (!ba.active && ba.type == ModContent.NPCType<Rek>() && axed == false)
				{
					timert++;

					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					if (timert == 600)
                    {
						if (StellaMultiplayer.IsHost)
						{
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 0, speedYb * 0, 
								ModContent.ProjectileType<Helios>(), 30, 0f, Owner: Main.myPlayer);

						}

						timert = 0;
					}			
				}
			}





			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
			NPC.spriteDirection = NPC.direction;

			if (!NPC.HasValidTarget)
			{
				NPC.TargetClosest();
				if (!NPC.HasValidTarget)
                {               // If the targeted player is dead, flee
                    NPC.velocity.Y += 0.5f;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                    NPC.EncourageDespawn(2);
                }
			}  

			FinishResetTimers();
			switch (State)
			{


				case ActionState.Idle:
					NPC.damage = 0;
					counter++;
				
					IdleGothivia();
					NPC.aiStyle = -1;
					break;
				
				case ActionState.ReallyIdle:
					NPC.damage = 0;
					counter++;

					ReallyIdleGothivia();
					NPC.aiStyle = -1;
					break;

				case ActionState.Idle2:
					NPC.damage = 0;
					counter++;

					IdleGothivia2();
					NPC.aiStyle = -1;
					break;

				case ActionState.StartGothivia:
					NPC.damage = 0;
					counter++;
				
					StartGothivia();
					NPC.aiStyle = -1;
					break;

				case ActionState.StartRollLeft:
					NPC.damage = 0;
					counter++;
				
					StartRollLeft();
					NPC.aiStyle = -1;
					break;

				case ActionState.RollLeft:
					NPC.damage = 200;
					counter++;
			
					RollLeft();
					NPC.aiStyle = -1;
					break;

				case ActionState.StartRollRight:
					NPC.damage = 0;
					counter++;
				
					StartRollRight();
					NPC.aiStyle = -1;
					break;

				case ActionState.PunchingFirstPhaseLaserBomb:
					NPC.damage = 0;
					counter++;

					PunchFlameLaser();
					NPC.aiStyle = -1;
					break;
				
				case ActionState.PunchingSecondPhaseFlameBalls:
					NPC.damage = 0;
					counter++;

					PunchFireballs();
					NPC.aiStyle = -1;
					break;

				case ActionState.PunchingSecondPhaseStopSign:
					NPC.damage = 0;
					counter++;

					PunchSign();
					NPC.aiStyle = -1;
					break;

				case ActionState.RollRight:
					NPC.damage = 200;
					counter++;
					
					RollRight();
					NPC.aiStyle = -1;
					break;

				case ActionState.Fall:
					NPC.damage = 0;
					counter++;
					NPC.velocity.Y *= 1.3f;
					NPC.aiStyle = -1;
					NPC.noTileCollide = false;
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X *= 0.7f;
						State = ActionState.Land;
						frameCounter = 0;
						frameTick = 0;
					}
					// You dont need to do anything here
					break;

				case ActionState.FallToMiddle:
					NPC.damage = 0;
					counter++;
					NPC.velocity.Y *= 1.3f;
					NPC.aiStyle = -1;
					NPC.noTileCollide = false;
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0.7f;
						State = ActionState.LandToMiddle;
						frameCounter = 0;
						frameTick = 0;
					}
					// You dont need to do anything here
					break;

				case ActionState.Land:
					NPC.damage = 0;
					counter++;

					Land();
					NPC.aiStyle = -1;
					break;

				case ActionState.JumpToMiddle:
					NPC.damage = 0;
					counter++;
					JumpToMiddle();
					NPC.aiStyle = -1;
                    break;

				case ActionState.LandToMiddle:
					NPC.damage = 0;
					counter++;

					LandToMiddle();
					NPC.aiStyle = -1;
					break;
			}
		}

		private void ReallyIdleGothivia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;

			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 600, (int)NPC.Center.Y + 20, ModContent.NPCType<Rek>());
                }
			}

			if (timer == 20)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 600, (int)NPC.Center.Y - 20, ModContent.NPCType<Train2>());
				}
			}

			if (timer == 39)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 600, (int)NPC.Center.Y - 20, ModContent.NPCType<Train1>());
				}
			}

			if (timer == 40)
			{
                ResetTimers();
                State = ActionState.StartGothivia;
            }
		}

		private void IdleGothivia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 40)
			{
	
				if (NPC.life >= NPC.lifeMax / 2)
                {
                    ResetTimers();
                    State = ActionState.PunchingFirstPhaseLaserBomb;
                }

				if (NPC.life < NPC.lifeMax / 2)
                {
                    ResetTimers();
					if (StellaMultiplayer.IsHost)
					{
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                State = ActionState.PunchingFirstPhaseLaserBomb;
                                break;

                            case 1:
                                State = ActionState.PunchingSecondPhaseFlameBalls;
                                break;

                            case 2:
                                State = ActionState.PunchingSecondPhaseStopSign;
                                break;
                        }
                    }
				}
			}
		}


		private void IdleGothivia2()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 40)
			{

				if (NPC.life >= NPC.lifeMax / 2)
                {
                    ResetTimers();
                    State = ActionState.PunchingFirstPhaseLaserBomb;
				}

				if (NPC.life < NPC.lifeMax / 2)
				{
                    ResetTimers();
					if (StellaMultiplayer.IsHost)
					{
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                State = ActionState.PunchingFirstPhaseLaserBomb;
                                break;

                            case 1:
                                State = ActionState.PunchingSecondPhaseFlameBalls;
                                break;

                            case 2:
                                State = ActionState.PunchingSecondPhaseStopSign;
                                break;
                        }
                    }
				}
			}
		}

		private void StartGothivia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 40)
			{
                ResetTimers();
                State = ActionState.StartRollLeft;
			}
		}

		private void StartRollLeft()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 27)
			{
                ResetTimers();
                State = ActionState.RollLeft;
			}
		}

		private void RollLeft()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer <= 1)
			{
				NPC.velocity.X += 15;
            }

            if (NPC.velocity.Y == 0 && timer >= 60)
			{
                ResetTimers();
                NPC.velocity.X -= 1;
				if (StellaMultiplayer.IsHost)
				{
                    var entitySource = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                        Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                }

                State = ActionState.Idle2;
			}
		}

		private void StartRollRight()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 27)
			{
                ResetTimers();
                State = ActionState.RollRight;
			}
		}

		private void RollRight()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
            if (timer <= 1)
            {
                NPC.velocity.X -= 15;
            }
            if (NPC.velocity.Y == 0 && timer >= 60)
			{
                ResetTimers();

				if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                        Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                }
       
                NPC.velocity.X += 1;  
                State = ActionState.Idle;
            }
		}

		public override bool? CanFallThroughPlatforms()
		{
			if (State == ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
			{
				// Dont do anything here
				return true;
			}

			return false;
			// You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
		}

		private void PunchFlameLaser()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// One moving beam thing that goes across the whole floor, depending on the side it changes
			// The lower her health is the more beams are punched out, like 2 at half, 4 at quarter
			// do that with the two punches using the timer to line up the punches ig, (just make them spaced out enough to dodge)


			timer++;
            if (timer == 12)
            {
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/bloodlamp"), NPC.position);
                var entitySource = NPC.GetSource_FromThis();
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
				if (StellaMultiplayer.IsHost)
				{
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX / 2, speedYb * 0,
                        ModContent.ProjectileType<LaserShooterFirstPhase>(), 10, 0f, Owner: Main.myPlayer);
                }	
            }

            if (timer == 48)
			{
                ResetTimers();
                State = ActionState.Fall;
			}
		}
		private void PunchFireballs()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// Two flames that go in a sine wave (check verlia's sine sword)
			// do that with the two punches using the timer to line up the punches ig, (just make them spaced out enough to dodge, you can use the fireballs from jack for the sprite tbh)


			timer++;
			if (p2)
			{
                if (timer == 12)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                    float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                    int damage = Main.expertMode ? 44 : 47;

					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                            Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
                            ModContent.ProjectileType<FlameBlast>(), damage, 1, Owner: Main.myPlayer);
                    }
                }
                if (timer == 33)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                    float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                    int damage = Main.expertMode ? 44 : 47;

					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
                            ModContent.ProjectileType<FlameBlast>(), damage, 1, Owner: Main.myPlayer);
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                            Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                    }
                }
            }
			else
			{
                if (timer == 12)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
					
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                    float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                    int damage = Main.expertMode ? 34 : 37;

					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                              Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
                            ModContent.ProjectileType<FlameBlast>(), damage, 1, Owner: Main.myPlayer);
                    }
                }
                if (timer == 33)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
			
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                    float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                    int damage = Main.expertMode ? 34 : 37;

					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
                            Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
                            ModContent.ProjectileType<FlameBlast>(), damage, 1, Owner: Main.myPlayer);
                    }
                }
            }

            if (timer == 48)
			{
                ResetTimers();
                State = ActionState.Fall;
            }
		}

		private void PunchSign()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// I was planning a petrifying sign, but tbh you could make a spining guitar that acts like daedus's axe or literally anything 
			// 


			timer++;
			if (p2)
			{
                if (timer == 12)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-10, 10);
                    int damage = Main.expertMode ? 44 : 47;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), 
							Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music2>(), damage, 1, Owner: Main.myPlayer);
                         offsetX = Main.rand.Next(-10, 10);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music1>(), damage, 1, Owner: Main.myPlayer);
                         offsetX = Main.rand.Next(-10, 10);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music1>(), damage, 1, Owner: Main.myPlayer);
                    }

				}
                if (timer == 33)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-10, 10);
                    int damage = Main.expertMode ? 44 : 47;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0),
							Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music2>(), damage, 1, Owner: Main.myPlayer);
                         offsetX = Main.rand.Next(-10, 10);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
							ModContent.ProjectileType<Music1>(), damage, 1, Owner: Main.myPlayer);
                         offsetX = Main.rand.Next(-10, 10);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music2>(), damage, 1, Owner: Main.myPlayer);
                    }
				}
			}
			else
			{
                if (timer == 12)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				
					SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-5, 5);
                    int damage = Main.expertMode ? 44 : 47;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), 
							Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0, 
							ModContent.ProjectileType<Music2>(), damage, 1, Owner: Main.myPlayer);
                        offsetX = Main.rand.Next(-5, 5);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
							ModContent.ProjectileType<Music1>(), damage, 1, Owner: Main.myPlayer);
                    }

                }
                if (timer == 33)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                   
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                    float offsetX = Main.rand.Next(-5, 5);
                    int damage = Main.expertMode ? 44 : 47;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(entitySource, NPC.Center, new Vector2(0, 0), 
							Mod.Find<ModProjectile>("JackSpawnEffect").Type, NPC.damage * 0, 0, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
							ModContent.ProjectileType<Music2>(), damage, 1, Owner: Main.myPlayer);
                        offsetX = Main.rand.Next(-5, 5);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X + offsetX, 0,
							ModContent.ProjectileType<Music1>(), damage, 1, Owner: Main.myPlayer);
                    }
                }
            }

            if (timer == 48)
			{
                ResetTimers();
                State = ActionState.Fall;
			}
		}

		

		private void Land()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// Maybe a land effect or projectile?
			// 


			timer++;
			if (timer == 12)
			{
                ResetTimers();
                State = ActionState.JumpToMiddle;
            }
		}

		private void JumpToMiddle()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// Maybe a land effect or projectile?
			// 


			timer++;
            if (timer <= 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Hyuh"), NPC.position);
				NPC.noTileCollide = true;
				NPC.velocity.Y -= 15.0f;
				if(GothiviaStartPos.X <= NPC.position.X)
				{
                    NPC.velocity.X -= 9;
				}
				else
				{
                    NPC.velocity.X += 9;
                }
            }
            if (NPC.velocity.Y >= 0)
            {
                NPC.noTileCollide = false;
            }
            if (timer == 80)
            {
                ResetTimers();
                State = ActionState.FallToMiddle;
			}
		}

		private void LandToMiddle()
		{
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			// Maybe a land effect or projectile?
			// 


			timer++;
			if (timer == 12)
			{
                ResetTimers();
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.StartRollLeft;
                            break;

                        case 1:
                            State = ActionState.StartRollRight;
                            break;

                    }
                }
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			//	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




			// ItemDropRule.MasterModeCommonDrop for the relic

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 3, 7));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GothiviaBag>()));
			// ItemDropRule.MasterModeDropOnAllPlayers for the pet
			//npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

		// All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
				ModContent.ItemType<BurningGBroochA>(),
				ModContent.ItemType<Items.Weapons.Melee.Helios>(),
				ModContent.ItemType<GothiviasCard>(),
				ModContent.ItemType<Twirlers>(),
				ModContent.ItemType<WeddingDay>()));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));
			notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.TitaniumBar, minimumDropped: 4, maximumDropped: 25));

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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedGothBoss, -1);
		}

	}
}
