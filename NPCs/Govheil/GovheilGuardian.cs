using Microsoft.Xna.Framework;
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
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Govheil
{
	public class GovheilGuardian : ModNPC
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
			Main.npcFrameCount[NPC.type] = 33;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}
		public override void SetDefaults()
		{
			NPC.width = 140; // The width of the npc's hitbox (in pixels)
			NPC.height = 140; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 1; // The amount of damage that this npc deals
			NPC.defense = 13; // The amount of defense that this npc has
			NPC.lifeMax = 810; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.value = 500f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.InModBiome<GovheilCastle>())
			{
				return 0.5f;
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
					FallAsleep();
					break;
				case ActionState.Notice:
					NPC.damage = 0;
					NPC.velocity *= 0;
					counter++;
					Notice();
					break;
				case ActionState.Attack:
					NPC.damage = 275;
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
					rect = new(0, 0, 140, 24 * 140);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 24, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Notice:
					rect = new Rectangle(0, 24 * 140, 140, 1 * 140);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 1000, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Attack:
					rect = new Rectangle(0, 25 * 140, 140, 8 * 140);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 3, 8, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
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
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 100f)
			{
				// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
				State = ActionState.Notice;
				ResetTimers();
			}
		}
		public void Notice()
		{

			timer++;
			if (timer >= 20)
			{
				State = ActionState.Attack;
				ResetTimers();
			}




			if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 100f)
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
						NPC.velocity = new Vector2(NPC.direction * 2, -0.1f);
						break;
					case 1:
						NPC.velocity = new Vector2(NPC.direction * 2, -0.1f);
						break;
					case 2:
						NPC.velocity = new Vector2(NPC.direction * 2, -0.1f);
						break;
					case 3:

						NPC.velocity = new Vector2(NPC.direction * 2, -0.1f);
						break;
				}
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CorsageRune1"));
				ShakeModSystem.Shake = 8;
				// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array

				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, NPC.direction, -1f, 1, default, .61f);

					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
					float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, 4) * 0f;
					if (StellaMultiplayer.IsHost)
					{
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 3, speedY, 
							ProjectileID.GoldenShowerHostile, 40, 0f, Owner: Main.myPlayer);
                    }
				
				}


				for (int i = 0; i < 100; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(NPC.Center, speed * 10, ParticleManager.NewInstance<morrowstar>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
				}
				for (int i = 0; i < 100; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(NPC.Center, speed * 5, ParticleManager.NewInstance<morrowstar>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
				}
				for (int i = 0; i < 100; i++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(NPC.Center, speed * 7, ParticleManager.NewInstance<morrowstar>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
				}

			}
			if (timer == 24)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Notice;
				ResetTimers();
				NPC.SimpleStrikeNPC(9999, 1, crit: false, NPC.knockBackResist);
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
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowChestKey>(), 2, 1, 1));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 2, 1, 5));
		
			
		}
		public override void HitEffect(NPC.HitInfo hit)
		{

			
			for (int i = 0; i < 5; i++)
			{
				Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
				ParticleManager.NewParticle(NPC.Center, speed * 4, ParticleManager.NewInstance<morrowstar>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
			}
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Guardians who are ready to die for Gothivia?"))
			});
		}
	}
}