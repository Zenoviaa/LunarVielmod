using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Armors.Illurian;
using Stellamod.Items.Armors.Pieces.RareMetals;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Mage;
using Stellamod.NPCs.Event.Gintzearmy;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Illuria
{
	public class IllurianMage : ModNPC
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
			Paceopp,
			Call,
			Attack
		}
		// Current state

		public ActionState State = ActionState.Wait;
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
			Main.npcFrameCount[NPC.type] = 30;


		}
		public override void SetDefaults()
		{
			NPC.width = 40; // The width of the npc's hitbox (in pixels)
			NPC.height = 96; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 30; // The amount of damage that this npc deals
			NPC.defense = 50; // The amount of defense that this npc has
			NPC.lifeMax = 2000; // The amount of health that this npc has
			NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
			NPC.value = 10f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0.1f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;

		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.OneFromOptions(1,
				ModContent.ItemType<IllurianCrestpants>(),
				ModContent.ItemType<IllurianCrestplate>(),
				ModContent.ItemType<IllurianCrestmask>()
			));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IllurineScale>(), minimumDropped: 3, maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrameStaff>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 1));
        }

		public override void AI()
		{


			timer3++;
			timer4++;


			if (timer3 == 1)
			{
				if (StellaMultiplayer.IsHost)
				{
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 30, (int)NPC.Center.Y - 10, ModContent.NPCType<IllurianGuard>());
					NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 30, (int)NPC.Center.Y - 10, ModContent.NPCType<IllurianGuard>());
				}
			}
			switch (State)
			{

				

				case ActionState.Pace:
					NPC.damage = 40;
					counter++;
					NPC.velocity.Y += 4f;

					NPC.TargetClosest(true);

					// Now we check the make sure the target is still valid and within our specified notice range (500)
					if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 450f)
					{
						// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
						State = ActionState.Call;
						ResetTimers();
					}

					Pace();

					break;


				case ActionState.Paceopp:
					NPC.damage = 40;
					counter++;
					NPC.velocity.Y += 4f;

					NPC.TargetClosest(true);

					// Now we check the make sure the target is still valid and within our specified notice range (500)
					if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 450f)
					{
						// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
						State = ActionState.Call;
						ResetTimers();
					}

					Paceopp();

					break;

				case ActionState.Call:
					NPC.damage = 0;
					counter++;
					NPC.velocity.X *= 0;
					BeforeAttack();
					break;

				case ActionState.Attack:
					NPC.damage = 100;
					counter++;
					NPC.velocity.X *= 0;
					Attack();
					break;


				case ActionState.Wait:
					NPC.damage = 40;
					counter++;

					NPC.TargetClosest(true);

					// Now we check the make sure the target is still valid and within our specified notice range (500)
					if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 850f)
					{
						// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
					
						
						switch (Main.rand.Next(2))
						{
							case 0:
								State = ActionState.Pace;
								break;
							case 1:
								State = ActionState.Paceopp;
								break;
							
						}
						ResetTimers();
					}

					break;

				default:
					counter++;
					break;
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
					rect = new Rectangle(0, 6 * 70, 66, 2 * 70);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 15, 2, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

				case ActionState.Fall:
					rect = new Rectangle(0, 0, 66, 70);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 30, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;




				case ActionState.Attack:
					rect = new Rectangle(0, 15 * 96, 40, 10 * 96);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;


				case ActionState.Wait:
					rect = new Rectangle(0, 26 * 96, 40, 4 * 96);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 20, 4, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

				case ActionState.Pace:
					rect = new Rectangle(0, 1 * 96, 40, 13 * 96);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

				case ActionState.Paceopp:
					rect = new Rectangle(0, 1 * 96, 40, 13 * 96);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;


				case ActionState.Call:
					rect = new Rectangle(0, 14 * 96, 40, 1 * 96);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 500, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
			}
			return false;
		}


		public void BeforeAttack()
		{
			timer++;


			if (timer > 60)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Attack;
				ResetTimers();
			}
		}
		public void Wait()
		{
			timer++;


			if (timer > 60)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Pace;
				ResetTimers();
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

			if (Main.player[NPC.target].Distance(NPC.Center) < 250f)
			{

				if (timer > 30)
				{
					State = ActionState.Call;
					ResetTimers();
				}
			}

			if (timer < 300)
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

			if (timer > 300)
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

			if (timer == 600)
			{

				ResetTimers();
			}




		}

		public void Paceopp()
		{
			NPC.velocity.Y += 4f;
			timer++;

			if (Main.player[NPC.target].Distance(NPC.Center) < 250f)
			{

				if (timer > 30)
				{
					State = ActionState.Call;
					ResetTimers();
				}
			}

			if (timer < 300)
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

			if (timer > 300)
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

			if (timer == 600)
			{

				ResetTimers();
			}




		}

		public void Attack()
		{
			timer++;

			Player player = Main.player[NPC.target];


			if (timer == 12)
			{

				Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
				float numberProjectiles = 1;
				float rotation = MathHelper.ToRadians(30);

				for (int i = 0; i < numberProjectiles; i++)
				{
					Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
					if (StellaMultiplayer.IsHost)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<HoloBuster>(), 50, 1, Main.myPlayer, 0, 0);
					}
				}
			}



			
				if (timer >= 30)
				{
					switch (Main.rand.Next(2))
					{
					case 0:
						State = ActionState.Pace;
						break;
					case 1:
						State = ActionState.Paceopp;
						break;

					}
					ResetTimers();
			}
			

		}
		public void ResetTimers()
		{
			timer = 0;
			timer4 = 0;
			frameCounter = 0;
			frameTick = 0;
		}
		public override bool CheckActive()
		{
			//Returning false here makes them not despawn
			return false;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A respected sorcerer from Sigfried's old rein"))
			});
		}
	}
}