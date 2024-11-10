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
    public class TribalPeasant : ModNPC
	{
		// States
		public enum ActionState
		{
			Asleep,
			Notice,
			Jump,
			Fall
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
			Main.npcFrameCount[NPC.type] = 35;
		}

		public override void SetDefaults()
		{
			NPC.width = 42; // The width of the npc's hitbox (in pixels)
			NPC.height = 60; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 30; // The amount of damage that this npc deals
			NPC.defense = 10; // The amount of defense that this npc has
			NPC.lifeMax = 77; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
			NPC.value = 500f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = .5f;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			Player player = spawnInfo.Player;
			if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
			{
				return spawnInfo.Player.ZoneFable() ? 1.0f : 0f;
			}

			return 0f;
		}
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			return State == ActionState.Fall;
        }

        public override void AI()
		{
			switch (State)
			{
				case ActionState.Asleep:
					counter++;
					FallAsleep();
					break;
				case ActionState.Notice:
					counter++;
					Notice();
					break;
				case ActionState.Jump:
					counter++;
					Jump();
					break;
				case ActionState.Fall:
					counter++;
					if (NPC.velocity.Y == 0)
					{
						NPC.velocity.X = 0;
						State = ActionState.Asleep;
						frameCounter = 0;
						frameTick = 0;
					}
					break;
				default:
					counter++;
					break;
			}
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
					rect = new(0, 22 * 84, 72, 8 * 84);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(0, 24), texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 8, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Notice:
					rect = new Rectangle(0, 29 * 84, 72, 6 * 84);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(0, 24), texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 6, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Jump:
					rect = new Rectangle(0, 0, 72, 7 * 84);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(0, 24), texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Fall:
					rect = new Rectangle(0, 7 * 84, 72, 14 * 84);
					spriteBatch.Draw(texture, NPC.position - screenPos - new Vector2(0, 24), texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 14, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
			}
			return false;
		}
		public void FallAsleep()
		{
			// TargetClosest sets npc.target to the player.whoAmI of the closest player.
			// The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
			// This is also automatically flipped if npc.confused.
			NPC.velocity *= 0.98f;
			NPC.TargetClosest(true);			

			// Now we check the make sure the target is still valid and within our specified notice range (500)
			if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 350f)
			{
				// Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
				State = ActionState.Notice;
				ResetTimers();
			}
		}
		public void Notice()
		{
			if (Main.player[NPC.target].Distance(NPC.Center) < 250f)
			{
				timer++;
				if (timer >= 30)
				{
					State = ActionState.Jump;
					ResetTimers();
				}
			}
			else
			{
				NPC.TargetClosest(true);
				timer++;
				if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 450f)
				{
					State = ActionState.Asleep;
					ResetTimers();
				}
			}
		}
		public void Jump()
		{
			timer++;

			if (timer == 9)
			{
				// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            NPC.velocity = new Vector2(NPC.direction * 5, -8f);
                            break;
                        case 1:
                            NPC.velocity = new Vector2(NPC.direction * 3, -7f);
                            break;
                        case 2:
                            NPC.velocity = new Vector2(NPC.direction * 4, -10f);
                            break;
                        case 3:
                            NPC.velocity = new Vector2(NPC.direction * 3, -9f);
                            break;
                    }
                    NPC.netUpdate = true;
                }
			}
			else if (timer > 35)
			{
				// after .66 seconds, we go to the hover state. //TODO, gravity?
				State = ActionState.Fall;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), chanceDenominator: 1, minimumDropped: 2, maximumDropped: 4));
        }

		public override void HitEffect(NPC.HitInfo hit)
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"), NPC.position);
            for (int k = 0; k < 3; k++)
            {
                float rot = Main.rand.NextFloat() * MathHelper.ToRadians(35);

                Vector2 vel = rot.ToRotationVector2() * Main.rand.NextFloat(1, 4);
                vel *= hit.HitDirection;

                float scale = Main.rand.NextFloat(0.5f, 1f);
                Dust.NewDustPerfect(NPC.Center, DustID.Torch, Velocity: vel, Scale: scale);
            }
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Even the weakest, most poor among the warriors in the morrow are still decent foes.."))
			});
		}
	}
}