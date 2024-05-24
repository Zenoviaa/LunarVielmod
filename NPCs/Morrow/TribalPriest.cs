﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
    public class TribalPriest : ModNPC
	{
		// States
		public enum ActionState
		{
			Asleep,
			Notice,
			Attack
		}
		// Current state
		public ActionState State = ActionState.Asleep;

		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 26;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}
		public override void SetDefaults()
		{
			NPC.width = 114; // The width of the npc's hitbox (in pixels)
			NPC.height = 92; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 1; // The amount of damage that this npc deals
			NPC.defense = 2; // The amount of defense that this npc has
			NPC.lifeMax = 200; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
            NPC.value = 500f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneFable() ? 0.5f : 0f;
            }

            if (spawnInfo.Player.InModBiome<MorrowUndergroundBiome>())
            {
                return SpawnCondition.Underground.Chance * 0.1f;
            }

            return 0f;
        }

        public override void AI()
		{
			switch (State)
			{
				case ActionState.Asleep:
					NPC.damage = 0;
					counter++;
					NPC.aiStyle = 22;
					NPC.velocity.Y *= 1.01f;
					FallAsleep();
					break;
				case ActionState.Notice:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					Notice();
					break;
				case ActionState.Attack:
					NPC.damage = 90;
					counter++;
					Attack();
					break;
				
				default:
					counter++;
					break;
			}

			Vector3 RGB = new(2.30f, 2.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive
			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			// The rectangle we specify allows us to only cycle through specific parts of the texture by defining an area inside it

			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			Rectangle rect;

			switch (State)
			{
				case ActionState.Asleep:
					rect = new(0, 0, 114, 8 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 8, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Notice:
					rect = new Rectangle(0, 9 * 92, 114, 6 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 6, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Attack:
					rect = new Rectangle(0, 15 * 92, 114, 11 * 92);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 11, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
		
			}
			return false;
		}
		
		public void FallAsleep()
		{
			// TargetClosest sets npc.target to the player.whoAmI of the closest player.
			// The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
			// This is also automatically flipped if npc.confused.
			NPC.TargetClosest(true);			

			// Now we check the make sure the target is still valid and within our specified notice range (500)
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 70f)
			{
				// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
				State = ActionState.Notice;
				ResetTimers();
			}
		}

		public void Notice()
		{		
			timer++;
			if (timer >= 23)
			{
				State = ActionState.Attack;
				ResetTimers();
			}
			
			if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 75f)
			{
				State = ActionState.Asleep;
				ResetTimers();
			}	
		}

		public void Attack()
		{
			timer++;

			if (timer == 1)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						NPC.velocity = new Vector2(NPC.direction * 5, -0.1f);
						break;
					case 1:
						NPC.velocity = new Vector2(NPC.direction * 5, -0.1f);
						break;
					case 2:
						NPC.velocity = new Vector2(NPC.direction * 5, -0.1f);
						break;
					case 3:

						NPC.velocity = new Vector2(NPC.direction * 4, -0.1f);
						break;
				}
				NPC.netUpdate = true;
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Slapin"));
				ShakeModSystem.Shake = 4;
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
			 
			if (timer == 21)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Notice;
				ResetTimers();
			}
		}
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Bomb, 5, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Fireblossom, 3, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 3, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Morrowshroom>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowChestKey>(), 3, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OvermorrowWood>(), 1, 1, 5));
		}
		public override void HitEffect(NPC.HitInfo hit)
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
				new FlavorTextBestiaryInfoElement("A much richer form of the trible worshippers")
			});
		}
	}
}