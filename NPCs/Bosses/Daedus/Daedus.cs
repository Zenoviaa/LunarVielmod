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

namespace Stellamod.NPCs.Bosses.Daedus
{
	[AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class Daedus : ModNPC
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
			

			StartGintze,
			Slammer,
			Rulse,
			Jumpstartup,
			Jumpin,
			HandsNRun,
			Stop,
			Fallin,


			Idle,
			Summonstart,
			HandsoutVoid,
			HandsoutFlametornado,
			HandsoutLantern,
			HandsoutAxe,
			HandsIn,
			Stopp,


		}
		// Current state

		public ActionState State = ActionState.Jumpstartup;
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

			Main.npcFrameCount[Type] = 42;

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
				CustomTexturePath = "Stellamod/NPCs/Bosses/Daedus/GintziaPreview",
				PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
				PortraitPositionYOverride = 0f,

			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
		{
			NPC.Size = new Vector2(120, 74);
			NPC.damage = 1;
			NPC.defense = 10;
			NPC.lifeMax = 2000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 40);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;
			NPC.BossBar = ModContent.GetInstance<DaedusBossBar>();







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
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gintzicane");
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A puppet, a guardian, and a follower of Gothivia, one of her finest creations")
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

			if (player.Center.X > NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}



			Rectangle rect;
			originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) - new Vector2(0, 68);

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
				case ActionState.Slammer:
					rect = new(0, 9 * 74, 42, 3 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Jumpin:
					rect = new(0, 5 * 67, 42, 3 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 20, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;


				case ActionState.Fallin:
					rect = new(0, 8 * 67, 42, 1 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 80, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Jumpstartup:
					rect = new Rectangle(0, 1 * 67, 42, 3 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 3, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Stop:
					rect = new Rectangle(0, 0 * 67, 42, 1 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 50, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Rulse:
					rect = new Rectangle(0, 13 * 67, 42, 16 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 16, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.StartGintze:
					rect = new Rectangle(0, 0 * 67, 42, 1 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 50, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsNRun:
					rect = new Rectangle(0, 30 * 67, 42, 12 * 67);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 12, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;





				case ActionState.Idle:
					rect = new(0, 1 * 74, 120, 30 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 30, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Summonstart:
					rect = new(0, 31 * 74, 120, 10 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 10, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsoutAxe:
					rect = new(0, 41 * 74, 120, 1 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsoutFlametornado:
					rect = new(0, 41 * 74, 120, 1 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsoutLantern:
					rect = new(0, 41 * 74, 120, 1 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsoutVoid:
					rect = new(0, 41 * 74, 120, 1 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.HandsIn:
					rect = new(0, 42 * 74, 120, 4 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 4, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;

				case ActionState.Stopp:
					rect = new(0, 31 * 74, 120, 1 * 74);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
					break;
			}


			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame


		int bee = 220;
		private Vector2 originalHitbox;
		int moveSpeed = 0;
		int moveSpeedY = 0;

		public override void AI()
		{
			
			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);




			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];

			NPC.TargetClosest();

			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest();
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
				case ActionState.Idle:
					NPC.damage = 0;
					counter++;
					NPC.noGravity = true;
					IdleDaed();
					NPC.aiStyle = -1;
					break;

				case ActionState.Summonstart:
					NPC.damage = 0;
					counter++;
					NPC.noGravity = true;
					SummonDaed();
					NPC.aiStyle = -1;
					break;

				case ActionState.HandsoutVoid:
					NPC.damage = 0;
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

				//	SummonVoid();
					NPC.aiStyle = -1;
					break;

				case ActionState.HandsoutLantern:
					NPC.damage = 0;
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

					SummonLantern();
					NPC.aiStyle = -1;
					break;

				case ActionState.HandsoutFlametornado:
					NPC.damage = 0;
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

				//	SummonTornado();
					NPC.aiStyle = -1;
					break;

				case ActionState.HandsoutAxe:
					NPC.damage = 0;
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

				//	SummonAxe();
					NPC.aiStyle = -1;
					break;

				case ActionState.HandsIn:
					NPC.damage = 0;
					counter++;
					NPC.noGravity = true;
					UnSummon();
					NPC.aiStyle = -1;
					break;

				case ActionState.Stopp:
					NPC.damage = 0;
					counter++;
					NPC.noGravity = true;
					Chill();
					NPC.aiStyle = -1;
					break;


				////////////////////////////////////////////////////////////////////////////////////
				///

			}
		}

	

		private void IdleDaed()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			
			


			if (timer == 400)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Stopp;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void Chill()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;

			NPC.velocity *= 0.98f;



			if (timer == 40)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Summonstart;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void SummonDaed()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;

			NPC.velocity *= 0.98f;


			if (timer == 40)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						State = ActionState.HandsoutLantern;
						ResetTimers();
						break;
					case 1:
						State = ActionState.HandsoutLantern;
						ResetTimers();
						break;
					case 2:
						State = ActionState.HandsoutLantern;
						ResetTimers();
						break;
					case 3:
						State = ActionState.HandsoutLantern;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void SummonLantern()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;

			
			if (timer == 0)
            {
				//var entitySource = NPC.GetSource_FromThis();
				//NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<GintziaHand>());

			}

			if (timer == 40)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.HandsIn;
						ResetTimers();
						break;
	

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void UnSummon()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			

			if (timer == 24)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.Idle;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}








		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

			// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
			//	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




			// ItemDropRule.MasterModeCommonDrop for the relic

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GintziaBossBag>()));
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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedGintzlBoss, -1);
			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}

		}

	}
}
