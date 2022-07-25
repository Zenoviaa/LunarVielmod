using Microsoft.Xna.Framework;
using Stellamod.Assets.Biomes;
using Stellamod.Buffs;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Harvesting.Ice
{
	// This ModNPC serves as an example of a completely custom AI.
	public class HarvestIce : ModNPC
	{
		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			first,
			second,
			third,
			fourth
		}
		private enum ActionState
		{
			Ambient,
			Amb2,
			Amb3,
			Amb4
		}
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_FlutterTime => ref NPC.ai[2];

		public int frame = 0;
		public int timer = 0;

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
			NPC.DeathSound = new SoundStyle($"Stellamod/Assets/Sounds/ItemHarvested");  // The sound the NPC will make when it dies.

			NPC.value = 250f; // How many copper coins the NPC will drop when killed.
			NPC.HasBuff<Harvester>();
			NPC.friendly = false;
			NPC.lifeMax = 1;

			NPC.dontTakeDamageFromHostiles = true;
			NPC.AddBuff(ModContent.BuffType<Harvester>(), 999999999);
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneSnow)
            {
					return SpawnCondition.Cavern.Chance  * 0.2f;
			}
			if (spawnInfo.Player.ZoneSnow)
			{
				return SpawnCondition.Overworld.Chance * 0.2f;
			}
			return SpawnCondition.Cavern.Chance * 0f;
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
			Vector3 RGB = new(0.55f, 0.85f, 2.01f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
		}
		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = frameHeight * frame;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 9, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.LifeCrystal, 6, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 7, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ItemID.GoldOre, 3, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceSkates, 20, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.Bomb, 10, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ItemID.EmptyBucket, 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 15, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.FrostDaggerfish, 2, 5, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.PotionOfReturn, 20, 1, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.WormholePotion, 20, 1, 20));
			npcLoot.Add(ItemDropRule.Common(ItemID.Bottle, 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.CanOfWorms, 2, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.DirtBlock, 5, 1, 999));
			npcLoot.Add(ItemDropRule.Common(ItemID.Snowball, 1, 1, 99));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceBlock, 1, 1, 99));
			npcLoot.Add(ItemDropRule.Common(ItemID.ManaCrystal, 9, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Stick>(), 7, 1, 9));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Mushroom>(), 13, 1, 12));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CondensedDirt>(), 5, 1, 25));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Fabric>(), 7, 5, 25));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bagitem>(), 20, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.WaterWalkingBoots, 25, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostSwing>(), 20, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrileOre>(), 2, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Bone, 3, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceBlade, 30, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.IceBoomerang, 15, 1));



		}
		public override void OnKill()
		{
			CombatText.NewText(NPC.getRect(), Color.White, "Ice item Harvested!", true, false);
			base.OnKill();
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Harvest these bums in style :P!")
			});
		}
	}
}