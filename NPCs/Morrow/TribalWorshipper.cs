using Stellamod.Assets.Biomes;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Utilis;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
	public class TribalWorshipper : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrowed Swampster");
			Main.npcFrameCount[NPC.type] = 13;
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
			NPC.width = 46;
			NPC.height = 50;
			NPC.damage = 42;
			NPC.defense = 8;
			NPC.lifeMax = 75;
			NPC.HitSound = SoundID.NPCHit5;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
			NPC.value = 563f;
			NPC.knockBackResist = .45f;
			NPC.aiStyle = 3;
			AIType = NPCID.SnowFlinx;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            if (spawnInfo.Player.InModBiome<FableBiome>())
            {
                return SpawnCondition.Overworld.Chance * 0.5f;
            }
            return 0f;
		}
		int invisibilityTimer;
		int invsTimer;
		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 11; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 1, -1f, 1, default, .61f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.15f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			if (Main.dayTime)
            {
				NPC.damage = 0;
				
			}
			timer++;
			invsTimer++;
			NPC.spriteDirection = NPC.direction;


			if (invsTimer < 255)
            {
				NPC.alpha++;
            }

			if (invsTimer > 255)
			{
				NPC.alpha--;
			}

			if (invsTimer > 510)
			{
				invsTimer = 0;
			}

			invisibilityTimer++;
			if (invisibilityTimer >= 100)
			{
				Speed();

				for (int k = 0; k < 11; k++)
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenMoss, NPC.direction, -1f, 1, default, .61f);


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
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), 2, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Morrowshroom>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OvermorrowWood>(), 1, 1, 5));
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

		public void Speed()
		{
			timer++;


			if (timer > 50)
			{

				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, NPC.direction, -1f, 1, default, .61f);
				}

				if (!Main.dayTime)
				{
					for (int k = 0; k < 5; k++)
					{
						Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, NPC.direction, -1f, 1, default, .61f);

						float speedXB = NPC.velocity.X * Main.rand.NextFloat(-0.5f, 0.5f);
						float speedY = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(-4, 4) * 0f;
						if (StellaMultiplayer.IsHost)
						{
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X, NPC.position.Y, speedXB * 3, speedY, ProjectileID.GreekFire3,
                                        15, 0f, Main.myPlayer);
                        }
		
					}
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