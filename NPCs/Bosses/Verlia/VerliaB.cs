using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Verlia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles.Sword;
using Stellamod.NPCs.Projectiles;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static ParticleLibrary.Particle;

namespace Stellamod.NPCs.Bosses.Verlia
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class VerliaB : ModNPC
	{
		private float _teleportX;
		private float _teleportY;
		private bool _resetTimers;
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
			MoonSummonStartup,
			CloneSummonStartup,
			BigSwordSummonStartup,
			BeggingingMoonStart,
			Dienow,
			SummonBeamer,
			idleSummonBeamer,
			HoldUPdie,
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

		private ActionState _state = ActionState.BeggingingMoonStart;


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

			Main.npcFrameCount[Type] = 80;

			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

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
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Verlia/VerliaPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(24, 42);
			NPC.damage = 1;
			NPC.defense = 15;
			NPC.lifeMax = 6750;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 12);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;

			NPC.aiStyle = 0;






			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<VerliaBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Verlia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman."))
			});
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((float)_state);
			writer.WriteVector2(dashDirection);
			writer.Write(dashDistance);
			writer.Write(_resetTimers);
			writer.Write(_teleportX);
			writer.Write(_teleportY);
			//writer.Write(frameCounter);
			//writer.Write(frameTick);
			//writer.Write(counter);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			_state = (ActionState)reader.ReadSingle();
			dashDirection = reader.ReadVector2();
			dashDistance = reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
			_teleportX = reader.ReadSingle();
			_teleportY = reader.ReadSingle();
            //frameCounter = reader.ReadInt32();
            //frameTick = reader.ReadInt32();
            //counter = reader.ReadInt32();
        }

		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
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

				case ActionState.BeggingingMoonStart:
					rect = new(0, 1 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.SummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.BigSwordSummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.MoonSummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.CloneSummonStartup:
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

				case ActionState.Dienow:
					rect = new(0, 1 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.SummonBeamer:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.idleSummonBeamer:
					rect = new Rectangle(0, 3 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

				case ActionState.HoldUPdie:
					rect = new Rectangle(0, 12 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;
			}
			
			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame
		

        int bee = 220;
		private Vector2 originalHitbox;

		public float Spawner = 0;
		public override void AI()
		{
			Spawner++;
			NPC.TargetClosest();
			NPC.velocity *= 0.97f;
			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);


			

			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.Center, RGB.X, RGB.Y, RGB.Z);
			
			Player player = Main.player[NPC.target];

			

			if(NPC.life > NPC.lifeMax / 2)
			{
				NPC.takenDamageMultiplier = 0.7f;
			}
			else
			{
                NPC.takenDamageMultiplier = 1f;
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
				case ActionState.StartVerlia:
					NPC.damage = 0;
					counter++;
					StartVerlia();
					break;

				case ActionState.BeggingingMoonStart:
					NPC.damage = 0;
					counter++;
					MoonStartVerlia();
					break;

				case ActionState.SummonStartup:
					NPC.damage = 0;
					counter++;
					StartSummonVerlia();
					break;

				case ActionState.MoonSummonStartup:
					NPC.damage = 0;
					counter++;

					if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", NPC.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(NPC.Center);

					}

					if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
					{
						float progress = (180f - bee) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
						Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
					}
					MoonStartSummonVerlia();
					break;

					case ActionState.CloneSummonStartup:
					NPC.damage = 0;
					counter++;
					CloneStartSummonVerlia();
					break;

				case ActionState.BigSwordSummonStartup:
					NPC.damage = 0;
					counter++;
					BigSwordStartSummonVerlia();
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

				case ActionState.Dienow:
					NPC.damage = 0;
					counter++;
					Dienow();
					break;

				case ActionState.SummonBeamer:
					NPC.damage = 0;
					counter++;
					SummonBeamer();
					break;

				case ActionState.idleSummonBeamer:
					NPC.damage = 0;
					counter++;
					idleSummonBeamer();
					break;

				case ActionState.HoldUPdie:
					NPC.damage = 0;
					counter++;
					HoldUPVerliadie();
					break;
			}
		}


		private void StartVerlia()
		{
			timer++;
			if (timer == 2)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            State = ActionState.SummonStartup;
                            break;
                        case 1:
                            State = ActionState.MoonSummonStartup;
                            break;
                        case 2:
                            State = ActionState.CloneSummonStartup;
                            break;
                        case 3:
                            State = ActionState.BigSwordSummonStartup;
                            break;
                    }
                }

 
            }
		}


		private void Dienow()
		{
			timer++;
			if (timer == 2)
            {
                ResetTimers();
                State = ActionState.SummonBeamer;
              
            }
		}

		private void MoonStartVerlia()
		{
			timer++;
			if (timer == 2)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.MoonSummonStartup;
            }

		}

		private void SummonBeamer()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(),
						0, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 
						0, 0f, Owner: Main.myPlayer);
                }
			}
			
			if (timer == 6)
            {
                if (StellaMultiplayer.IsHost)
                    GeneralStellaUtilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y + 1000, 0, -10, ModContent.ProjectileType<VRay>(), 600, 0f, -1, 0, NPC.whoAmI);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AbsoluteDistillence"));
			}
						
			if (timer == 110)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.idleSummonBeamer;
            }
		}

		private void StartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
            {
                if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<Sigil>(), 0, 0f, Owner: Main.myPlayer);
                }
			}

			if (timer > 5)
			{
				if (timer == 6)
				{
					if (StellaMultiplayer.IsHost)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"));
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword1F>(), 7, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 20)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 80, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword1F>(), 7, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 25)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 120, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword1F>(), 9, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 35)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 140, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword2F>(), 9, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 45)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 170, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword2F>(), 9, 0f, Owner: Main.myPlayer);
					}
				}

				if (timer == 54)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 200, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2,
							ModContent.ProjectileType<Sword2F>(), 9, 0f, Owner: Main.myPlayer);
					}

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"));
				}
			}
				
			if (timer == 55)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.SummonIdle;
            }
		}

		private void MoonStartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0,
						ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<Sigil>(), 0, 0f, Owner: Main.myPlayer);
                }
			
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StarCharge"));
			}
			if (timer > 5)
			{
                if (timer == 6)
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedXd = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYd = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXd + 10, NPC.position.Y + speedYd - 130, speedXd - 2 * 2, speedYd - 2 * 2,
							ModContent.ProjectileType<TheMoon>(), 90, 0f, Owner: Main.myPlayer);
                    }
                }
            }

			if (timer == 55)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

				State = ActionState.SummonIdle;
				ResetTimers();
			}
		}


		private void CloneStartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"));
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0,
                        ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0,
                        ModContent.ProjectileType<Sigil>(), 0, 0f, Owner: Main.myPlayer);
                }

            }
			if (timer > 5)
			{
                if (timer == 6)
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedXd = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYd = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 60, ModContent.NPCType<CloneV>());
                    }
                }
            }

			if (timer == 55)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

                ResetTimers();
                State = ActionState.SummonIdle;
			}
		}



		private void BigSwordStartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"));
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0,
						ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, 
						ModContent.ProjectileType<Sigil>(), 0, 0f, Owner: Main.myPlayer);
                }		
			}

			if (timer > 5)
			{
                if (timer == 10)
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0,
                            ModContent.ProjectileType<SineSword>(), 29, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0,
                            ModContent.ProjectileType<SineSword>(), 29, 0f, Owner: Main.myPlayer);
                    }

                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
                }

                if (timer == 30)
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-2f, -2f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(2f, 2f);
                        float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(2, 2) * 0f;
                        float speedYc = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-2, -2) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYb * -1,
							ModContent.ProjectileType<SineSword>(), 27, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYc * 1, 
							ModContent.ProjectileType<SineSword>(), 27, 0f, Owner: Main.myPlayer);
                    }
                   
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
                }

                if (timer == 50)
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-1f, -1f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(1f, 1f);
                        float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(4, 4) * 0f;
                        float speedYc = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYb * -1,
							ModContent.ProjectileType<SineSword>(), 23, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYc * 1, 
							ModContent.ProjectileType<SineSword>(), 23, 0f, Owner: Main.myPlayer);
                    }
                   
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
                }
            }

			if (timer == 55)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.	
                ResetTimers();
                State = ActionState.SummonIdle;
			}
		}


		private void IdleSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 50)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"));
			}
			if (timer == 200)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.Unsummon;
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void idleSummonBeamer()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 50)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"));
			}
			float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
			float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
			float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
			if (timer == 100)
			{
				if (StellaMultiplayer.IsHost)
				{
                    int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 40, 0f, Owner: Main.myPlayer);
                }

			
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 150)
			{
                if (StellaMultiplayer.IsHost)
				{
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 60, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 50, 0f, Owner: Main.myPlayer);
                }


                   
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 200)
			{
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 90, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 40, 0f, Owner: Main.myPlayer);
				}

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 250)
			{
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 20, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 23, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 300)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 40, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 350)
            {
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 20, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 400)
			{
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 450)
			{
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 90, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 50, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}
			if (timer == 500)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 100, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 275)
            {
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 45, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
						ModContent.ProjectileType<AltideSword>(), 22, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 20, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 375)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 60, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 50, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 225)
            {
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 190, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 50, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 125)
			{

				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 230, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 40, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 325)
            {
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 32, 0f, Owner: Main.myPlayer);
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 140, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 475)
            {
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
						ModContent.ProjectileType<AltideSword>(), 22, 0f, Owner: Main.myPlayer);
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 220, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 400)
			{
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, 
						ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 120, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 425)
            {
				if (StellaMultiplayer.IsHost)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
						ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
						ModContent.ProjectileType<AltideSword>(), 22, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 190, speedXb * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}
			if (timer == 175)
            {
				if (StellaMultiplayer.IsHost)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
						ModContent.ProjectileType<AltideSword>(), 22, 0f, Owner: Main.myPlayer);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 200, speedXa * 1, speedYa - 1 * 0, 
						ModContent.ProjectileType<SineSword>(), 30, 0f, Owner: Main.myPlayer);
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}


















			if (timer == 600)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.Unsummon;
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
				if (NPC.life < NPC.lifeMax / 7)
				{

					switch (Main.rand.Next(1))
					{
						case 0:
                            ResetTimers();
                            State = ActionState.HoldUPdie;
							break;


					}





				}
				if (NPC.life > NPC.lifeMax / 7)
				{

					switch (Main.rand.Next(1))
					{
						case 0:
                            ResetTimers();
                            State = ActionState.HoldUP;
							break;

					}





				}
				

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void HoldUPVerlia()
		{
			timer++;
			if (timer == 2)
            {
				if (NPC.life < NPC.lifeMax / 2)
				{
					if (StellaMultiplayer.IsHost)
					{
                        float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1, 
							ModContent.ProjectileType<Strummer>(), 20, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1, 
							ModContent.ProjectileType<Strummer>(), 20, 0f, Owner: Main.myPlayer);
                    }
					
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Sadano"));

					


				}

				if (NPC.life < NPC.lifeMax / 3)
				{
					if (StellaMultiplayer.IsHost)
					{

						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);

						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
					}
				}

				if (NPC.life < NPC.lifeMax / 4)
				{
					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);

                    }



                }

				if (NPC.life < NPC.lifeMax / 5)
				{

					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
                    }
				}

				if (NPC.life < NPC.lifeMax / 6)
				{

					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
                    }
				}
			}
			
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				
				if (NPC.life < NPC.lifeMax / 2)
				{
					if (NPC.life < NPC.lifeMax / 7)
                    {
                        ResetTimers();
                        State = ActionState.Dienow;
                    }
					if (NPC.life > NPC.lifeMax / 7)
                    {
                        ResetTimers();
                        State = ActionState.SwordUP;
                    }
				}
                else
                {
                    ResetTimers();
                    if (StellaMultiplayer.IsHost)
					{
                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                State = ActionState.TriShot;
                                break;
                            case 1:
                                State = ActionState.SwordUP;
                                break;

                        }
                    }
                }


			




				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}



		private void HoldUPVerliadie()
		{
			timer++;
			if (timer == 2)
			{
				if (NPC.life < NPC.lifeMax / 2)
				{
					if (StellaMultiplayer.IsHost)
					{
                        float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1,
                            ModContent.ProjectileType<Strummer>(), 20, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1,
                            ModContent.ProjectileType<Strummer>(), 20, 0f, Owner: Main.myPlayer);
                    }

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Sadano"));




				}

				if (NPC.life < NPC.lifeMax / 3)
				{

					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
                    }
                }

				if (NPC.life < NPC.lifeMax / 4)
				{

					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                    }
                }

				if (NPC.life < NPC.lifeMax / 5)
				{

					if (StellaMultiplayer.IsHost)
					{
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;

						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
							ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
							ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
                    }
				}

				if (NPC.life < NPC.lifeMax / 6)
				{

					if (StellaMultiplayer.IsHost)
					{
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2,
                            ModContent.ProjectileType<AltideSword>(), 15, 0f, Owner: Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, 
                            ModContent.ProjectileType<AltideSword>(), 12, 0f, Owner: Main.myPlayer);
                    }

				}
			}

			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

						switch (Main.rand.Next(1))
						{
							case 0:
								ResetTimers();
								State = ActionState.SwordUP;
								break;


						}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
		}
			
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
					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2,
						ModContent.ProjectileType<FrostShot2>(), 40, 0f, Owner: Main.myPlayer);
					}
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot2"));
				}
				
				if (timer == 45)
				{
					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2,
                            ModContent.ProjectileType<FrostShot2>(), 40, 0f, Owner: Main.myPlayer);
                    }

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot3"));
				}
			
				if (timer == 65)
                {
					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2,
						ModContent.ProjectileType<FrostShot2>(), 40, 0f, Owner: Main.myPlayer);
					}
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot1"));
				}
				
				
			}

			else
            {
				float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
				if (timer == 15)
                {
					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2,
						ModContent.ProjectileType<FrostShot>(), 43, 0f, Owner: Main.myPlayer);
					}
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot1"));
				}
				
				
				if (timer == 45)
				{
					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2,
                                ModContent.ProjectileType<FrostShot>(), 43, 0f, Owner: Main.myPlayer);
                    }
	
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot3"));
				}
				
			}
			
			if (timer == 88)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.Explode;
						break;
					

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void ExplodeVerlia()
		{
			timer++;
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"));
				if (NPC.life < NPC.lifeMax / 2)
				{
					if (StellaMultiplayer.IsHost)
					{
                        int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());
                        int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 30, ModContent.NPCType<GhostCharger>());
                    }
				}
			}
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.IdleInvis;
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
				if (StellaMultiplayer.IsHost)
                {
                    int distanceY = Main.rand.Next(-125, -125);
					_teleportX = player.Center.X;
                    _teleportY = player.Center.Y + distanceY;
                    NPC.netUpdate = true;
				}			
			}
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.In;
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
				if (StellaMultiplayer.IsHost)
				{
                    int distanceY = Main.rand.Next(-30, -30);
                    _teleportX = player.Center.X;
                    _teleportY = player.Center.Y + distanceY;
                    NPC.netUpdate = true;
                }
			}
			if (timer == 8)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.In;
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void Verliasinsideme()
		{
			timer++;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VDisappear"));
				
			}
			if (timer == 27)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life < NPC.lifeMax / 2)
                {
                    ResetTimers();
                    if (StellaMultiplayer.IsHost)
                    {
                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                State = ActionState.HoldUP;
                                break;


                            case 1:
                                State = ActionState.CutExplode;
                                break;

                        }
                    }
				}

                else
                {
                    ResetTimers();
                    if (StellaMultiplayer.IsHost)
					{
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                State = ActionState.SummonStartup;             
                                break;

                            case 1:
                                State = ActionState.MoonSummonStartup;
                                break;

                            case 2:
                                State = ActionState.CloneSummonStartup;
                                break;

                            case 3:
                                State = ActionState.BigSwordSummonStartup;
                                break;
                        }
                    }
                }
			

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}



		private void CutExplodeVerlia()
		{
			timer++;
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"));
				
			}
				if (timer == 22)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.InvisCut;
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}



		private void SwordUPVerlia()
		{
			timer++;
			if (timer == 4)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"));

			}
				if (timer == 41)
			{
			
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
                        ResetTimers();
                        State = ActionState.SwordSimple;
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
					if (StellaMultiplayer.IsHost)
					{
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                        float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2,
                            ModContent.ProjectileType<SlashRight>(), 32, 0f, Owner: Main.myPlayer);
                    }

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"));
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"));
				}
                else
                {
					if (StellaMultiplayer.IsHost)
					{
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                        float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2,
                            ModContent.ProjectileType<SlashLeft>(), 32, 0f, Owner: Main.myPlayer);
                    }

					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"));
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"));
				}
				
				
				
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
                        ResetTimers();
                        State = ActionState.SwordHold;
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
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, 
						ModContent.ProjectileType<SlashHold>(), 160, 0f, Main.myPlayer, 0f, ai1);
                }

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"));
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
                        ResetTimers();
                        State = ActionState.TriShot;
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManifestedBravery>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StolenMagicTome>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.VerliBossRel>()));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<VerliaBossBag>()));

			LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
				 ModContent.ItemType<VerliaHat>(),
				 ModContent.ItemType<SwordsOfRevengence>(),
				 ModContent.ItemType<SupernovaSitar>(),
				 ModContent.ItemType<HarmonicBlasphemy>(),
				 ModContent.ItemType<Curlistine>()));
	
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PearlescentScrap>(), minimumDropped: 3, maximumDropped: 25));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<VerliaBroochA>()));
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

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Spawn"), NPC.position);
				Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 128f);
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X + 00, (int)NPC.Center.Y + 0, ModContent.NPCType<DeathVerlia>());
				NPC.NewNPC(entitySource, (int)NPC.Center.X + 00, (int)NPC.Center.Y + 0, ModContent.NPCType<VerliaDP>());
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
			}
			else
			{
				for (int k = 0; k < 2; k++)
				{

					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 1.2f);
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), 0.5f);
				}
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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedVeriBoss, -1);
		}
	}
}
