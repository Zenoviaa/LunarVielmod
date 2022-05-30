using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Materials;
using Stellamod.Buffs;
using System.Threading;
using Stellamod.Items.Harvesting;
using Stellamod.Items.weapons.melee;
using Stellamod.Items.weapons.ranged;
using Stellamod.Items.weapons.summon;
using Stellamod.Items.weapons.mage;

namespace Stellamod.npcs.Harvesting.Morrow
{
	// This ModNPC serves as an example of a completely custom AI.
	public class HarvestOverworld : ModNPC
	{
		private enum ActionState
		{
			Ambient,
			Amb2,
			Amb3,
			Amb4,

		}
		int frame = 0;
		int timer = 0;


		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			first,
			second,
			third,
			fourth,

		}
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_FlutterTime => ref NPC.ai[2];
		public override void SetStaticDefaults()
		{
			 DisplayName.SetDefault("???"); 
			Main.npcFrameCount[NPC.type] = 16; // make sure to set this for your modnpcs.

			// Specify the debuffs it is immune to
			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned, // This NPC will be immune to the Poisoned debuff.
					BuffID.OnFire
				}
			});
		}

		public override void SetDefaults()
		{
			NPC.width = 7; // The width of the npc's hitbox (in pixels)
			NPC.height = 25; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.// The amount of damage that this npc deals // The amount of defense that this npc has // The amount of health that this npc has
			NPC.DeathSound = SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/ItemHarvested"); // The sound the NPC will make when it dies.
			NPC.value = 250f; // How many copper coins the NPC will drop when killed.
			NPC.HasBuff<Harvester>();
			NPC.friendly = true;
			NPC.lifeMax = 1;
			NPC.dontTakeDamageFromHostiles = true;

		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// we would like this npc to spawn in the overworld.
			return SpawnCondition.Overworld.Chance * 0.1f;
		}

		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			NPC.HasBuff<Harvester>();
			// The npc starts in the asleep state, waiting for a player to enter range
		 
				timer++;
				if (timer >= 15)
				{
					frame++;
					timer = 0;
				}
				if (frame >= 16)
				{
					frame = 0;
				}
			Vector3 RGB = new Vector3(2.55f, 2.55f, 0.94f);
			float multiplier = 1;
			float max = 2.25f;
			float min = 1.0f;
			RGB *= multiplier;
			if (RGB.X > max)
			{
				multiplier = 0.5f;
			}
			if (RGB.X < min)
			{
				multiplier = 1.5f;
			}
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = frameHeight * frame;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 5, 3, 30));
			npcLoot.Add(ItemDropRule.Common(ItemID.Geode, 1, 1, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.HerbBag, 1, 1, 15));
			npcLoot.Add(ItemDropRule.Common(ItemID.LifeCrystal, 7, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 7, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ItemID.PlatinumOre, 3, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ItemID.Musket, 40, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.WoodenCrate, 3, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.ApplePie, 3, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ItemID.Apple, 5, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ItemID.Bomb, 5, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ItemID.EmptyBucket, 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 15, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.BandofStarpower, 15, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.HermesBoots, 20, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.MagicMirror, 20, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.PotionOfReturn, 20, 1, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.WormholePotion, 20, 1, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.Bottle, 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Aglet, 7, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.SandstorminaBottle, 50, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.CloudinaBalloon, 50, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.CanOfWorms, 2, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldenKey, 10, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.DirtBlock, 5, 1, 999));
			npcLoot.Add(ItemDropRule.Common(ItemID.ManaCrystal, 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RippedFabric>(), 1, 3, 9));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 7, 1, 9));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Mushroom>(), 5, 1, 12));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Hlos>(), 21, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ViolinStick>(), 15, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 5, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 2, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Fabric>(), 7, 5, 25));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HornedNail>(), 10, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<wowgun>(), 30, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CanOfLeaves>(), 7, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GildedStaff>(), 7, 1));
		}
        public override void OnKill()
        {


            base.OnKill();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Harvest these bums!")
			});
		}
	}
}