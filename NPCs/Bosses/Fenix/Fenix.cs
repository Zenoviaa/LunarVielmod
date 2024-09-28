using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Igniter;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Melee;
using Stellamod.NPCs.Bosses.Fenix.Projectiles;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.NPCs.Bosses.Fenix
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class Fenix : ModNPC
	{
		public static Vector2 FenixPos;
		public enum ActionState
		{
			StartFen,//
			Startset,//
			IdleFen,//
			IdleFloating,//
			LaughingCircle,//
			LaughFen,//
			SwordsDanceFen,//
			SwordsDanceFen2,//
			SwordsDanceFen3,//
			SwordSlash1st,//
			SwordSlashHalf,//
			SwordSlashHalf2,// goes to 1st
			SwordSlashPhase2,//
			SwirlSwordNeko,
			SwirlSwordYumi,
			SwirlSwordArku,
			Backdown,
			ReadySwordsDance,
			Pause1,//
			Pause2,//
			SwordsSwirlPhase2,//
			ReappearFen,//
			OutSD,
			PreOrderingChildren,
			OutSD2,
			OutSD3,
		}

		// Current state
		private bool _resetTimers;
		private ActionState _state = ActionState.StartFen;
		public ActionState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				if (StellaMultiplayer.IsHost)
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
			Main.npcFrameCount[Type] = 64;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 2;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Fenix/FenixPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Searching for a lover, this particular Queen lacks anyone she can socialize with as she tries to gain power of the void to kill Sigfried for revenge, so much power yet so lonely.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Fenix the Vengeful", "2"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 5, 12));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AuroreanStarI>(), 1, 100, 500));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<STARCORE>(), 1, 2, 3));
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<FenixBag>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<FenixBossRel>()));

            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LittleScissor>(), 1));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Angelenthal>(), chanceDenominator: 15));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Yumiko>(), chanceDenominator: 2));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Nekomara>(), chanceDenominator: 1));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FenixxCard>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FoxMark>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), minimumDropped: 7, maximumDropped: 50));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TomedDustingMagic>(), chanceDenominator: 1));
			npcLoot.Add(notExpertRule);
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(63, 50);
			NPC.damage = 1;
			NPC.defense = 75;
			NPC.lifeMax = 79900;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.value = Item.buyPrice(gold: 20);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.scale = 2f;
			NPC.alpha = 255;
			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<QueenBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/RoyalQueenFenix");
			}
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			return false;
        }

       

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.WriteVector2(dashDirection);
			writer.Write(dashDistance);
			writer.Write((float)State);
			writer.Write(Spawner);
			writer.Write(_resetTimers);

        }

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			dashDirection = reader.ReadVector2();
			dashDistance = reader.ReadSingle();
			State = (ActionState)reader.ReadSingle();
			Spawner = reader.ReadSingle();
			_resetTimers = reader.ReadBoolean();
        }

		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
		public float squish = 0f;
		private int _wingFrameCounter;
		private int _wingFrameTick;
		public bool Wingies = false;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.player[NPC.target];
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			SpriteEffects effects = SpriteEffects.None;

			if (player.Center.X > NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Color drawColors = NPC.GetAlpha(Color.White);
			Rectangle rect;
			originalHitbox = new Vector2(10, 20);


			if (Wingies)
            {
				Vector2 drawPosition = NPC.Center - screenPos;
				Vector2 origin = new Vector2(144, 82);
				Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Fenix/FenixGravWings").Value;
				int wingFrameSpeed = 1;
				int wingFrameCount = 30;
				spriteBatch.Draw(syliaWingsTexture, drawPosition,
					syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
					drawColor, 0f, origin, 1f, effects, 0f);
				
			}
			

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
				//------------------------------------- Fenix duh

				case ActionState.StartFen:
					rect = new(0, 1 * 96, 125, 1 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordsDanceFen:
					rect = new(0, 1 * 96, 125, 1 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordsDanceFen2:
					rect = new(0, 1 * 96, 125, 1 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordsDanceFen3:
					rect = new(0, 1 * 96, 125, 1 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.Startset:
					rect = new(0, 3 * 96, 125, 6 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.ReappearFen:
					rect = new(0, 2 * 96, 125, 1 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.IdleFen:
					rect = new(0, 3 * 96, 125, 6 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.IdleFloating:
					rect = new(0, 3 * 96, 125, 6 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.Pause1:
					rect = new(0, 3 * 96, 125, 6 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.Pause2:
					rect = new(0, 3 * 96, 125, 6 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.LaughFen:
					rect = new(0, 10 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.PreOrderingChildren:
					rect = new(0, 10 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.LaughingCircle:
					rect = new(0, 10 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordSlash1st:
					rect = new(0, 18 * 96, 125, 21 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 21, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 21, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordSlashPhase2:
					rect = new(0, 10 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordsSwirlPhase2:
					rect = new(0, 34 * 96, 125, 13 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordSlashHalf:
					rect = new(0, 18 * 96, 125, 16 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 16, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 16, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwordSlashHalf2:
					rect = new(0, 18 * 96, 125, 16 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 16, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 16, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwirlSwordArku:
					rect = new(0, 40 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwirlSwordNeko:
					rect = new(0, 40 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.SwirlSwordYumi:
					rect = new(0, 40 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.Backdown:
					rect = new(0, 48 * 96, 125, 8 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.OutSD:
					rect = new(0, 48 * 96, 125, 8 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.OutSD2:
					rect = new(0, 48 * 96, 125, 8 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.OutSD3:
					rect = new(0, 48 * 96, 125, 8 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect).Size() / 2, NPC.scale, effects, 0f);
					break;

				case ActionState.ReadySwordsDance:
					rect = new(0, 57 * 96, 125, 7 * 96);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColors, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					break;
			}

			NPC.netUpdate2 = true;
			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame


		int bee = 220;
		private Vector2 originalHitbox;
		public float Spawner = 0;
		public override void AI()
		{
			Spawner++;
			/*
            Player players = Main.player[NPC.target];
            if (Spawner == 2)
            {
                int distanceY = Main.rand.Next(-250, -250);
                NPC.position.X = players.Center.X;
                NPC.position.Y = players.Center.Y + distanceY;
            }*/

			NPC.velocity *= 0.97f;
			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);

			if(NPC.life <= NPC.lifeMax / 2)
			{
				NPC.takenDamageMultiplier = 0.5f;
			}

			FenixPos = NPC.Center;
			if (bee == 0)
			{
				bee = 220;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.Center, RGB.X, RGB.Y, RGB.Z);

			Player player = Main.player[NPC.target];
			NPC.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				// If the targeted player is dead, flee
				NPC.velocity.Y += 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				NPC.alpha++;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(1);

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}


			FinishResetTimers();
			switch (State)
			{
				case ActionState.StartFen:
					NPC.damage = 0;
					
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					StartFen();
					break;
				
				case ActionState.Startset:
					NPC.damage = 0;
					
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					StartSet();
					break;


				case ActionState.ReadySwordsDance:
					NPC.damage = 0;
					
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					Readyswords();
					break;


				case ActionState.SwordsDanceFen:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordsDance();
					NPC.velocity *= 0.99f;
					break;

				case ActionState.SwordsDanceFen2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordsDance2();
					NPC.velocity *= 0.99f;
					break;

				case ActionState.SwordsDanceFen3:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordsDance3();
					NPC.velocity *= 0.99f;
					break;

				case ActionState.Pause1:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					Pause1();
					break;

				case ActionState.Pause2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					Pause2();
					break;

				case ActionState.OutSD:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					OutSD();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.OutSD2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					OutSD2();
					NPC.velocity *= 0.96f;
					break;

				case ActionState.OutSD3:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					OutSD3();
					NPC.velocity *= 0.96f;
					break;

				case ActionState.PreOrderingChildren:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					PreSwords();
					break;

				case ActionState.LaughFen:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					LaughingIdle();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.SwordsSwirlPhase2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordSwirlThree();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.IdleFloating:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					IdleFloating();
					break;

				case ActionState.SwordSlashHalf:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordHalf();
					break;

				case ActionState.SwordSlashHalf2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordHalf2();
					break;

				case ActionState.SwordSlash1st:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordSlashFull();
					break;

				case ActionState.Backdown:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					DownSwirl();
					break;


				case ActionState.IdleFen:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					PostSummon();
					break;

				case ActionState.SwirlSwordArku:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordSwirlGrav();
					NPC.velocity *= 0.98f;
					break;


				case ActionState.SwirlSwordYumi:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordSwirlYumi();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.SwirlSwordNeko:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordSwirlNeko();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.LaughingCircle:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					NPC.velocity *= 0.98f;
					LaughingCircle();
					break;

				case ActionState.SwordSlashPhase2:
					NPC.damage = 0;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SwordsFull2();
					NPC.velocity *= 0.98f;
					break;

				default:
					break;
			}
		}

		private void StartFen()
		{
			timer++;
			Player player = Main.player[NPC.target];
			if (timer == 0)
			{
			
				NPC.alpha = 255;
			}
			if (timer == 1)
			{
				NPC.alpha = 255;

				if (StellaMultiplayer.IsHost)
                {
                    float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + 80, NPC.Center.Y - 40, 0, speedYa * 0, 
					ModContent.ProjectileType<SpawnFen>(), 0, 0f, Owner: Main.myPlayer);
                }
			}

			if (timer == 2)
			{
				if (StellaMultiplayer.IsHost)
                {
                    int distanceY = Main.rand.Next(-150, -150);
                    NPC.position.X = player.Center.X;
                    NPC.position.Y = player.Center.Y + distanceY;
                    NPC.netUpdate = true;
                }

                if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer > 3)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.Startset;
           
            }
		}


		private void StartSet()
		{
			timer++;
			if (timer == 0)
			{
				NPC.alpha = 255;
			}

			if (timer == 1)
			{
				NPC.alpha = 255;
			}

			if (timer < 52)
			{
				NPC.alpha =- 5;
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer > 90)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.ReadySwordsDance;
               
            }
		}


		private void Pause1()
		{
			timer++;
			if (timer == 0)
			{
				NPC.alpha = 255;
			}

			if (timer == 1)
			{
				NPC.alpha = 255;
			}

			if (timer == 35)
			{
				if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, 
						ModContent.ProjectileType<AlcShot>(), 50, 0f, Main.myPlayer, 0f, 0);
				}
			}


			if (timer < 52)
			{
				NPC.alpha = -5;
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer > 53)
			{
                ResetTimers();
                State = ActionState.SwordsDanceFen2;   
            }
		}

		private void Pause2()
		{
			timer++;
			if (timer == 0)
			{
				NPC.alpha = 255;
			}

			if (timer == 1)
			{
				NPC.alpha = 255;
			}

			if (timer == 35)
			{
				if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, 
						ModContent.ProjectileType<AlcShot>(), 50, 0f, Main.myPlayer, 0f, 0);
				}
			}


			if (timer < 52)
			{
				NPC.alpha = -5;
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer > 53)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.SwordsDanceFen3;
               
            }
		}

		private void Readyswords()
		{
			timer++;
			Player player = Main.player[NPC.target];
			if (timer > 1)
			{
				NPC.alpha = 0;
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixReady"), NPC.position);
			}

			if (timer > 70)
			{
                State = ActionState.SwordsDanceFen;
                ResetTimers();
            }
		}

		private void OutSD()
		{
			timer++;
			if (timer > 1)
			{
				NPC.alpha = 0;
			}

			if (timer > 24)
            {
                ResetTimers();
                State = ActionState.IdleFloating;
            }
		}

		private void OutSD2()
		{
			timer++;
			if (timer > 1)
			{
				NPC.alpha = 0;
			}

			if (timer > 24)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.SwordSlashPhase2;
            }
		}

		private void OutSD3()
		{
			timer++;
			if (timer > 1)
			{
				NPC.alpha = 0;
			}

			if (timer > 24)
            {
                ResetTimers();
                State = ActionState.LaughingCircle;
            }
		}

		private void DownSwirl()
		{
			timer++;
			if (timer > 1)
			{
				NPC.alpha = 0;
			}

			if (timer > 24)
            {
                ResetTimers();
                State = ActionState.IdleFen;
            }
		}

		int moveSpeed = 0;
        int moveSpeedY = 0;
		float HomeY = 230f;
		

		private void PreSwords()
		{
			timer++;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixWarn>());
				}
			}


			if (timer > 35)
            {
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
					switch (Main.rand.Next(2))
					{
						case 0:
							State = ActionState.SwordSlash1st;
							break;

						case 1:
							State = ActionState.SwordSlashHalf;
							break;

					}
                }
       
            }
		}

		private void SwordsFull2()
		{
			timer++;
			if (timer == 1)
			{
				if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixWarn>());
				}
			}

			if (timer > 35)
            {
                ResetTimers();
                State = ActionState.SwordSlash1st;       
            }
		}

		private void SwordHalf()
		{
			timer++;
			Player player = Main.player[NPC.target];
			Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
			
			if (timer == 12)
			{
				float numberProjectiles = 1;
				float rotation = MathHelper.ToRadians(30);
			
				for (int i = 0; i < numberProjectiles; i++)
				{
					if (StellaMultiplayer.IsHost)
                    {
                        Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
					}
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));
			}

			if (timer == 36)
			{
				float numberProjectiles = 1;
				float rotation = MathHelper.ToRadians(30);

				for (int i = 0; i < numberProjectiles; i++)
				{
					if (StellaMultiplayer.IsHost)
                    {
                        Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
					}
				}
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash3"));
			}

			if (timer > 64)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            State = ActionState.SwordSlash1st;
                            break;

                        case 1:
                            State = ActionState.SwordSlashHalf2;
                            break;

                    }
                }


            }
		}

		private void SwordHalf2()
		{
			timer++;
			Player player = Main.player[NPC.target];
			Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

			if (timer == 12)
			{
				float numberProjectiles = 2;
				float rotation = MathHelper.ToRadians(30);


				for (int i = 0; i < numberProjectiles; i++)
				{
					if (StellaMultiplayer.IsHost)
                    {
                        Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
					}
				}


				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash3"));
			}

			if (timer == 36)
			{
				float numberProjectiles = 2;
				float rotation = MathHelper.ToRadians(30);

				for (int i = 0; i < numberProjectiles; i++)
				{			
					if (StellaMultiplayer.IsHost)
					{
                        Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
					}
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));
			}


			if (timer > 64)
            {
                ResetTimers();
                State = ActionState.SwordSlash1st;
            }
		}


		private void SwordSlashFull()
		{
			timer++;
			Player player = Main.player[NPC.target];
			Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

			if (timer == 12)
			{
			
				if (NPC.life > NPC.lifeMax / 2)
				{
					float numberProjectiles = 2;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{
						if (StellaMultiplayer.IsHost)
                        {
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}
					}
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));
				}



				if (NPC.life < NPC.lifeMax / 2)
				{
					float numberProjectiles = 1;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{	
						if (StellaMultiplayer.IsHost)
						{
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}
					}
				}
			}

			if (timer == 36)
			{
				if (NPC.life > NPC.lifeMax / 2)
				{
					float numberProjectiles = 3;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{
						if (StellaMultiplayer.IsHost)
                        {
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}
					}
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash3"));
				}

				if (NPC.life < NPC.lifeMax / 2)
				{
					float numberProjectiles = 1;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{
						if (StellaMultiplayer.IsHost)
						{
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}
					}
				}
			}

			if (timer == 72)
			{


				if (NPC.life > NPC.lifeMax / 2)
				{
					float numberProjectiles = 4;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{
						if (StellaMultiplayer.IsHost)
						{
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}

					}
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash2"));
				}

				if (NPC.life < NPC.lifeMax / 2)
				{
					float numberProjectiles = 1;
					float rotation = MathHelper.ToRadians(30);

					for (int i = 0; i < numberProjectiles; i++)
					{
						if (StellaMultiplayer.IsHost)
						{
                            Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<CrileShot>(), 40, 1, Main.myPlayer, 0, 0);
						}
					}
				}
			}

			if (timer > 84)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (NPC.life > NPC.lifeMax / 2)
                {
                    ResetTimers();
                    if (StellaMultiplayer.IsHost)
                    {
						switch (Main.rand.Next(5))
						{
							case 0:
								State = ActionState.SwirlSwordArku;
								break;
							case 1:
								State = ActionState.SwirlSwordYumi;
								break;

							case 2:
								State = ActionState.SwirlSwordYumi;
								break;
							case 3:
								State = ActionState.SwirlSwordNeko;
								break;

							case 4:
								State = ActionState.SwirlSwordNeko;
								break;
						}
                    }
                }

				if (NPC.life < NPC.lifeMax / 2)
				{
					if (Wingies == false)
					{
						if (StellaMultiplayer.IsHost)
						{
                            float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 40, 0, speedYa * 0, ModContent.ProjectileType<SpawnFen>(), 0, 0f, Owner: Main.myPlayer);
                        }

                        Wingies = true;
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixLaugh"), NPC.position);
					}

                    ResetTimers();
                    State = ActionState.SwordsSwirlPhase2;
                }
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void IdleFloating()
		{
			timer++;
			Player player = Main.player[NPC.target];

			if (timer < 120)
            {
				NPC.rotation = NPC.velocity.X * 0.07f;



				if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
					moveSpeed--;
				else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
					moveSpeed++;

				NPC.velocity.X = moveSpeed * 0.10f;

				if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
				{
					moveSpeedY--;
					HomeY = 150f;
				}
				else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
				{

					moveSpeedY++;
				}

				NPC.velocity.Y = moveSpeedY * 0.12f;
			}

			if (timer > 150)
            {
                ResetTimers();
                if (StellaMultiplayer.IsHost)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							State = ActionState.LaughFen;
							break;

						case 1:
							State = ActionState.PreOrderingChildren;
							break;

						case 2:
							State = ActionState.PreOrderingChildren;
							break;
					}
                }
            }
		}

		public float Goth = 0;
		private void PostSummon()
		{
			timer++;
			Goth++;
			Player player = Main.player[NPC.target];
			HomeY = -230f;
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixNeNe"), NPC.position);
			}

			if (Goth == 30)
            {
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(-1, 1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 2, speedYb * 2, ModContent.ProjectileType<FoxRusher>(), 0, 0f, Main.myPlayer, 0f, 0);
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FrostBringer"), NPC.position);


			}

			if (Goth == 130)
            {
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(-1, 1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 2, speedYb * 2, ModContent.ProjectileType<FoxRusher>(), 0, 0f, Main.myPlayer, 0f, 0);
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FrostBringer"), NPC.position);
			}

			if (Goth == 330)
            {
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(-1, 1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb * 2, speedYb * 2, ModContent.ProjectileType<FoxRusher>(), 0, 0f, Main.myPlayer, 0f, 0);
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FrostBringer"), NPC.position);
			}

			if (timer < 320)
			{
				if (NPC.Center.X >= player.Center.X && moveSpeed >= -120) // flies to players x position
					moveSpeed--;
				else if (NPC.Center.X <= player.Center.X && moveSpeed <= 120)
					moveSpeed++;

				NPC.velocity.X = moveSpeed * 0.10f;

				if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -20) //Flies to players Y position
				{
					moveSpeedY--;
					HomeY = 200f;
				}
				else if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 20)
				{
					moveSpeedY++;
				}

				NPC.velocity.Y = moveSpeedY * 0.12f;
			}


			//NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixWarn>());
			if (timer > 360)
            {
                ResetTimers();
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                if (StellaMultiplayer.IsHost)
				{
					switch (Main.rand.Next(4))
					{
						case 0:
							State = ActionState.LaughFen;
							break;

						case 1:
							State = ActionState.IdleFloating;
							break;

						case 2:
							State = ActionState.PreOrderingChildren;
							break;

						case 3:
							State = ActionState.ReadySwordsDance;
							break;
					}
                }

         
            }
		}


		private void LaughingIdle()
		{
			timer++;
			Player player = Main.player[NPC.target];

			if (timer == 1)
            {
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixFun"), NPC.position);
			}

			if (timer > 35)
            {
                ResetTimers();
                if (StellaMultiplayer.IsHost)
                {
                    switch (Main.rand.Next(1))
                    {
                        case 0:
                            State = ActionState.PreOrderingChildren;
                            break;
                    }
                }
            }
		}

		private void SwordSwirlGrav()
		{
			timer++;
			Player player = Main.player[NPC.target];


			float ai1 = NPC.whoAmI;

			if (timer == 1)
			{
				var entitySource = NPC.GetSource_FromThis();
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DrownOut>());
				}
			}

			if (timer == 30)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSummonGrav"), NPC.position);
				var entitySource = NPC.GetSource_FromThis();
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ALCADHOLE>());
				}
			}

			if (timer > 35)
            {
                ResetTimers();
                State = ActionState.Backdown;
            }
		}


		private void SwordSwirlThree()
		{
			timer++;
			Player player = Main.player[NPC.target];


			float ai1 = NPC.whoAmI;

			if (timer == 1)
			{

				if (Wingies == false)
				{
					if (StellaMultiplayer.IsHost)
					{
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 40, 0, speedYa * 0, ModContent.ProjectileType<SpawnFen>(), 0, 0f, Owner: Main.myPlayer);
					}
					Wingies = true;
					SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixLaugh"), NPC.position);
				}


				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixNeNe"), NPC.position);



			}

			if (timer == 60)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashCharge"), NPC.position);
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(-1, 1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 400, speedXb * 2, speedYb * 2, ModContent.ProjectileType<Angelu>(), 0, 0f, Main.myPlayer, 0f, 0);
				}
			}

			//NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixWarn>());
			if (timer > 65)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.OutSD3;
                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
            }

		}

		public bool canhitplayer = false;
		Vector2 BaseVel = Vector2.UnitX;
		public float Grimber = 0;

		private void LaughingCircle()
		{
			timer++;
			Grimber++;
			Player player = Main.player[NPC.target];
			Player target = Main.player[NPC.target];

			if (timer == 140)
            {
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixLaugh"), NPC.position);
			}

			if (timer < 30 && NPC.HasValidTarget)
            {
                Vector2 targetCenter = target.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -256);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                NPC.netUpdate = true;

                float hoverSpeed = 5;
				float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
				NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }

			if (Grimber == 60)
			{
				Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				float numberProjectiles = 1;
				float rotation = MathHelper.ToRadians(30);

				for (int i = 0; i < numberProjectiles; i++)
				{
					Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<NiceBuster>(), 50, 1, Main.myPlayer, 0, 0);
					}
				}

				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1"));
				Grimber = 0;
			}


			NPC.noTileCollide = true;
			NPC.noGravity = true;
			float targetrotation;
			if (timer > 30)
			{
				if (timer < 120 && timer > 30)
				{ //fly away from player until attack starts
					NPC.spriteDirection = NPC.direction;
					targetrotation = NPC.AngleTo(player.Center);
					if (Math.Abs(targetrotation) > MathHelper.PiOver2)
						targetrotation -= MathHelper.Pi;
					NPC.rotation = Utils.AngleLerp(NPC.rotation, targetrotation, 0.1f);
					NPC.velocity = (NPC.Distance(player.Center) < 100) ? Vector2.Lerp(NPC.velocity, NPC.DirectionFrom(player.Center) * 4, 0.06f) :
						(NPC.Distance(player.Center) > 200) ? Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center) * 3, 0.05f) : Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.05f);
				}
				else
				{

					canhitplayer = true;
					targetrotation = NPC.velocity.ToRotation();
					if (NPC.spriteDirection < 0)
						targetrotation -= MathHelper.Pi;

					int numwaves = (Main.expertMode) ? 2 : 1;
					if (timer == 600)
					{ //when the spin starts, save the initial velocity of the spin to rotate each tick, and store the player's center and a random spot far away from them
						BaseVel = Vector2.UnitX.RotatedBy(NPC.rotation) * NPC.spriteDirection * 2;
					}
					if (BaseVel.Length() < 40)
						BaseVel *= 1.005f;

					NPC.rotation = targetrotation;
					NPC.velocity = BaseVel.RotatedBy(MathHelper.ToRadians((timer - 600) * 4));
				}


				if (timer > 620)
                {
                    ResetTimers();
                    State = ActionState.ReadySwordsDance;
                }
			}
		}

		private void SwordSwirlYumi()
		{
			timer++;
			Player player = Main.player[NPC.target];

			float ai1 = NPC.whoAmI;
			if (timer == 30)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashCharge"), NPC.position);
				var entitySource = NPC.GetSource_FromThis();
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(entitySource, (int)NPC.Center.X + 150, (int)NPC.Center.Y - 100, ModContent.NPCType<ExpoiyosOrb>());
					NPC.NewNPC(entitySource, (int)NPC.Center.X - 150, (int)NPC.Center.Y - 100, ModContent.NPCType<ExpoiyosOrb>());
				}
			}

			if (timer > 35)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                ResetTimers();
                State = ActionState.Backdown;
            }
		}

		private void SwordSwirlNeko()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float ai1 = NPC.whoAmI;

			if (timer < 30)
			{
				for (int i = 0; i < 4; i++)
				{
					Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(256, 256);
					Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * 16;
					Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
						default(Color), 1 / 3f);
					p.layer = Particle.Layer.BeforeProjectiles;
				}
			}

			if (timer == 30)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashCharge"), NPC.position);
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 100, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Aldox>(), 40, 0f, Main.myPlayer, 0f, 0);
				}
			}

			if (timer > 35)
            {
                ResetTimers();
                State = ActionState.Backdown;
            }
		}

		private void SwordsDance2()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float ai1 = NPC.whoAmI;
			float speed = 1;
			if (NPC.life < NPC.lifeMax / 2)
			{
				speed = 18f;
			}
			if (NPC.life > NPC.lifeMax / 2)
			{
				speed = 12f;
			}

			
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSwordsDance2"), NPC.position);
				NPC.alpha = 255;
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Slashers>(), 240, 0f, Main.myPlayer, 0f, ai1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixHold>(), 240, 0f, Main.myPlayer, 0f, ai1);

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}


					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}


			if (timer == 11)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}



			if (timer == 21)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}

					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}

			if (timer == 31)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}


			if (timer == 41)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}

					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}

			if (timer == 51)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}

			if (timer == 61)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}


			if (timer < 80)
			{
				if (StellaMultiplayer.IsHost)
				{
                    int distance = Main.rand.Next(2, 2);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (player.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }

			if (timer == 90)
            {
                ResetTimers();
                State = ActionState.Pause2;
            }
		}

		private void SwordsDance()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float ai1 = NPC.whoAmI;
			float speed = 1;
			if (NPC.life < NPC.lifeMax / 2)
			{
				speed = 16f;
			}
			if (NPC.life > NPC.lifeMax / 2)
			{
				speed = 9f;
			}

			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSwordsDance1"), NPC.position);
				NPC.alpha = 255;
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Slashers>(), 200, 0f, Main.myPlayer, 0f, ai1);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixHold>(), 200, 0f, Main.myPlayer, 0f, ai1);

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}


			if (timer == 11)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}



			if (timer == 21)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}

			if (timer == 31)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}


			if (timer == 41)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}
				}
			}

			if (timer == 51)
			{
				if (StellaMultiplayer.IsHost)
				{
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

					switch (Main.rand.Next(5))
					{
						case 0:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 1:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 2:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 3:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;

						case 4:
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
							break;
					}

					if (NPC.life < NPC.lifeMax / 2)
					{
						var entitySource = NPC.GetSource_FromThis();
						NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
					}
				}
			}

			if (timer == 61)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 1:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 2:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 3:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 4:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;
                    }
                }
			}




			if (timer < 70)
			{
				if (StellaMultiplayer.IsHost)
				{
                    int distance = Main.rand.Next(2, 2);
                    NPC.ai[3] = Main.rand.Next(1);
					NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (player.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }

			if (timer == 75)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                State = ActionState.Pause1;
                ResetTimers();
            }
		}

		private void SwordsDance3()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float ai1 = NPC.whoAmI;
			float speed = 1;
			if (NPC.life < NPC.lifeMax / 2)
			{
				speed = 26f;
			}
			if (NPC.life > NPC.lifeMax / 2)
			{
				speed = 18f;
			}
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordHoldVerlia"), NPC.position);
				NPC.alpha = 255;

				if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Slashers>(), 200, 0f, Main.myPlayer, 0f, ai1);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixHold>(), 200, 0f, Main.myPlayer, 0f, ai1);

                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 1:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 2:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 3:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 4:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;
                    }
                }	
			}

			if (timer == 11)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 1:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 2:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 3:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 4:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;
                    }

                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        var entitySource = NPC.GetSource_FromThis();
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
                    }
                }
			}

			if (timer == 21)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 1:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 2:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 3:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 4:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;
                    }
                }
			}

			if (timer == 31)
			{
				if (StellaMultiplayer.IsHost)
				{
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade1>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 1:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade2>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 2:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade4>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 3:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade5>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;

                        case 4:
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FenixBlade6>(), 0, 0f, Main.myPlayer, 0f, ai1);
                            break;
                    }

                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        var entitySource = NPC.GetSource_FromThis();
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FenixSnipe>());
                    }
                }			
			}

			if (timer < 35)
			{
				if (StellaMultiplayer.IsHost)
				{
                    int distance = Main.rand.Next(2, 2);
                    NPC.ai[3] = Main.rand.Next(1);
					NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (player.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
                //NPC.netUpdate = true;
            }

			if (timer == 45)
			{
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                if (NPC.life < NPC.lifeMax / 2)
				{
					if (Wingies == false)
					{
						if (StellaMultiplayer.IsHost)
						{
                            float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 40, 0, speedYa * 0, ModContent.ProjectileType<SpawnFen>(), 0, 0f, Owner: Main.myPlayer);
                        }

						Wingies = true;
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixLaugh"), NPC.position);
					}

                    ResetTimers();
                    State = ActionState.OutSD2;
				}
				else
				{
                    ResetTimers();
                    State = ActionState.OutSD;
                }
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
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedFenixBoss, -1);
			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}
		}
	}
}
