using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Event.Gintzearmy
{
    public class GintzeSolider : ModNPC
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
			Main.npcFrameCount[NPC.type] = 11;

			
		}
		public override void SetDefaults()
		{
			NPC.width = 50; // The width of the npc's hitbox (in pixels)
			NPC.height = 52; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 30; // The amount of damage that this npc deals
			NPC.defense = 0; // The amount of defense that this npc has
			NPC.lifeMax = 100; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 5f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0.2f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;


		}
	
		public override void AI()
		{


			timer3++;
			timer4++;
			switch (State)
			{

				case ActionState.Jump:
				
					counter++;
					Jump();
					break;

				case ActionState.Pace:
				
					counter++;
					NPC.velocity.Y += 4f;

					NPC.TargetClosest(true);

					// Now we check the make sure the target is still valid and within our specified notice range (500)
					if (Main.player[NPC.target].HasBuff(ModContent.BuffType<GintzeSeen>()) && NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 250f)
					{
						// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
						State = ActionState.Call;
						ResetTimers();
					}

					Pace();
				

					break;

				case ActionState.Call:
				
					NPC.aiStyle = 3;
					AIType = NPCID.SnowFlinx;
					Call();
					break;


				case ActionState.Wait:
			
					counter++;
					Wait();
					break;

				case ActionState.Fall:
			
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
					rect = new Rectangle(0, 6 * 52, 50, 2 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 15, 2, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Wait:
					rect = new Rectangle(0, 5 * 56, 40, 1 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 20, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Fall:
					rect = new Rectangle(0, 0, 66, 70);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 30, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;








				case ActionState.Pace:
					rect = new Rectangle(0, 1 * 52, 50, 10 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

					case ActionState.Call:
					rect = new Rectangle(0, 1 * 52, 50, 10 * 52);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
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
			timer++;
			NPC.velocity.Y += 4f;
			if (Main.player[NPC.target].HasBuff(ModContent.BuffType<GintzeSeen>()) && NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 250f)
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
		public void Call()
		{
			timer++;

			Player player = Main.player[NPC.target];
			if (timer == 50)
			{

				NPC.velocity.X *= 1.3f;
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TreasureSparkle, NPC.direction, -1f, 1, default, .61f);
				}





			}

			if (Main.player[NPC.target].Distance(NPC.Center)  > 350f || !Main.player[NPC.target].HasBuff(ModContent.BuffType<GintzeSeen>()))
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzlMetal>(), 6, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizMetal>(), 6, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
		}
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gofria's ranks, be careful"))
			});
		}
	}
}