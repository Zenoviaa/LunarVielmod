using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using On.Terraria.DataStructures;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Summon;
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

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
	public class CloneV : ModNPC
	{
		// States
		public enum ActionState
		{
		
			Wait,
			Shoot
		}
		// Current state

		public ActionState State = ActionState.Shoot;
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

			
		}
		public override void SetDefaults()
		{
			NPC.width = 36; // The width of the npc's hitbox (in pixels)
			NPC.height = 52; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 50; // The amount of damage that this npc deals
			NPC.defense = 0; // The amount of defense that this npc has
			NPC.lifeMax = 200; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.knockBackResist = 0f;
			
		}


	
		public override void AI()
		{
			switch (State)
			{

				case ActionState.Shoot:
					NPC.velocity *= 0.95f;
					counter++;
					Jump();
					break;

				case ActionState.Wait:
					counter++;
					NPC.aiStyle = 10;
					Wait();
					break;


				default:
					counter++;
					break;
			}




			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);

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
				case ActionState.Shoot:				
					rect = new Rectangle(0, 2 * 104, 36, 10 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Wait:
					rect = new Rectangle(0, 1 * 104, 36, 1 * 104);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 60, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;

			}
			return false;
		}
		
		public void Jump()
		{
			timer++;
			if (timer == 1)
            {
				int index = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 30, ModContent.NPCType<GhostCharger>());
				NPC minionNPC = Main.npc[index];
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

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(90, 200, 210, 125) * (1f - (float)NPC.alpha / 50f);
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Clone of a powerfu sexy goddess :)")
			});
		}
	}
}