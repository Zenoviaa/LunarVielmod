
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;
using Stellamod.NPCs.Catacombs;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    public class StarrVeriplant : ModNPC
    {
        private float _teleportX;
        private float _teleportY;
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
			TeleportSlam,
            SpinLONG,
            TeleportBIGSlam,
            BIGSlam,
            BIGLand,
			FallSlam,
			FallBIGSlam,
			StartSlam,
			FallStartSlam,
			TeleportStartSlam
		}

		private bool _resetTimers;
		private ActionState _state = ActionState.Start;
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

		public int rippleCount = 10;
		public int rippleSize = 20;
		public int rippleSpeed = 25;
		public float distortStrength = 210f;
		public float Spawner = 0;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return Spawner > 30;
        }

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 64;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Stellamod/NPCs/Bosses/StarrVeriplant/StarrPreview",
				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.width = 80;
			NPC.height = 44;
			NPC.damage = 1;
			NPC.defense = 5;
			NPC.lifeMax = 600;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(copper: 40);
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
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Veriplant");
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A beloved magical stone guardian, protected the natural life and would petrify anyone who disturbs it."))
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)State);
			writer.Write(_resetTimers);
			writer.Write(_teleportX);
            writer.Write(_teleportY);
        }

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
			_teleportX = reader.ReadSingle();
			_teleportY = reader.ReadSingle();
        }
		

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Spawn"), NPC.position);
				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X + 40, (int)NPC.Center.Y + 30, ModContent.NPCType<StoneDeath>());
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hay, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
			}
			else
			{
				for (int k = 0; k < 7; k++)
				{

					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
				}
			}
		}

			
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Rectangle rect;
		
			
			originalHitbox = new Vector2(NPC.width / 100, (NPC.height / 2) + 46);



			
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
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.TeleportPulseIn:
					rect = new Rectangle(0, 2 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.TeleportWindUp:
					rect = new Rectangle(0, 27 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				

				case ActionState.TeleportSlam:
					rect = new Rectangle(0, 46 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.TeleportStartSlam:
					rect = new Rectangle(0, 46 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.TeleportBIGSlam:
					rect = new Rectangle(0, 46 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.TeleportPulseOut:
					rect = new(0, 20 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.Dash:
					rect = new(0, 39 * 89, 80, 7 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
				
				
				case ActionState.Slam:
					rect = new(0, 53 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.StartSlam:
					rect = new(0, 53 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.BIGSlam:
					rect = new(0, 53 * 89, 80, 1 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 100, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.FallBIGSlam:
					rect = new(0, 53 * 89, 80, 1 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.FallSlam:
					rect = new(0, 53 * 89, 80, 1 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.FallStartSlam:
					rect = new(0, 53 * 89, 80, 1 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.BIGLand:
					rect = new(0, 54 * 89, 80, 5 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.WindUp:
					rect = new(0, 34 * 89, 80, 5 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.WindUpSp:
					rect = new(0, 34 * 89, 80, 5 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Spin:
					rect = new(0, 59 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.SpinLONG:
					rect = new(0, 59 * 89, 80, 6 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Pulse:
					rect = new(0, 9 * 89, 80, 11 * 89);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
			}

			return false;
		}

		int bee = 220;
        private Vector2 originalHitbox;

        public override void AI()
		{
			Spawner++;
			bee--;

			if (bee == 0)
            {
				bee = 220;
            }

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];

			NPC.TargetClosest();
			if (player.dead)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y -= 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(1);
				Falling();
				
			}

			FinishResetTimers();
            if (_teleportX != 0 || _teleportY != 0)
            {
                NPC.position.X = _teleportX;
                NPC.position.Y = _teleportY;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                _teleportX = 0f;
                _teleportY = 0f;
            }

            switch (State)
			{
				case ActionState.Pulse:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
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
					NPC.noTileCollide = false;
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
					NPC.noTileCollide = false;
					break;

				case ActionState.StartSlam:
					NPC.damage = 0;
					counter++;
					StartSlam();
					break;

				case ActionState.BIGLand:
					NPC.damage = 0;
					counter++;
					BIGLand();
					break;

				case ActionState.Start:
					NPC.damage = 0;
					counter++;
					Start();
					break;

				case ActionState.Dash:
					NPC.noTileCollide = false;
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

				case ActionState.TeleportStartSlam:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportStartSlam();
					break;

				case ActionState.TeleportBIGSlam:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportBIGSlam();
					break;

				case ActionState.BIGSlam:
					NPC.damage = 0;
					NPC.noTileCollide = false;
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

				case ActionState.FallBIGSlam:
					NPC.damage = 0;
					NPC.velocity *= 2;
					counter++;
					FallBIGSlam();
					break;


				case ActionState.FallSlam:
					NPC.damage = 0;
					NPC.velocity *= 2;
					counter++;
					FallSlam();
					break;

				case ActionState.FallStartSlam:
					NPC.damage = 0;
					NPC.velocity *= 2;
					counter++;
					FallStartSlam();
					break;

				case ActionState.TeleportWindUp:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					TeleportWindUp();
					NPC.noTileCollide = false;
					break;
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

				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(3))
                    {
                        case 0:

                            float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                            float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY * 1f,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 2, speedY - 3 * 1.5f,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 1,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY - 2 * 2f,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 3, speedY - 1 * 1f,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 3,
                                ProjectileID.DandelionSeed, 3, 0f, Owner: Main.myPlayer);
                            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"));
                            break;

                        case 1:

                            break;

                        case 2:
                            float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                            float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 3, speedYa * 1f, 
								ModContent.ProjectileType<CosButterfly>(), 4, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 1 * 3, speedYa - 2 * 1f, 
								ModContent.ProjectileType<CosButterfly>(), 4, 0f, Owner: Main.myPlayer);
                            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"));
                            break;

                    }
                }

			}

			

			if (timer == 23)
			{
				//Reset our timers
                ResetTimers();

				//Set the new state
				//Don't forget to check IsHost!
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.TeleportPulseIn;
                            break;
                        case 1:
                            State = ActionState.TeleportPulseIn;
                            break;

                    }

                }
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
				ResetTimers();
                State = ActionState.TeleportWindUp;
            }
			
		}
        private void TeleportSlam()
        {
            
			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)	
			{
				if (StellaMultiplayer.IsHost)
				{
                    int distanceY = -250;
                    _teleportX = player.Center.X;
                    _teleportY = player.Center.Y + distanceY;
                    NPC.netUpdate = true;
                }

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));
			}

						
			if (timer == 27)
            {
                ResetTimers();
                State = ActionState.FallSlam;
			}	
		}


		private void TeleportStartSlam()
		{

			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)
            {
				if (StellaMultiplayer.IsHost)
				{
					int distanceY = -250;
                    _teleportX = player.Center.X;
                    _teleportY = player.Center.Y + distanceY;
					NPC.netUpdate = true;
				}
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));
			}

			if (timer == 27)
            {
                ResetTimers();
                State = ActionState.FallStartSlam;
			}
		}


		private void TeleportBIGSlam()
		{

			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));

				if (StellaMultiplayer.IsHost)
                {
                    int distanceY = -300;
					_teleportX = player.Center.X;
                    _teleportY = player.Center.Y + distanceY;
                    NPC.netUpdate = true;
				}

            }

			if (timer == 27)
			{
				State = ActionState.FallBIGSlam;

				ResetTimers();
			}


		}
		private void TeleportWindUp()
		{
			timer++;


			Player player = Main.player[NPC.target];

			if (timer == 1) {

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"));

				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            int distance = Main.rand.Next(20, 20);
                            int distanceY = Main.rand.Next(-110, -110);
							_teleportX = player.Center.X + distance;
							_teleportY = player.Center.Y + distanceY;
							NPC.netUpdate = true;
                            break;


                        case 1:
                            int distance2 = Main.rand.Next(-120, -120);
                            int distanceY2 = Main.rand.Next(-110, -110);
                            _teleportX = player.Center.X + distance2;
                            _teleportY = player.Center.Y + distanceY2;
                            NPC.netUpdate = true;
                            break;
                    }
                }
            }

			if (timer == 27)
			{
                ResetTimers();
                State = ActionState.WindUp;
			}	
        }



        private void Dash()
        {
			timer++;

			Player player = Main.player[NPC.target];
		
			float speed = 12f;
			if (timer == 1)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
			}

			if (timer < 5)
            {
				NPC.damage = 30;
				int distance = Main.rand.Next(3, 3);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				Vector2 dashDirection = (player.Center - (angle * distance)) - NPC.Center;
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
                    ResetTimers();
					if (StellaMultiplayer.IsHost)
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
                    }
				}

				if (NPC.life > NPC.lifeMax / 2)
                {
                    ResetTimers();
                    if (StellaMultiplayer.IsHost)
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
                    }

				}
			}	
        }



        private void Start()
        {
			timer++;
			if (timer == 20)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.TeleportStartSlam;
            }		
        }


		private void FallSlam()
        {
			timer++;
			if (NPC.collideY)
			{
				NPC.velocity.Y *= 0;
                ResetTimers();
                State = ActionState.Slam;
			}

			if (timer < 15)
			{
				NPC.noTileCollide = true;
			}

			if (timer > 15)
			{
				NPC.noTileCollide = false;
			}

			if (timer > 80)
            {
                ResetTimers();
                State = ActionState.Slam;	
			}
		}

		private void FallBIGSlam()
		{
			timer++;
			if (NPC.collideY)
			{
				NPC.velocity.Y *= 0;
                ResetTimers();
                State = ActionState.BIGSlam;		
			}


			if (timer > 120)
            {
                ResetTimers();
                State = ActionState.BIGSlam;
			}
		}

		private void FallStartSlam()
		{
			timer++;
			if (NPC.collideY)
			{
				NPC.velocity.Y *= 0;
                ResetTimers();
                State = ActionState.StartSlam;
			}


			if (timer > 120)
            {
                ResetTimers();
                State = ActionState.StartSlam;
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
					if (StellaMultiplayer.IsHost)
					{
                        float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                        float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 90, speedX + 2 * 6, speedY,
                            ModContent.ProjectileType<SpikeBullet>(), 5, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 90, speedXB - 2 * 6, speedY,
                            ModContent.ProjectileType<SpikeBullet>(), 5, 0f, Owner: Main.myPlayer);
                    }
					
					ShakeModSystem.Shake = 8;

				}
			}
			if (timer == 10)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifall"));
			}
			

			if (timer == 27)
			{
                ResetTimers();
                if (NPC.life > (NPC.lifeMax / 4) + (NPC.lifeMax / 2))
				{
					State = ActionState.WindUp;
				}

				if (NPC.life < (NPC.lifeMax / 4) + (NPC.lifeMax / 2))
				{
					State = ActionState.WindUpSp;
				}
			}
		}


		private void StartSlam()
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

					

					ShakeModSystem.Shake = 8;

				}
			}
			if (timer == 10)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifall"));
			}


			if (timer == 27)
			{
                ResetTimers();
                if (NPC.life > (NPC.lifeMax / 4) + (NPC.lifeMax / 2))
				{
					State = ActionState.WindUp;
				}

				if (NPC.life < (NPC.lifeMax / 4) + (NPC.lifeMax / 2))
				{
					State = ActionState.WindUpSp;
				}
			}
		}

		private void BIGLand()
		{
			timer++;

			if (timer == 1)
            {
				ShakeModSystem.Shake = 8;
				if (StellaMultiplayer.IsHost)
				{
                    float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                    float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX * 0, speedY * 0,
                        ProjectileID.DD2OgreSmash, 0, 0f, Owner: Main.myPlayer);
                }
			}



			if (timer == 24)
			{
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

				ResetTimers();
				State = ActionState.WindUpSp;
			}
		}


		private void Falling()
		{
			timer++;
			NPC.velocity *= 2.5f;

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
					if (StellaMultiplayer.IsHost)
					{
                        float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                        float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 2 * 2, speedY,
                            ModContent.ProjectileType<SmallRock2>(), 3, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB - 2 * 2, speedY,
                            ModContent.ProjectileType<SmallRock2>(), 3, 0f, Owner: Main.myPlayer);
                    }

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verifallstar"));
				}
			}

			if (timer > 25)
            {
                ResetTimers();
                State = ActionState.BIGLand;
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

				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            //Summon

                            break;

                        case 1:
                            float speedXB = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                            float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                            float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 2,
                                ModContent.ProjectileType<SmallRock>(), 2, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 1, speedY - 2 * 1,
                                ModContent.ProjectileType<SmallRock>(), 2, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 1,
                                ModContent.ProjectileType<SmallRock>(), 2, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 2, speedY - 2 * 2,
                                ModContent.ProjectileType<SmallRock>(), 2, 0f, Owner: Main.myPlayer);

                            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
                            break;


                        case 2:
                            float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                            float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                            float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 60, speedXb * 0.1f, speedYb - 1 * 1,
                                ModContent.ProjectileType<BigRock>(), 11, 0f, Owner: Main.myPlayer);
                            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
                            break;

                        case 3:
                            float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                            float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                            float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 5, speedYa - 1 * 1,
                                ModContent.ProjectileType<Rock>(), 5, 0f, Owner: Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 5, speedYa - 1 * 1,
                                ModContent.ProjectileType<Rock>(), 5, 0f, Owner: Main.myPlayer);
                            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Flowers"));
                            break;

                    }
                }
			}
			

			if (timer == 23)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.TeleportPulseIn;
            }
		}



        private void WindUp()
        {
			timer++;
			if (timer == 27)
            {
                ResetTimers();
                if (NPC.life > NPC.lifeMax / 2 && NPC.life > (NPC.lifeMax / 4) + (NPC.lifeMax / 2))				
				{
                    State = ActionState.Dash;
                }


				if (NPC.life > NPC.lifeMax / 2 && NPC.life < (NPC.lifeMax / 4) + (NPC.lifeMax / 2))
				{
					if (StellaMultiplayer.IsHost)
					{
                        switch (Main.rand.Next(2))
                        {

                            case 0:
                                State = ActionState.SpinLONG;
                                break;
                            case 1:
                                State = ActionState.Dash;
                                break;
                        }
                    }
				}

				if (NPC.life < NPC.lifeMax / 2)
				{
					switch (Main.rand.Next(2))
					{

						case 0:
							State = ActionState.Spin;
							break;
						case 1:
							State = ActionState.Dash;
							break;
					}
				}
			}
		}

		private void WindUpSp()
		{
			timer++;
			if (timer == 27)
			{
                ResetTimers();
                if (NPC.life < NPC.lifeMax / 2)
                {
                    State = ActionState.Spin;
                }
				else if (NPC.life > NPC.lifeMax / 2)
				{
                    State = ActionState.SpinLONG;
                }
			}
		}

		private void Pulse()
        {
			timer++;
			Player player = Main.player[NPC.target];
			if (timer == 7)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veripulse"));
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            CombatText.NewText(NPC.getRect(), Color.YellowGreen, LangText.Misc("StarrVeriplant.1"), true, false);
                            player.AddBuff(BuffID.Weak, 60);
                            break;


                        case 1:
                            CombatText.NewText(NPC.getRect(), Color.MistyRose, LangText.Misc("StarrVeriplant.2"), true, false);
                            player.AddBuff(BuffID.BrokenArmor, 60);
                            break;


                        case 2:
                            CombatText.NewText(NPC.getRect(), Color.Coral, LangText.Misc("StarrVeriplant.3"), true, false);
                            player.AddBuff(BuffID.Wrath, 300);
                            break;


                        case 3:

                            CombatText.NewText(NPC.getRect(), Color.Purple, LangText.Misc("StarrVeriplant4."), true, false);
                            player.AddBuff(BuffID.Swiftness, 600);
                            break;
                    }
                }
			}

			if (timer == 43)
            {
                ResetTimers();
                State = ActionState.TeleportPulseOut;
			}		
		}


        public override void OnKill()
        {
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedStoneGolemBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 10, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StoneKey>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<VeriBossRel>()));
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
    }
}
