using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
	// This ModNPC serves as an example of a completely custom AI.
	public class TribalPeasant : ModNPC
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
		private enum Frames
		{
			Asleep,
			Notice,
			Jump,
			Falling,

		}

		// These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
		// Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
		// This is all to just make beautiful, manageable, and clean code.
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public int counter;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flutter Slime"); // Automatic from .lang files
			Main.npcFrameCount[NPC.type] = 35; // make sure to set this for your modnpcs.

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

		
			NPC.width = 32; // The width of the npc's hitbox (in pixels)
			NPC.height = 26; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 0; // The amount of damage that this npc deals
			NPC.defense = 6; // The amount of defense that this npc has
			NPC.lifeMax = 200; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.value = 500f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = .5f;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.InModBiome<MorrowUndergroundBiome>())
			{
				return SpawnCondition.Cavern.Chance * 0.5f;
			}
			return SpawnCondition.Cavern.Chance * 0f;
		}
		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			counter++;
			// The npc starts in the asleep state, waiting for a player to enter range
			switch (AI_State)
			{
				case (float)ActionState.Asleep:
					NPC.damage = 0;
					counter++;
					FallAsleep();
				
					break;

				case (float)ActionState.Notice:
					NPC.damage = 0;
					counter++;


					Notice();
					
					break;

				case (float)ActionState.Jump:
					NPC.damage = 0;
					counter++;
					Jump();
					
					
					break;
				case (float)ActionState.Fall:
					NPC.damage = 90;
					counter++;
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0;
						AI_State = (float)ActionState.Asleep;
						
					}

					break;
			}
		}

		float trueFrame = 0;
		public void UpdateFrame(float speed, int minFrame, int maxFrame)
		{
			trueFrame += speed;
			if (trueFrame < minFrame)
			{
				trueFrame = minFrame;
			}
			if (trueFrame > maxFrame)
			{
				trueFrame = minFrame;
			}
		}
		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.





		//Frames: 1 - 7 is jump (this is just jump, it plays after notice (see Jump() later down)
		//Frames: 8 - 21 is fall (plays quickest out of the couple, I'd say if the other frames speed were 75 than this is 50, about a third difference)
		// frames 22 - 29 are asleep calltime / when the enemy doesnt notice
		//frames 30 - 35 are notice (this takes place when the player is in range (see Notice() and Fallasleep() a bit further down under my frame failure)

		//Also Im really sorry i just couldnt get this to work in game with the frames and ive been searching for a while so i need help, (examplemod didnt explain it well at all because of how simple the frame system is)



		public override void FindFrame(int frameHeight)
		{

			NPC.frameCounter++;
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			NPC.spriteDirection = NPC.direction;

			// For the most part, our animation matches up with our states.
			switch (AI_State)
			{
				case (float)ActionState.Asleep:
					// npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.

					
					NPC.frame.Y = 22 * frameHeight;
					NPC.frameCounter = 0;
					NPC.frameCounter++;
					counter++;

					if (counter == 0)
						NPC.frame.Y = 22 * frameHeight;

					else if (counter == 8)
						NPC.frame.Y = 23 * frameHeight;

					else if (counter == 16)
						NPC.frame.Y = 24 * frameHeight;

					else if (counter == 24)
						NPC.frame.Y = 25 * frameHeight;

					else if (counter == 32)
						NPC.frame.Y = 26 * frameHeight;

					else if (counter == 40)
						NPC.frame.Y = 27 * frameHeight;

					else if (counter == 48)
						NPC.frame.Y = 28 * frameHeight;

					else if (counter == 56)
						NPC.frame.Y = 29 * frameHeight;

					else if (counter == 64)
						NPC.frameCounter = 0;



						break;
				
				case (float)ActionState.Notice:
					// Going from Notice to Asleep makes our npc look like it's crouching to jump.
					
					NPC.frame.Y = 30 * frameHeight;
					NPC.frameCounter = 0;
					NPC.frameCounter++;
					counter++;

					if (counter == 0)
						NPC.frame.Y = 30 * frameHeight;

					else if (counter == 6)
						NPC.frame.Y = 31 * frameHeight;

					else if (counter == 12)
						NPC.frame.Y = 32 * frameHeight;

					else if (counter == 18)
						NPC.frame.Y = 33 * frameHeight;

					else if (counter == 24)
						NPC.frame.Y = 34 * frameHeight;

					else if (counter == 30)
						NPC.frame.Y = 35 * frameHeight;

			

					break;
				case (float)ActionState.Jump:

				
					NPC.frame.Y = 1 * frameHeight;
					NPC.frameCounter = 0;
					NPC.frameCounter++;
					counter++;

					if (counter == 0)
                   				NPC.frame.Y = 1 * frameHeight;
					
					else if (counter == 5)
						NPC.frame.Y = 2 * frameHeight;
					
					else if (counter == 10)
						NPC.frame.Y = 3 * frameHeight;
					
					else if (counter == 15)
						NPC.frame.Y = 4 * frameHeight;

					else if (counter == 20)
						NPC.frame.Y = 5 * frameHeight;

					else if (counter == 25)
						NPC.frame.Y = 6 * frameHeight;

					else if (counter == 30)
						NPC.frame.Y = 7 * frameHeight;



					break;

				case (float)ActionState.Fall:
					
					NPC.frame.Y = 8 * frameHeight;
					NPC.frameCounter = 0;
					NPC.frameCounter++;
					counter++; 

					if (counter == 0)
						NPC.frame.Y = 8 * frameHeight;

					else if (counter == 3)
						NPC.frame.Y = 9 * frameHeight;

					else if (counter == 6)
						NPC.frame.Y = 10 * frameHeight;

					else if (counter == 9)
						NPC.frame.Y = 11 * frameHeight;

					else if (counter == 12)
						NPC.frame.Y = 12 * frameHeight;

					else if (counter == 15)
						NPC.frame.Y = 13 * frameHeight;

					else if (counter == 18)
						NPC.frame.Y = 14 * frameHeight;
					
					else if (counter == 21)
						NPC.frame.Y = 15 * frameHeight;

					else if (counter == 24)
						NPC.frame.Y = 16 * frameHeight;

					else if (counter == 27)
						NPC.frame.Y = 17 * frameHeight;

					else if (counter == 30)
						NPC.frame.Y = 18 * frameHeight;

					else if (counter == 33)
						NPC.frame.Y = 19 * frameHeight;

					else if (counter == 36)
						NPC.frame.Y = 20 * frameHeight;

					else if (counter == 39)
						NPC.frame.Y = 21 * frameHeight;


					break;
			}
		}
		private void FallAsleep()
		{
			// TargetClosest sets npc.target to the player.whoAmI of the closest player.
			// The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
			// This is also automatically flipped if npc.confused.
			NPC.TargetClosest(true);
			UpdateFrame(0.5f, 22, 29);
			

			// Now we check the make sure the target is still valid and within our specified notice range (500)
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 200f)
			{
				// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
				AI_State = (float)ActionState.Notice;
				AI_Timer = 0;
				UpdateFrame(0.4f, 30, 35);
			}
		}
		private void Notice()
		{
			// If the targeted player is in attack range (250).
			if (Main.player[NPC.target].Distance(NPC.Center) < 100f)
			{
				// Here we use our Timer to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being used to animate the pre-jump crouch
				AI_Timer++;
				

				if (AI_Timer >= 30)
				{
					AI_State = (float)ActionState.Jump;
					AI_Timer = 0;
					UpdateFrame(0.4f, 1, 7);
				}
			}
			else
			{
				NPC.TargetClosest(true);
				AI_Timer++;
				if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 300f)
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
				NPC.velocity = new Vector2(NPC.direction * 4, -10f);
			}
			else if (AI_Timer > 30)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				AI_State = (float)ActionState.Fall;
				AI_Timer = 0;
				UpdateFrame(0.2f, 8, 21);
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

			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"));

			for (int i = 0; i < 5; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
				ParticleManager.NewParticle(NPC.Center, speed * 4, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
			}
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Even the weakest, most poor among the warriors in the morrow are still decent foes..")
			});
		}
	}
}