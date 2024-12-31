using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
    public class MorrowedJellyFish : ModNPC
	{
		// States
		public enum ActionState
		{
			
			Jump,
			Fall,
			Wait
		}
		// Current state

		public ActionState State = ActionState.Jump;
		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;
		public float Mainframe;
		// AI counter
		public int counter;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;

			
		}
		public override void SetDefaults()
		{
			NPC.width = 40; // The width of the npc's hitbox (in pixels)
			NPC.height = 56; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 1; // The amount of damage that this npc deals
			NPC.defense = 15; // The amount of defense that this npc has
			NPC.lifeMax = 1500; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.value = 5000f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.WaterCritter.Chance * 0.5f;
		}
		public override void AI()
		{
			switch (State)
			{

				case ActionState.Jump:
					NPC.damage = 250;
					counter++;
					Jump();
					break;

				case ActionState.Wait:
					NPC.damage = 0;
					counter++;
					Wait();
					break;

				case ActionState.Fall:
					NPC.damage = 250;
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
				case ActionState.Jump:				
					rect = new Rectangle(0, 6 * 56, 40, 2 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 15, 2, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Wait:
					rect = new Rectangle(0, 5 * 56, 40, 1 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 20, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Fall:
					rect = new Rectangle(0, 0, 40, 5 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 17, 5, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
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
		
				
				
				// GeneralStellaUtilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, -10, ModContent.ProjectileType<VRay>(), 50, 0f, -1, 0, NPC.whoAmI);

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
				State = ActionState.Jump;
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
			npcLoot.Add(ItemDropRule.Common(ItemID.BlackPearl, 3, 1, 2));
			npcLoot.Add(ItemDropRule.Common(ItemID.Fireblossom, 3, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 2, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyfishTissue>(), 25, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowedJelliesBroochA>(), 25, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.SpelunkerGlowstick, 1, 1, 7));
		}

		public override void HitEffect(NPC.HitInfo hit)
		{

			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"));

			
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Extremely powerful jellyfish that has been aflicted by the gild in the morrow, very dangerour"))
			});
		}
	}
}