﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Armors.Pieces.RareMetals;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Utilis;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Town
{
	public class NaturalWitch : ModNPC
	{
		// States
		Vector2 dashDirection = Vector2.Zero;
		float dashDistance = 0f;
		public enum ActionState
		{

			Jump,
			Fall,
			Wait,
			Pace,
			Call
		}
		// Current state

		public ActionState State = ActionState.Pace;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;
		public float timer3;
		public float timer4;

		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 14;


		}
		public override void SetDefaults()
		{
			NPC.width = 36; // The width of the npc's hitbox (in pixels)
			NPC.height = 52; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 1; // The amount of damage that this npc deals
			NPC.defense = 2; // The amount of defense that this npc has
			NPC.lifeMax = 2000; // The amount of health that this npc has
			NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
			NPC.value = 10f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.dontTakeDamage = true;

		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			float spawnChance = SpawnCondition.Overworld.Chance * (spawnInfo.Player.ZoneXixianVillage() ? 3f : 0f);
			return spawnChance;
		}
		public override void AI()
		{


			timer3++;
			timer4++;


			switch (State)
			{

				case ActionState.Jump:
					NPC.damage = 0;
					counter++;
					Jump();
					break;

				case ActionState.Pace:
					NPC.damage = 0;
					counter++;



					if (NPC.collideX && NPC.collideY) 
                    {
						NPC.velocity = new Vector2(NPC.direction * 0, -5f);
					}
					NPC.TargetClosest(true);

					// Now we check the make sure the target is still valid and within our specified notice range (500)
					

					Pace();


					break;

				case ActionState.Call:
					NPC.damage = 0;
					counter++;
					NPC.aiStyle = 3;
					AIType = NPCID.SnowFlinx;
					NPC.velocity.X *= 0;
					Call();
					break;


				case ActionState.Wait:
					NPC.damage = 0;
					counter++;
					Wait();
					break;

				case ActionState.Fall:
					NPC.damage = 0;
					counter++;
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0;
						State = ActionState.Wait;
						frameCounter = 0;
						frameTick = 0;
					}
					break;
				default:
					counter++;
					break;
			}

            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }


            Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything

			//for (int j = 0; j < 2; j++)
			//{
			//	Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, NPC.velocity * 0, ProjectileID.Spark, NPC.damage / 2, NPC.knockBackResist);
			//}
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
				case ActionState.Jump:
					rect = new Rectangle(0, 6 * 70, 36, 2 * 70);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 15, 2, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Wait:
					rect = new Rectangle(0, 1 * 52, 36, 1 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Fall:
					rect = new Rectangle(0, 0, 36, 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 30, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;








				case ActionState.Pace:
					rect = new Rectangle(0, 1 * 52, 36, 13 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

				case ActionState.Call:
					rect = new Rectangle(0, 12 * 70, 36, 6 * 70);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
			}
			return false;
		}

		public void Jump()
		{
			timer++;

			if (timer == 9)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(4))
				{
					case 0:
						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
					case 1:
						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
					case 2:
						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
					case 3:

						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


			}
			else if (timer > 27)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Fall;
				ResetTimers();
			}
		}

		public void Wait()
		{
			timer++;


			if (timer > 120)
			{
				switch (Main.rand.Next(3))
				{
					case 0:
						State = ActionState.Pace;
						ResetTimers();
						break;
					case 1:
						State = ActionState.Wait;
						ResetTimers();
						break;
					case 2:
						State = ActionState.Pace;
						timer = 899;
						break;

				}

				
			}
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 20; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
			}
			if (NPC.life <= 0)
			{
				EventWorld.GintzeKills += 1;
				for (int i = 0; i < 20; i++)
				{
					int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
					Main.dust[num].noGravity = true;
					Dust expr_62_cp_0 = Main.dust[num];
					expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
					Dust expr_92_cp_0 = Main.dust[num];
					expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
					if (Main.dust[num].position != NPC.Center)
					{
						Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
					}
				}
			}
		}

		public void Pace()
		{
			NPC.velocity.Y += 4f;
			timer++;

			if (timer == 300)
            {
				
					// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

					switch (Main.rand.Next(3))
					{
						case 0:
							State = ActionState.Pace;
							break;
						case 1:
							State = ActionState.Wait;
							ResetTimers();
							break;

						case 2:
							timer = 899;
							break;

				}

					// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


				
			}

			if (timer < 900)
			{
				Player player = Main.player[NPC.target];
				float speed = 1f;

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
				Vector2 angle = new Vector2((float)anglex);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.direction = 2;
				NPC.velocity.Y += 4f;
			}


			if (timer == 1200)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.Pace;
						break;
					case 1:
						State = ActionState.Wait;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array



			}

			if (timer > 900)
			{
				Player player = Main.player[NPC.target];
				float speed = -1f;

				int distance = Main.rand.Next(2, 2);
				NPC.ai[3] = Main.rand.Next(1);
				double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));

				Vector2 angle = new Vector2((float)anglex);
				dashDirection = ((angle * distance)) - NPC.Center;
				dashDistance = dashDirection.Length();
				dashDirection.Normalize();
				dashDirection *= speed;
				NPC.velocity = dashDirection;
				NPC.direction = 1;
				NPC.velocity.Y += 4f;

			}

			if (timer == 1500)
			{

				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.

				switch (Main.rand.Next(2))
				{
					case 0:
						State = ActionState.Pace;
						break;
					case 1:
						State = ActionState.Wait;
						ResetTimers();
						break;

				}

				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array



			}

			if (timer == 1800)
			{

				ResetTimers();
			}




		}
		public void Call()
		{
			timer++;

			Player player = Main.player[NPC.target];


			player.AddBuff(ModContent.BuffType<GintzeSeen>(), 720, false);
			if (Main.player[NPC.target].Distance(NPC.Center) > 150f)
			{
				timer++;
				if (timer >= 30)
				{
					State = ActionState.Pace;
					ResetTimers();
				}
			}

		}
		public void ResetTimers()
		{
			timer = 0;
			timer4 = 0;
			frameCounter = 0;
			frameTick = 0;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzlMetal>(), 0, 1, 3));
			EventWorld.GintzeKills += 1;
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 6, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzeMask>(), 80, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ItemID.IronBar, 5, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A Captain of Gofria's ranks, be careful")
			});
		}
	}
}