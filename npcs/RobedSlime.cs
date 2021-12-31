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

namespace Stellamod.npcs
{
	// This ModNPC serves as an example of a completely custom AI.
	public class RobedSlime : ModNPC
	{
		// Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private enum ActionState
		{
			Asleep,
			Notice,
			Jump,
			Fall
		
		}

		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			Asleep,
			Notice,
			Falling,

		}

		// These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
		// Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
		// This is all to just make beautiful, manageable, and clean code.
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_FlutterTime => ref NPC.ai[2];

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flutter Slime"); // Automatic from .lang files
			Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modnpcs.

			// Specify the debuffs it is immune to
			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned // This NPC will be immune to the Poisoned debuff.
				}
			});
		}

		public override void SetDefaults()
		{
			NPC.width = 32; // The width of the npc's hitbox (in pixels)
			NPC.height = 26; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 40; // The amount of damage that this npc deals
			NPC.defense = 6; // The amount of defense that this npc has
			NPC.lifeMax = 160; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
			NPC.value = 25f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = .5f;
			NPC.alpha = 85;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// we would like this npc to spawn in the overworld.
			return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
		}

		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			// The npc starts in the asleep state, waiting for a player to enter range
			switch (AI_State)
			{
				case (float)ActionState.Asleep:
					FallAsleep();
					break;
				case (float)ActionState.Notice:
					Notice();
					break;
				case (float)ActionState.Jump:
					Jump();
					break;
				case (float)ActionState.Fall:
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0;
						AI_State = (float)ActionState.Asleep;
						AI_Timer = 0;
					}

					break;
			}
		}

		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
		public override void FindFrame(int frameHeight)
		{
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			NPC.spriteDirection = NPC.direction;

			// For the most part, our animation matches up with our states.
			switch (AI_State)
			{
				case (float)ActionState.Asleep:
					// npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
					NPC.frame.Y = (int)Frame.Asleep * frameHeight;
					break;
				case (float)ActionState.Notice:
					// Going from Notice to Asleep makes our npc look like it's crouching to jump.
					if (AI_Timer < 10)
					{
						NPC.frame.Y = (int)Frame.Notice * frameHeight;
					}
					else
					{
						NPC.frame.Y = (int)Frame.Asleep * frameHeight;
					}

					break;
				case (float)ActionState.Jump:
					NPC.frame.Y = (int)Frame.Falling * frameHeight;
					break;

				case (float)ActionState.Fall:
					NPC.frame.Y = (int)Frame.Falling * frameHeight;
					break;
			}
		}

		private void FallAsleep()
		{
			// TargetClosest sets npc.target to the player.whoAmI of the closest player.
			// The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
			// This is also automatically flipped if npc.confused.
			NPC.TargetClosest(true);

			// Now we check the make sure the target is still valid and within our specified notice range (500)
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 2000f)
			{
				// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
				AI_State = (float)ActionState.Notice;
				AI_Timer = 0;
			}
		}

		private void Notice()
		{
			// If the targeted player is in attack range (250).
			if (Main.player[NPC.target].Distance(NPC.Center) < 1000f)
			{
				// Here we use our Timer to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being used to animate the pre-jump crouch
				AI_Timer++;

				if (AI_Timer >= 20)
				{
					AI_State = (float)ActionState.Jump;
					AI_Timer = 0;
				}
			}
			else
			{
				NPC.TargetClosest(true);

				if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 2000f)
				{
					// Out targeted player seems to have left our range, so we'll go back to sleep.
					AI_State = (float)ActionState.Asleep;
					AI_Timer = 0;
				}
			}
		}

		private void Jump()
		{
			AI_Timer++;

			if (AI_Timer == 1)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				NPC.velocity = new Vector2(NPC.direction * 4, -7f);
			}
			else if (AI_Timer > 20)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				AI_State = (float)ActionState.Fall;
				AI_Timer = 0;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Robe, 5)); 
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RippedFabric>(), 1, 3, 9));
		}

		public override void HitEffect(int hitDirection, double damage)
		{

			for (int i = 0; i < 10; i++)
			{
				int dustType = Main.rand.Next(110, 113);
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}


		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("This type of slime for some reason is really magical somehow, it is exceptionally powerful.")
			});
		}
	}
}