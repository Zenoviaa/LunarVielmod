using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
	public class GhostCharger : ModNPC
	{


		public int moveSpeed = 0;
		public int moveSpeedY = 0;
		public int counter;
		public ref float AI_State => ref NPC.ai[0];

		public float AiTimer
		{
			get => NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		public ref float AI_FlutterTime => ref NPC.ai[2];

		public int frame = 0;
		public int timer = 0;
		public int timer2 = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Verlia Servant");
			Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modNPCs.

			// Specify the debuffs it is immune to
			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
			{

			});
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 17;
			NPC.damage = 20;
			NPC.defense = 0;
			NPC.lifeMax = 50;
			NPC.noGravity = true;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
		}
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
			if (NPC.Center.X >= player.Center.X && moveSpeed >= -30)
			{
				moveSpeed--;
			}

			if (NPC.Center.X <= player.Center.X && moveSpeed <= 30)
			{
				moveSpeed++;
			}

			NPC.velocity.X = moveSpeed * 0.09f;

			if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -25)
			{
				moveSpeedY--;
				NPC.ai[0] = 150f;
			}

			if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 25)
			{
				moveSpeedY++;
			}

			NPC.velocity.Y = moveSpeedY * 0.14f;
			if (Main.rand.NextBool(220) && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[0] = -25f;
				NPC.netUpdate = true;
			}
			if (counter >= 140)
			{
				counter = 0;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
					direction.Normalize();
					direction.X *= 6f;
					direction.Y *= 6f;


				}
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
