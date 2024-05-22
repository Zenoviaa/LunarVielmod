using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Particles;
using Stellamod.UI.Systems;

using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.Jack;

namespace Stellamod.NPCs.Catacombs.Fire
{

	[AutoloadBossHead]
	public class PandorasFlamebox : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 60;
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
		}

		public enum ActionState
		{

			Speed,
			Wait
		}
		// Current state
		public int frameTick;
		public int frameCounter;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 44;
			NPC.height = 50;
			NPC.damage = 40;
			NPC.defense = 8;
			NPC.lifeMax = 5000;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 5000f;
			NPC.knockBackResist = .45f;
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;

			if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CatacombsBoss");
			}
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
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
				case ActionState.Wait:
					rect = new(0, 0, 44, 29 * 50);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 29, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Speed:
					rect = new Rectangle(0, 30 * 50, 44, 29 * 50);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 29, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;


			}
			return false;
		}


		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.DesertTorch, 1, -1f, 1, default, .61f);
			}


		}



		public float Spawner = 0;
		public override void AI()
		{
			Spawner++;

			NPC.TargetClosest();
			if (!NPC.HasValidTarget)
			{
				//WheelMovement(2);
				
				NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, 8), 0.025f);
				NPC.EncourageDespawn(1);
				NPC.noTileCollide = true;
				return;
			}
			else
			{
				NPC.noTileCollide = false;
			}

			NPC.spriteDirection = NPC.direction;

			invisibilityTimer++;
		

			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();
					NPC.velocity *= 0.9f;
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.aiStyle = 3;
					AIType = NPCID.ArmoredSkeleton;
					break;


				default:
					counter++;
					break;
			}
		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			base.ModifyNPCLoot(npcLoot);
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreasureBoxFire>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedPandorasBox, -1);
		}

		public void Wait()
		{
			timer++;

			if (timer == 50)
			{

				var entitySource = NPC.GetSource_FromThis();
				timer++;

				if (StellaMultiplayer.IsHost)
				{

                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            {

                                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasGuard>());
                                int index2 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasGuard>());

                            }


                            break;
                        case 1:
                            {
                                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 20, (int)NPC.Center.Y - 20, ModContent.NPCType<PandorasKnife>());
                                int index2 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 20, (int)NPC.Center.Y - 20, ModContent.NPCType<PandorasKnife>());
                                int index3 = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y - 20, ModContent.NPCType<PandorasKnife>());
                                // Now that the minion is spawned, we need to prepare it with data that is necessary for it to work
                                // This is not required usually if you simply spawn NPCs, but because the minion is tied to the body, we need to pass this information to it
                                // Finally, syncing, only sync on server and if the NPC actually exists (Main.maxNPCs is the index of a dummy NPC, there is no point syncing it)
                            }
                            break;
                        case 2:
                            {
                                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasSeeker>());
                                int index2 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasSeeker>());
  
                            }

                            break;
                        case 3:

                            {
                                int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 20, (int)NPC.Center.Y - 10, ModContent.NPCType<PandorasSeeker>());
                                int index2 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 20, (int)NPC.Center.Y - 10, ModContent.NPCType<PandorasSeeker>());

                                int index3 = NPC.NewNPC(entitySource, (int)NPC.Center.X - 40, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasKnife>());
                                int index4 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 40, (int)NPC.Center.Y - 30, ModContent.NPCType<PandorasKnife>());
                                int index5 = NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y - 30, ModContent.NPCType<PandorasGuard>());
                            }
                            break;
                    }
                }
            }
			else if (timer == 120)
			{
				State = ActionState.Speed;
				timer = 0;
			}
		}

		public void Speed()
		{
			timer++;


			if (timer > 490)
			{

				NPC.velocity.X *= 1.05f;
				NPC.velocity.Y *= 0.5f;
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, NPC.direction, -1f, 1, default, .61f);
				}
			}

			if (timer == 500)
			{
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}