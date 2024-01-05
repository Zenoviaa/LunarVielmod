using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Mage;
using Stellamod.NPCs.Bosses.STARBOMBER.Projectiles;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles.Sword;
using Stellamod.NPCs.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Stellamod.WorldG;
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
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Bosses.STARBOMBER
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
	public class STARBOMBER : ModNPC
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
			BIGLand,






			StartStar,
			IdleStar,
			GunStar,
			BomberStar,
			BreakdownStar,
			SpinStar,
			SpinVerticleStar,
			SpinGroundStar,
			DisappearStar,
			TeleportStar,
			DropdownSpinStar,
			RageSpinStar,
			PullInStar,
			LaserdrillStar,
			WaitStar,
			FallStar,

		}
		// Current state

		public ActionState State = ActionState.StartStar;
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

			Main.npcFrameCount[Type] = 62;

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
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
			drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/STARBOMBER/STARBOMBERPreview";
			drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
			drawModifiers.PortraitPositionYOverride = 0f;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 3));
		
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AuroreanStarI>(), 1, 20, 100));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<STARCORE>(), 1, 1, 2));

			switch (Main.rand.Next(3))
			{
				case 0:
					npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FurihaMKIII>(), 1, 1));
					break;

				case 1:
					npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 3));
					break;


				case 2:
					npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StarSilk>(), 1, 1, 40));
					npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), 1, 2, 5));
					break;


			}

			
			
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(96, 65);
			NPC.damage = 1;
			NPC.defense = 40;
			NPC.lifeMax = 10500;
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
			NPC.alpha = 255;
			NPC.aiStyle = 0;






			// Take up open spawn slots, preventing random NPCs from spawning during the fight

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			NPC.aiStyle = -1;

			// Custom boss bar
			NPC.BossBar = ModContent.GetInstance<AlcaBossBar>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Boss6");
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Destroy these things immediately. We do not need this world being annihalated.")
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
		public float squish = 0f;
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Player player = Main.player[NPC.target];

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

			SpriteEffects effects = SpriteEffects.None;


			Vector2 spritesquish = new(1 - squish, 1 + squish);
			

			Rectangle rect;
			originalHitbox = new Vector2(0, 60);

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
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.BeggingingMoonStart:
					rect = new(0, 1 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.SummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.BigSwordSummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.MoonSummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.CloneSummonStartup:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.SummonIdle:
					rect = new Rectangle(0, 3 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;


				case ActionState.Unsummon:
					rect = new Rectangle(0, 8 * 92, 133, 4 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 4, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.HoldUP:
					rect = new Rectangle(0, 12 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.SwordUP:
					rect = new Rectangle(0, 20 * 92, 133, 11 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 11, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.SwordSimple:
					rect = new(0, 31 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 5, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;


				case ActionState.SwordHold:
					rect = new(0, 36 * 92, 133, 10 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 10, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;


				case ActionState.TriShot:
					rect = new(0, 46 * 92, 133, 22 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 22, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.Explode:
					rect = new(0, 68 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.CutExplode:
					rect = new(0, 70 * 92, 133, 6 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.In:
					rect = new(0, 76 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 5, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.IdleInvis:
					rect = new(0, 74 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.InvisCut:
					rect = new(0, 74 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.Dienow:
					rect = new(0, 1 * 92, 133, 1 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.SummonBeamer:
					rect = new Rectangle(0, 1 * 92, 133, 7 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				case ActionState.idleSummonBeamer:
					rect = new Rectangle(0, 3 * 92, 133, 5 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;


				case ActionState.HoldUPdie:
					rect = new Rectangle(0, 12 * 92, 133, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 8, rect), drawColor, 0f, Vector2.Zero, NPC.scale, effects, 0f);
                    NPC.netUpdate = true;
                    break;

				//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~





				case ActionState.StartStar:
					rect = new(0, 1 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.IdleStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.GunStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.BomberStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.BreakdownStar:
					rect = new(0, 29 * 129, 206, 26 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 26, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 26, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.SpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.DropdownSpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.SpinGroundStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.RageSpinStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.PullInStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.LaserdrillStar:
					rect = new(0, 1 * 129, 206, 28 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 28, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.SpinVerticleStar:
					rect = new(0, 55 * 129, 206, 7 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 1, 7, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;

				case ActionState.WaitStar:
					rect = new(0, 55 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;


				case ActionState.FallStar:
					rect = new(0, 1 * 129, 206, 1 * 129);
					spriteBatch.Draw(texture, NPC.Center - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect), drawColor, 0f, texture.AnimationFrame(ref frameCounter, ref frameTick, 800, 1, rect).Size() / 2, NPC.scale, effects, 0f);
					NPC.netUpdate = true;
					break;


			}


			return false;
		}

		//Custom function so that I don't have to copy and paste the same thing in FindFrame
		

        int bee = 220;
		private Vector2 originalHitbox;
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			bool npcAlreadyExists = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.type == ModContent.NPCType<STARBOMBER>())
				{
					npcAlreadyExists = true;
					break;
				}
			}

			//Don't spawn the npc if it already exists
			if (npcAlreadyExists)
			{
				return 0f;
			}

			//If any player is underground and has an example item in their inventory, the example bone merchant will have a slight chance to spawn.

			if (EventWorld.Aurorean && Main.hardMode)
			{

				return 0f;

			}

			//Else, the example bone merchant will not spawn if the above conditions are not met.
			return 0f;
		}


		public override void AI()
		{
			NPC.velocity *= 0.97f;
			bee--;
			//Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);

			NPC.HasBuff<Starbombin>();


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
				NPC.velocity.Y += 0.5f;
				NPC.noTileCollide = true;
				NPC.noGravity = false;
				NPC.alpha++;
				// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
				NPC.EncourageDespawn(2);

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
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
				//////////////////////////////////////////////////////////////////////////////////////////////
				///





				case ActionState.BomberStar:
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
					BombStar();
					break;



				case ActionState.StartStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					StartStar();
					break;

				case ActionState.IdleStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					IdleStar();
					break;

				case ActionState.BreakdownStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					BreakdownStar();
					break;

				case ActionState.GunStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					GUNSTAR();
					break;

				case ActionState.FallStar:
					NPC.damage = 0;
					counter++;

					
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					FallStar();
					break;

				case ActionState.LaserdrillStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = false;
					NPC.noGravity = false;
					LaserStar();
					break;

				case ActionState.SpinStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinStar();
					break;

				case ActionState.SpinGroundStar:
					NPC.damage = 0;
					counter++;
					NPC.velocity.X *= 0f;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinGroundStar();
					break;

				case ActionState.SpinVerticleStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					SpinVerticleStar();
					break;

				case ActionState.RageSpinStar:
					NPC.damage = 100;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;


					
					TeleportRageStar();
					break;






				case ActionState.TeleportStar:
					NPC.damage = 0;
					counter++;
					
					TeleportStar();
					break;

				case ActionState.WaitStar:
					NPC.damage = 0;
					counter++;
					NPC.noTileCollide = true;
					NPC.noGravity = true;
					WaitStar();
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

		private void StartStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			if (timer == 0)
			{
				NPC.scale = 0f;
				NPC.alpha = 255;
			}
			if (timer == 1)
            {
				NPC.scale = 0f;
				NPC.alpha = 255;
            }
			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-150, -150);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + distanceY;


				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

			if (timer < 101)
            {
				NPC.scale += 0.02f;
				NPC.alpha--;
            }

			if (timer == 50)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARBOMBERWAKE"));
				
			}
			if (timer > 130)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						State = ActionState.IdleStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.IdleStar;
						ResetTimers();
						break;

					case 2:
						State = ActionState.IdleStar;
						ResetTimers();
						break;
					case 3:
						State = ActionState.IdleStar;
						ResetTimers();
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void IdleStar()
		{
			timer++;


			if (timer > 112)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						State = ActionState.BomberStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.BreakdownStar;
						ResetTimers();
						break;

					case 2:
						State = ActionState.GunStar;
						ResetTimers();
						break;
					case 3:
						State = ActionState.LaserdrillStar;
						ResetTimers();
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void Disappear()
		{

			Player obj = Main.player[NPC.target];
			NPC.velocity.Y += 0.1f;
			NPC.scale -= 0.01f;
			if (NPC.scale <= 0)
			{
				NPC.active = false;
			}
			NPC.netUpdate = true;
		}


		private void GUNSTAR()
		{
			timer++;

			if (timer == 2)
			{

				float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);


			//	int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 2, (int)NPC.Center.Y - 100, ModContent.NPCType<STARBOMBERGUN>());

				float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + speedYa + 110, 0, speedYa - 1 * 3, ModContent.ProjectileType<STARBOMBERGUN2>(), 5, 0f, 0, 0f, 0f);


			}
				if (timer > 560)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.BomberStar;
						ResetTimers();
						break;
					case 1:
						State = ActionState.BreakdownStar;
						ResetTimers();
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		 
		private void LaserStar()
		{
			timer++;

			if (timer == 2)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARNBIG>());


			}

			if (timer < 180)
            {
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player npc = Main.player[i];

					if (npc.active)
					{
						float distance = Vector2.Distance(NPC.Center, npc.Center);
						if (distance <= 4000)
						{
							Vector2 direction = npc.Center - NPC.Center;
							direction.Normalize();
							npc.velocity -= direction * 0.5f;
						}
					}
				}


			}
			if (timer > 360)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}


				switch (Main.rand.Next(3))
				{
					case 0:
						State = ActionState.BreakdownStar;
						ResetTimers();
						break;
					case 1:
						State = ActionState.GunStar;
						ResetTimers();
						break;

					case 2:
						State = ActionState.BomberStar;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		

		}

		private void BombStar()
		{
			timer++;

			if (timer == 2)
			{



				//	int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 2, (int)NPC.Center.Y - 100, ModContent.NPCType<STARBOMBERGUN>());
				var entitySource = NPC.GetSource_FromThis();
				int index = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<STARLINGBIG>());
				NPC minionNPC = Main.npc[index];
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARIGNITE"));
			}
			if (timer > 360)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}


				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.BreakdownStar;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

			

		}


		private void BreakdownStar()
		{
			timer++;


			if (timer == 26)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.SpinGroundStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.SpinStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void WaitStar()
		{
			timer++;

			if (timer == 5)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}

			if (timer == 90)
            {
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARWAVE"));
			}
			if (timer > 120)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{


					case 0:
						State = ActionState.SpinStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void FallStar()
		{
			timer++;

			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}
			NPC.noTileCollide = false;
			NPC.noGravity = false;
			NPC.damage = 0;

			Player player = Main.player[NPC.target];



			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-150, -150);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + distanceY;
			
            

            }


			if (timer < 50)
            {
				NPC.velocity.Y += 0.4f;
            }
			if (timer > 120)
			{
				NPC.scale += 0.02f;

			}

			if (timer > 220)
			{
				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 2f;
				switch (Main.rand.Next(1))
				{


					case 0:
						State = ActionState.IdleStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}
		private void TeleportStar()
		{
			timer++;

			NPC.scale -= 0.01f;
			NPC.velocity *= 0.96f;
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));

			}



			if (timer > 201)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 0f;

				for (int j = 0; j < 25; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<AVoidParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.FallStar;
						ResetTimers();
						break;

					case 1:
						State = ActionState.RageSpinStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		public float Voiden = 0;
		public float missue = 0;
		private void TeleportRageStar()
		{
			timer++;
			Voiden++;
			missue++;
			Player player = Main.player[NPC.target];
			NPC.velocity *= 0.96f;
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARLAUGH"));
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARWAVE"));
			}

			if (timer < 201)
            {
				NPC.scale += 0.02f;
			
            }

			if (timer == 2)
			{
				int distanceY = Main.rand.Next(-600, -600);
				NPC.position.X = player.Center.X;
				NPC.position.Y = player.Center.Y + distanceY;

			}

			if (Voiden == 5)
            {
			
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-20f, 20f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-20, 20);
				//Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y , speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcaricMushBoom>(), 30, 0f, 0, 0f, 0f);

				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2Circular(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<ShadeParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				Voiden = 0;
			}


			if (missue == 25)
            {

				float speedXa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
				float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + speedXa, NPC.position.Y + speedYa + 110, speedXa, speedYa - 1 * 1, ProjectileID.SaucerMissile, 25, 0f, 0, 0f, 0f);

				for (int j = 0; j < 30; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				missue = 0;
            }


		

			float speed = 8f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180))) * 0;
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer < 302 && timer > 201)
            {
				NPC.scale -= 0.02f;

				
			}

			if (timer > 303)
			{
			
				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<AVoidParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.scale = 0f;
				switch (Main.rand.Next(1))
				{


					case 0:
						State = ActionState.FallStar;
						ResetTimers();
						break;



				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void SpinGroundStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float speed = 14f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = 0;
				Vector2 angle = new Vector2((float)anglex, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				dashDirection.Y = 0;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer == 5)
            {

				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
			}


			if (timer == 25)
			{

				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
			}

			if (timer == 45)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
			}

			if (timer == 55)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
			}

			if (timer == 75)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
			}

			if (timer == 95)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AssassinsKnifeHit"));
				var entitySource = NPC.GetSource_FromThis();
				NPC.NewNPC(entitySource, (int)NPC.Center.X - 10, (int)NPC.Center.Y - 200, ModContent.NPCType<STARBOMBERLASERWARN>());
			}

			if (timer > 110)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(3))
				{
					case 0:
						State = ActionState.SpinGroundStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.SpinStar;
						ResetTimers();
						break;

					case 2:
						State = ActionState.SpinVerticleStar;
						ResetTimers();
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void SpinVerticleStar()
		{
			timer++;
			Player player = Main.player[NPC.target];
			float speed = 14f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			if (timer < 75)
			{

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180))) ;
				Vector2 angle = new Vector2((float)0, (float)angley);
				dashDirection = (player.Center - (angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				dashDirection.X = 0;
				NPC.velocity = dashDirection;
				ShakeModSystem.Shake = 3;
			}

			if (timer == 5)
			{

				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
			}


		

			if (timer == 45)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
			}

			

			if (timer == 75)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
			}

			if (timer == 105)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/gun1"));
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SINESTAR>(), 50, 0f, 0, 0f, 0f);
			}


			if (timer > 180)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.WaitStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.WaitStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		public float spinst = 0;
		public float constshoot = 0;
		private void SpinStar()
		{
			timer++;
			spinst++;
			constshoot++;

		
			Player player = Main.player[NPC.target];
		
			NPC.noTileCollide = true;
			NPC.noGravity = true;

			float speed = 8f;

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
			if (spinst < 60)
            {
				 speed = 8f;

				
			}

			if (spinst < 100 && spinst > 60)
			{
				speed = 12f;

				
			}


			if (spinst < 180 && spinst > 100)
			{
				 speed = 8f;

				
			}

			if (spinst < 240 && spinst > 180)
			{
				 speed = 12f;

				
			}



			if (constshoot == 70)
            {
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
				float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
				float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 100, NPC.position.Y + speedYa, speedXa * 5, speedYa - 1 * 0, ModContent.ProjectileType<STRIKEBULLET2>(), 40, 0f, 0, 0f, 0f);

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + 70, speedXa * -5, speedYa - 1 * 0, ModContent.ProjectileType<STRIKEBULLET>(), 40, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SunStalker_Bomb_2"));

				constshoot = 0;

				float speedXaz = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
				float speedYaz = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-10, 10);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + speedXaz, NPC.position.Y + speedYaz + 110, speedXaz * 0, speedYaz - 1 * 1, ProjectileID.ShadowBeamHostile, 25, 0f, 0, 0f, 0f);

			}





			if (timer > 240)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.


				for (int j = 0; j < 50; j++)
				{
					Vector2 speedg = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(NPC.Center, speedg * 7, ParticleManager.NewInstance<AVoidParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));


				}

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.TeleportStar;
						ResetTimers();
						break;


					case 1:
						State = ActionState.TeleportStar;
						ResetTimers();
						break;


				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}






















		//-------------------------------------------------------------------------------------------------
		private void StartVerlia()
		{
			timer++;
			if (timer == 2)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						State = ActionState.SummonStartup;
						ResetTimers();
						break;


					case 1:
						State = ActionState.MoonSummonStartup;
						ResetTimers();
						break;

					case 2:
						State = ActionState.CloneSummonStartup;
						ResetTimers();
						break;
					case 3:
						State = ActionState.BigSwordSummonStartup;
						ResetTimers();
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void Dienow()
		{
			timer++;
			if (timer == 2)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.SummonBeamer;
						ResetTimers();
						break;



				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void MoonStartVerlia()
		{
			timer++;
			if (timer == 2)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.MoonSummonStartup;
						ResetTimers();
						break;



				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}

		private void SummonBeamer()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{


				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 0, 0f, 0, 0f, 0f);
			}
			
						if (timer == 6)
						{
							GeneralStellaUtilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y + 1000, 0, -10, ModContent.ProjectileType<VRay>(), 600, 0f, -1, 0, NPC.whoAmI);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/AbsoluteDistillence"));
						}


						


						//case 2:

						//	break;
				
			
			if (timer == 110)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(1))
				{
					case 0:
						State = ActionState.idleSummonBeamer;
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
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 0, 0f, 0, 0f, 0f);
			}
			if (timer > 5)
			{
				switch (Main.rand.Next(1))
				{
					case 0:
						
							if (timer == 6)
							{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"));
							float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
								float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), 5, 0f, 0, 0f, 0f);
							}

							if (timer == 20)
							{
								float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
								float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 80, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), 5, 0f, 0, 0f, 0f);


							}

							if (timer == 25)
							{
								float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
								float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 120, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword1F>(), 7, 0f, 0, 0f, 0f);

							}

							if (timer == 35)
							{
								float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 140, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), 7, 0f, 0, 0f, 0f);

							}

							if (timer == 45)
							{
								float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
								float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 170, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), 7, 0f, 0, 0f, 0f);

							}

							if (timer == 54)
							{
								float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
								float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 200, NPC.position.Y + speedY - 130, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Sword2F>(), 7, 0f, 0, 0f, 0f);
								SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"));
						}
						

						break;











					case 1:
				
							float speedXd = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedYd = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXd + 10, NPC.position.Y + speedYd - 130, speedXd - 2 * 2, speedYd - 2 * 2, ModContent.ProjectileType<TheMoon>(), 50, 0f, 0, 0f, 0f);

						
						break;













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

		private void MoonStartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{


				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 0, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StarCharge"));
			}
			if (timer > 5)
			{
				switch (Main.rand.Next(1))
				{
	
					case 0:
						if (timer == 6)
						{
							float speedXd = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedYd = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXd + 10, NPC.position.Y + speedYd - 130, speedXd - 2 * 2, speedYd - 2 * 2, ModContent.ProjectileType<TheMoon>(), 50, 0f, 0, 0f, 0f);


						
						}
						break;
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
				
						
				

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}

		}


		private void CloneStartSummonVerlia()
		{
			NPC.spriteDirection = NPC.direction;
			timer++;
			if (timer == 2)
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon"));
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 0, 0f, 0, 0f, 0f);
			}
			if (timer > 5)
			{
				switch (Main.rand.Next(1))
				{

					case 0:
						if (timer == 6)
						{
							float speedXd = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
							float speedYd = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 60, ModContent.NPCType<CloneV>());



						}
						break;
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




				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


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
				float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
				float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 15, NPC.position.Y + speedYb + 30, speedXb * 0, speedYb * 0, ModContent.ProjectileType<BackgroundOrb>(), 0, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 30, NPC.position.Y + speedYb + 40, speedXb * 0, speedYb * 0, ModContent.ProjectileType<Sigil>(), 0, 0f, 0, 0f, 0f);
			
			}
			if (timer > 5)
			{
			


			

				switch (Main.rand.Next(1))
				{

					case 0:

						
						if (timer == 10)
						{

							float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
							float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
							float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
						}

						if (timer == 30)
						{
							float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-2f, -2f);
							float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(2f, 2f);
							float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(2, 2) * 0f;
							float speedYc = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-2, -2) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYb * -1, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYc * 1, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
						}

						if (timer == 50)
						{
							float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
							float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-1f, -1f);
							float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(1f, 1f);
							float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(4, 4) * 0f;
							float speedYc = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 1, speedYb * -1, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 1, speedYc * 1, ModContent.ProjectileType<SineSword>(), 23, 0f, 0, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
						}
						break;
				}

			}
			if (timer == 55)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				
				State = ActionState.SummonIdle;


				ResetTimers();




				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


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
						State = ActionState.Unsummon;
						ResetTimers();
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

				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 150)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 60, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 50, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 200)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 90, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 40, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 250)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 20, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 20, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 300)
			{

				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 40, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 350)
			{
				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 20, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 400)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 450)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 90, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 50, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}
			if (timer == 500)
			{

				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 100, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 275)
			{


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 22, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 100, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 20, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 375)
			{

				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 60, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 50, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 225)
			{
				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 190, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 50, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 125)
			{



				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 230, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 325)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 22, 0f, 0, 0f, 0f);
				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 140, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 475)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 22, 0f, 0, 0f, 0f);
				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 220, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 400)
			{

				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa - 120, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}

			if (timer == 425)
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 22, 0f, 0, 0f, 0f);


				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 190, speedXb * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}
			if (timer == 175)
			{
				int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 22, 0f, 0, 0f, 0f);

				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa + 200, speedXa * 1, speedYa - 1 * 0, ModContent.ProjectileType<SineSword>(), 30, 0f, 0, 0f, 0f);
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordThrow"));
			}


















			if (timer == 600)
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
				if (NPC.life < NPC.lifeMax / 7)
				{

					switch (Main.rand.Next(1))
					{
						case 0:
							State = ActionState.HoldUPdie;
							ResetTimers();
							break;


					}





				}
				if (NPC.life > NPC.lifeMax / 7)
				{

					switch (Main.rand.Next(1))
					{
						case 0:
							State = ActionState.HoldUP;
							ResetTimers();
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
					float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Strummer>(), 12, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Strummer>(), 12, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Sadano"));

					


				}

				if (NPC.life < NPC.lifeMax / 3)
				{
					

					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}

				if (NPC.life < NPC.lifeMax / 4)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
		
				}

				if (NPC.life < NPC.lifeMax / 5)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}

				if (NPC.life < NPC.lifeMax / 6)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}
			}
			
			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				

				if (NPC.life < NPC.lifeMax / 2)
				{
					if (NPC.life < NPC.lifeMax / 7)
					{

						switch (Main.rand.Next(1))
						{
							case 0:
								State = ActionState.Dienow;
								ResetTimers();
								break;


						}





					}
					if (NPC.life > NPC.lifeMax / 7)
					{

						switch (Main.rand.Next(1))
						{
							case 0:
								State = ActionState.SwordUP;
								ResetTimers();
								break;


						}





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



		private void HoldUPVerliadie()
		{
			timer++;
			if (timer == 2)
			{
				if (NPC.life < NPC.lifeMax / 2)
				{
					float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Strummer>(), 12, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Strummer>(), 12, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Sadano"));




				}

				if (NPC.life < NPC.lifeMax / 3)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}

				if (NPC.life < NPC.lifeMax / 4)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);

					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);

				}

				if (NPC.life < NPC.lifeMax / 5)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}

				if (NPC.life < NPC.lifeMax / 6)
				{


					float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
					float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, -4) * 0f;


					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 15, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa - 2 * 1, speedYa - 1 * 2, ModContent.ProjectileType<AltideSword>(), 12, 0f, 0, 0f, 0f);
				}
			}

			if (timer == 30)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

						switch (Main.rand.Next(1))
						{
							case 0:
								State = ActionState.SwordUP;
								ResetTimers();
								break;


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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), 33, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot2"));
				}
				
				if (timer == 45)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), 33, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot3"));
				}
			
				if (timer == 65)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot2>(), 33, 0f, 0, 0f, 0f);
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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot>(), 27, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot1"));
				}
				
				
				if (timer == 45)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb + 10, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<FrostShot>(), 27, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/FrostShot3"));
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
			if (timer == 5)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"));
				if (NPC.life < NPC.lifeMax / 2)
				{
					int index2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 10, (int)NPC.Center.Y - 40, ModContent.NPCType<GhostCharger>());
				
					int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 30, ModContent.NPCType<GhostCharger>());
				}
			}
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
				NPC.position.Y = player.Center.Y + distanceY;
				
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
				NPC.position.Y = player.Center.Y + distanceY;

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
			if (timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VDisappear"));
				
			}
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
					switch (Main.rand.Next(4))
					{
						case 0:
							State = ActionState.SummonStartup;
							ResetTimers();
							break;

						case 1:
							State = ActionState.MoonSummonStartup;
							ResetTimers();
							break;

						case 2:
							State = ActionState.CloneSummonStartup;
							ResetTimers();
							break;

						case 3:
							State = ActionState.BigSwordSummonStartup;
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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashRight>(), 26, 0f, 0, 0f, 0f);
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"));
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"));
				}
                else
                {
					float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
					float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb - 100, NPC.position.Y + speedYb + 80, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashLeft>(), 26, 0f, 0, 0f, 0f);
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
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y + speedYb, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SlashHold>(), 110, 0f, Main.myPlayer, 0f, ai1);

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
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY - 1 * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 3, speedY * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY * 1f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 2, speedY - 3 * 1.5f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 1, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 3, speedY - 2 * 2f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 3, speedY - 1 * 1f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 1, speedY - 3, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 1 * 3f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 1 * 3, speedY * 2f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 3, speedY - 2 * 1f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"));
						break;

					case 1:

						break;

					case 2:
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa - 1 * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 3, speedYa * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa * 1f, ModContent.ProjectileType<SineButterfly>(), 15, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa * 1f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 2, speedYa - 3 * 1.5f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 1, speedYa - 1, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 3, speedYa - 2 * 2f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 1 * 3, speedYa - 1 * 1f, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 1, speedYa - 3, ProjectileID.DandelionSeed, 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 2, speedYa - 1 * 3f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa + 1 * 3, speedYa * 2f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 1 * 3, speedYa - 2 * 1f, ModContent.ProjectileType<CosButterfly>(), 9, 0f, 0, 0f, 0f);
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
				NPC.position.Y = player.Center.Y + distanceY;
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
				NPC.position.Y = player.Center.Y + distanceY;

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
						NPC.position.X = player.Center.X + distance;
						NPC.position.Y = player.Center.Y + distanceY;

						break;


					case 1:
						int distance2 = Main.rand.Next(-120, -120);
						int distanceY2 = Main.rand.Next(-110, -110);
						NPC.position.X = player.Center.X + distance2;
						NPC.position.Y = player.Center.Y + distanceY2;

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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), 20, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB - 2 * 6, speedY, ModContent.ProjectileType<SpikeBullet>(), 20, 0f, 0, 0f, 0f);



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
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX * 0, speedY * 0, ProjectileID.DD2OgreSmash, 0, 0f, 0, 0f, 0f);


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
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX + 2 * 6, speedY, ModContent.ProjectileType<StarBullet>(), 10, 0f, 0, 0f, 0f);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB - 2 * 6, speedY, ModContent.ProjectileType<StarBullet>(), 10, 0f, 0, 0f, 0f);
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
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<SmallRock>(), 10, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 1, speedY - 2 * 1, ModContent.ProjectileType<SmallRock2>(), 10, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 2 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedXB + 2 * 2, speedY - 2 * 2, ModContent.ProjectileType<Rock>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 110, speedX - 1 * 2, speedY - 2 * 1, ModContent.ProjectileType<Rock2>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX + 60, NPC.position.Y + speedY + 80, speedX * 0.1f, speedY - 1 * 1, ModContent.ProjectileType<BigRock>(), 40, 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
						break;


					case 2:
						float speedXBb = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedXb = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYb = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<SmallRock>(), 10, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXBb + 2 * 1, speedYb - 2 * 1, ModContent.ProjectileType<SmallRock2>(), 10, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 2 * 2, speedYb - 2 * 1, ModContent.ProjectileType<Rock>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXBb + 2 * 2, speedYb - 2 * 2, ModContent.ProjectileType<Rock>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 110, speedXb - 1 * 2, speedYb - 2 * 1, ModContent.ProjectileType<Rock2>(), 20, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb + 60, NPC.position.Y + speedYb + 80, speedXb * 0.1f, speedYb - 1 * 1, ModContent.ProjectileType<BigRock>(), 40, 0f, 0, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Verirocks"));
						break;

					case 3:
						float speedXBa = NPC.velocity.X * Main.rand.NextFloat(-.3f, -.3f) + Main.rand.NextFloat(-4f, -4f);
						float speedXa = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
						float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXa - 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), 5, 0f, 0, 0f, 0f);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa + 60, NPC.position.Y + speedYa + 110, speedXBa + 2 * 8, speedYa - 1 * 1, ModContent.ProjectileType<Flowing>(), 5, 0f, 0, 0f, 0f);
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



		
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}


		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSTARBoss, -1);

			if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
			{
				Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
			}
		}

	}
}
