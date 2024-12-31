using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
	public class PULSARHOLE : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 18;
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
			NPC.width = 300;
			NPC.height = 300;
			NPC.damage = 910;
			NPC.defense = 30;
			NPC.lifeMax = 500000;
			NPC.value = 0f;
			NPC.knockBackResist = .0f;
			NPC.aiStyle = -1;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.dontTakeDamage = true;
		}

		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, 1, -1f, 1, default, .61f);
			}
		}

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
			NPC.frame.Width = 300;
			NPC.frame.X = ((int)trueFrame % 5) * NPC.frame.Width;
			NPC.frame.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * NPC.frame.Height;
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
		int bee2 = 535;
		public int rippleCount = 20;
		public int rippleSize = 5;
		public int rippleSpeed = 15;
		public float distortStrength = 300f;
		public override void AI()
		{
			var entitySource = NPC.GetSource_FromAI();
			timer++;
			NPC.TargetClosest();
			NPC.spriteDirection = NPC.direction;

			Shooting++;
			if (Shooting == 1)
			{
				//NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ALCADSWIRL>());
			}
			NPC.noTileCollide = true;
			invisibilityTimer++;
			if (invisibilityTimer == 5)
			{
				
				invisibilityTimer = 0;
			}
		
			UpdateFrame(0.8f, 1, 90);
			bee2--;

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player npc = Main.player[i];

				if (npc.active)
				{
					float distance = Vector2.Distance(NPC.Center, npc.Center);
					if (distance <= 100)
					{
						Vector2 direction = npc.Center - NPC.Center;
						direction.Normalize();
						npc.velocity -= direction * 0.6f;
					}
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


        public override bool CheckActive()
        {
			return false;
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


				
			}
		}


		public void Speed()
		{
			timer++;
			if (timer == 60)
			{
				State = ActionState.Wait;
				timer = 0;


				
			}

		}
	}
}