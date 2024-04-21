using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Desert
{
	public class Shrewmet : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 30;
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

		// AI counter
		public int counter;

		public ActionState State = ActionState.Wait;
		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 16;
			NPC.damage = 20;
			NPC.defense = 1;
			NPC.lifeMax = 80;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 563f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 38;
			AIType = NPCID.MisterStabby;
			NPC.noTileCollide = false;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDayDesert.Chance * 0.8f;
		}

		int invisibilityTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand, 1, -1f, 1, default, .61f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.3f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{

			timer++;
			NPC.spriteDirection = NPC.direction;

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LavaMoss, NPC.direction, -1f, 1, default, .61f);


				invisibilityTimer = 0;
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
					NPC.velocity *= 0.98f;
					break;


				default:
					counter++;
					break;
			}
		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Amber, 5, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 2, 1, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArncharChunk>(), 5, 1, 5));



		}


		public void Wait()
		{
			timer++;

			if (timer > 50)
			{

				NPC.oldVelocity *= 0.99f;



			}
			else if (timer == 60)
			{
				State = ActionState.Speed;
				timer = 0;
			}
		}
		public float ty = 0;
		public float Spawner = 0;
		private int attackCounter;

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(attackCounter);
		}
		public void Speed()
		{
			timer++;
			Spawner++;
			/*
			Player players = Main.player[NPC.target];
			if (Spawner == 2)

			{



				int distanceY = Main.rand.Next(-250, -250);
				NPC.position.X = players.Center.X;
				NPC.position.Y = players.Center.Y + distanceY;

			}*/
			ty++;


			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (attackCounter > 0)
				{
					attackCounter--; // tick down the attack counter.
				}

				Player target = Main.player[NPC.target];

			
				// If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
				if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 300 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
				{
					Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
					direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

					int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 3,
						ProjectileID.SandBallFalling, 15, 0, Main.myPlayer);
					int projectile2 = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 2,
						ProjectileID.SandBallFalling, 15, 0, Main.myPlayer);
					Main.projectile[projectile].timeLeft = 300;
					Projectile ichor = Main.projectile[projectile];
					ichor.hostile = true;
					ichor.friendly = false;


					attackCounter = 200;
					NPC.netUpdate = true;
				}
			}
		


			if (timer > 50)
			{

				NPC.velocity.X *= 1f;
				NPC.velocity.Y *= 0.5f;
				for (int k = 0; k < 1; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, NPC.direction, -1f, 1, default, .61f);

					float speedXB = NPC.velocity.X * Main.rand.NextFloat(-1f, 1f);
					float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, 4) * 0f;
					

				}





			}

			if (timer == 100)
			{
				State = ActionState.Wait;
				timer = 0;
			}

		}
	}
}