using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Fenix
{
    public class ALCADSWIRL : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 12;
		}

		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		// Current state's timer
		public float timer;
		public int PrevAtack;
		float DaedusDrug = 4;
		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 540;
			NPC.height = 540;
			NPC.damage = 100;
			NPC.defense = 30;
			NPC.lifeMax = 500000;
			NPC.value = 0f;
			NPC.timeLeft = 450;
			NPC.knockBackResist = .0f;
			NPC.aiStyle = 85;
			AIType = NPCID.StardustCellBig;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
		}


		int invisibilityTimer;


		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive

			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			spriteBatch.Draw(texture, new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, new Color(255, 255, 255, 0) * (1f - NPC.alpha / 80f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 2, effects, 0);


			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)


			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			return false;
		}

		float trueFrame = 0;
		public override void FindFrame(int frameHeight)
		{


			NPC.frame.Width = 540;
			NPC.frame.X = ((int)trueFrame % 6) * NPC.frame.Width;
			NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 6)) / 6) * NPC.frame.Height;
		}

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
		public float Shooting = 0f;



		int bee = 220;
		int bee2 = 600;
		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;
		public override void AI()
		{
			var entitySource = NPC.GetSource_FromAI();
			timer++;
			NPC.spriteDirection = NPC.direction;
			Shooting++;

			invisibilityTimer++;

			
			NPC.rotation -= 0.3f;

			UpdateFrame(1f, 1, 72);
			bee2--;

			



			if (bee2 == 0)
			{
				NPC.Kill();
				for (int i = 0; i < 150; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
					var d = Dust.NewDustPerfect(NPC.Center, DustID.BoneTorch, speed * 11, Scale: 3f);
					;
					d.noGravity = true;

					Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
					var da = Dust.NewDustPerfect(NPC.Center, DustID.CoralTorch, speeda * 5, Scale: 3f);
					;
					da.noGravity = false;

					Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
					var dab = Dust.NewDustPerfect(NPC.Center, DustID.BlueTorch, speeda * 20, Scale: 3f);
					;
					dab.noGravity = false;
				}


				
			}





			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();

					


					break;

				case ActionState.Speed:
					counter++;
					Speed();

					
					break;


				default:
					counter++;
					break;
			}
		}




		public void Wait()
		{
			timer++;

			if (timer > 50)
			{





			}
			else if (timer == 60)
			{
				State = ActionState.Speed;
				timer = 0;


				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}
		}


		public void Speed()
		{
			timer++;


			if (timer > 50)
			{








			}

			if (timer == 60)
			{
				State = ActionState.Wait;
				timer = 0;


				if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
				{
					Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
				}
			}

		}




	}
}