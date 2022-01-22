using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using static On.Terraria.GameContent.Creative.ItemFilters;
using Stellamod.Items.Materials;

namespace Stellamod.npcs.RobedFlyer
{
	public class RobedFlyer : ModNPC
	{
		int moveSpeed = 0;
		int moveSpeedY = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Robed FLyer");
			Main.npcFrameCount[NPC.type] = 7;
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 34;
			NPC.damage = 16;
			NPC.defense = 10;
			NPC.lifeMax = 80;
			NPC.noGravity = true;
			NPC.value = 90f;
			NPC.buffImmune[BuffID.Poisoned] = true;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;

			
		}
		int counter;
		public override void AI()
		{
			if (counter == 0)
			{
				NPC.ai[0] = 150;
			}
			counter++;
			NPC.spriteDirection = NPC.direction;
			Player player = Main.player[NPC.target];
			NPC.rotation = NPC.velocity.X * 0.1f;
			if (NPC.Center.X >= player.Center.X && moveSpeed >= -60) // flies to players x position
			{
				moveSpeed--;
			}

			if (NPC.Center.X <= player.Center.X && moveSpeed <= 60)
			{
				moveSpeed++;
			}

			NPC.velocity.X = moveSpeed * 0.06f;

			if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -50) //Flies to players Y position
			{
				moveSpeedY--;
				NPC.ai[0] = 150f;
			}

			if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 50)
			{
				moveSpeedY++;
			}

			NPC.velocity.Y = moveSpeedY * 0.12f;
			if (Main.rand.Next(220) == 8 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[0] = -25f;
				NPC.netUpdate = true;
			}
			if (counter >= 240) //Fires desert feathers like a shotgun
			{
				counter = 0;
				
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
					direction.Normalize();
					direction.X *= 11f;
					direction.Y *= 11f;

					int amountOfProjectiles = 4;
					for (int i = 0; i < amountOfProjectiles; ++i)
					{
						float A = (float)Main.rand.Next(-150, 150) * 0.01f;
						float B = (float)Main.rand.Next(-150, 150) * 0.01f;
						int p = Projectile.NewProjectile(NPC.GetProjectileSpawnSource(),NPC.Center.X, NPC.Center.Y, direction.X + A, direction.Y + B, ModContent.ProjectileType<Projectiles.RobedProjectile>(), 11, 1, Main.myPlayer, 0, 0);
						Main.projectile[p].scale = .5f;
					}
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			// we would like this npc to spawn in the overworld.
			return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Robe, 5));
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 3, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RippedFabric>(), 1, 3, 12 ));
		}

		public override void HitEffect(int hitDirection, double damage)
		{

			for (int i = 0; i < 10; i++)
			{
				int dustType = Main.rand.Next(110, 113);
				var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

				dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
				dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

				dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.22f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}
}