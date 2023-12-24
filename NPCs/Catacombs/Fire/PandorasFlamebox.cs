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

namespace Stellamod.NPCs.Catacombs.Fire
{

	[AutoloadBossHead]
	public class PandorasFlamebox : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 60;
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
			NPC.height = 25;
			NPC.damage = 40;
			NPC.defense = 8;
			NPC.lifeMax = 5000;
			NPC.HitSound = SoundID.NPCHit34;
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
			NPC.BossBar = ModContent.GetInstance<BossBarTest2>();
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
					rect = new(0, 0, 44, 30 * 50);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 30, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
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

	

		public override void AI()
		{

			
			NPC.spriteDirection = NPC.direction;

			invisibilityTimer++;
		

			switch (State)
			{

				case ActionState.Wait:
					counter++;
					Wait();
					NPC.velocity *= 0.98f;
					break;

				case ActionState.Speed:
					counter++;
					Speed();
					NPC.aiStyle = 7;
					AIType = NPCID.PartyGirl;
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


		public void Wait()
		{
			timer++;

			if (timer == 50)
			{

				var entitySource = NPC.GetSource_FromAI();
				timer++;


				switch (Main.rand.Next(4))
				{
					case 0:
                        {
							int index = NPC.NewNPC(entitySource, (int)NPC.Center.X - 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasGuard>());
							int index2 = NPC.NewNPC(entitySource, (int)NPC.Center.X + 20, (int)NPC.Center.Y - 40, ModContent.NPCType<PandorasGuard>());
							// Now that the minion is spawned, we need to prepare it with data that is necessary for it to work
							// This is not required usually if you simply spawn NPCs, but because the minion is tied to the body, we need to pass this information to it
							// Finally, syncing, only sync on server and if the NPC actually exists (Main.maxNPCs is the index of a dummy NPC, there is no point syncing it)
							if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: index);
							}

							if (Main.netMode == NetmodeID.Server && index2 < Main.maxNPCs)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: index2);
							}

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
							if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: index);
							}

							if (Main.netMode == NetmodeID.Server && index2 < Main.maxNPCs)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: index2);
							}

							if (Main.netMode == NetmodeID.Server && index3 < Main.maxNPCs)
							{
								NetMessage.SendData(MessageID.SyncNPC, number: index3);
							}

						}
						break;
					case 2:
						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
					case 3:

						NPC.velocity = new Vector2(NPC.direction * 0, -10f);
						break;
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