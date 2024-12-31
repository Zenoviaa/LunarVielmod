using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.UI.Dialogue;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.NPCs.Town.MerenaQuestSystem;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class StarteV : ModNPC
	{

		// States
		public enum ActionState
		{
		
			Wait,
			Shoot,
			Starter,
			Death
		}
		// Current state
		public float AI_Timer;
		public float Beam_Timer;
		public ActionState State = ActionState.Starter;
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
			Main.npcFrameCount[NPC.type] = 11;
			NPCID.Sets.TrailCacheLength[NPC.type] = 15;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

		}
		public override void SetDefaults()
		{
			NPC.width = 40; // The width of the npc's hitbox (in pixels)
			NPC.height = 52; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 300; // The amount of damage that this npc deals
			NPC.defense = 0; // The amount of defense that this npc has
			NPC.lifeMax = 10000; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.friendly = true;
			NPC.aiStyle = -1;
			NPC.dontTakeDamageFromHostiles = true;
		}
		public float Spawner = 0;
		public override void AI()
		{
			Spawner++;
			/*
            Player players = Main.player[NPC.target];
            if (Spawner == 2)
            {
                int distanceY = Main.rand.Next(-250, -250);
                NPC.position.X = players.Center.X;
                NPC.position.Y = players.Center.Y + distanceY;
            }*/

			NPC.damage = 0;
			switch (State)
			{
				
				case ActionState.Shoot:
					counter++;
					NPC.velocity *= 0f;
					Jump();

					break;

				case ActionState.Wait:
					counter++;
					NPC.aiStyle = 86;
					NPC.velocity *= 0.96f;
					Wait();
					break;

				case ActionState.Starter:
					counter++;
				
					NPC.velocity.X *= 0f;
					Starter();
					break;

				case ActionState.Death:
					counter++;
					killyoself();
					break;
			}

			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
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
				case ActionState.Shoot:				
					rect = new Rectangle(0, 2 * 104, 40, 10 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(15, 25), texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    NPC.netUpdate = true;
                    break;
				case ActionState.Wait:
					rect = new Rectangle(0, 1 * 104, 40, 1 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(15, 25), texture.AnimationFrame(ref frameCounter, ref frameTick, 60, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    NPC.netUpdate = true;
                    break;
				case ActionState.Starter:
					rect = new Rectangle(0, 0, 40, 11 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(15, 25), texture.AnimationFrame(ref frameCounter, ref frameTick, 9, 11, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    NPC.netUpdate = true;
                    break;
				case ActionState.Death:
					rect = new Rectangle(0, 1 * 104, 40, 1 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(15, 25), texture.AnimationFrame(ref frameCounter, ref frameTick, 60, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    NPC.netUpdate = true;
                    break;
			}


			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)






			
			

			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			return false;
		}
		
		public void Jump()
		{
			timer++;
			if (timer == 1)
            {
				int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 30, ModContent.NPCType<GhostCharger>());
				NPC minionNPC = Main.npc[index];
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Upp"));
			}
			
			 if (timer == 39)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Wait;
				ResetTimers();
			}
		}

		public void Wait()
		{
			timer++;

			
			if (timer == 280)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Shoot;
				ResetTimers();
			}
		}
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}

		public void Starter()
		{
			timer++;
			if (timer == 1)
			{
				if(Main.netMode == NetmodeID.SinglePlayer)
				{
                    DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                    //2. Create a new instance of your dialogue
                    VerliasDialogue exampleDialogue = new VerliasDialogue();

                    //3. Start it
                    dialogueSystem.StartDialogue(exampleDialogue);
                } 
				else
				{
                    Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(),
						(byte)MessageType.StartDialogue,
                        (int)DialogueType.Start_Verlia).Send(-1);
                }


                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 25f);
				NPC.netUpdate = true;
			}

			
		}

		public void killyoself()
		{
			timer++;	
		
			
			if (timer == 2)
			{
				int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VerliaB>());
                Main.npc[index].netUpdate = true;
            }
			if (timer == 3)
			{
				NPC.SimpleStrikeNPC(99999, 1, crit: false, NPC.knockBackResist);
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Clone of a powerful sexy goddess :)"))
			});
		}
	}
}