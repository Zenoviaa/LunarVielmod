using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class GhostCharger : ModNPC
	{
		public int moveSpeed = 0;
		public int moveSpeedY = 0;
		public int counter;
		public bool dash = false;
		public short npcCounter = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia Servant");
			Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modNPCs.

			// Specify the debuffs it is immune to
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
		}

		public override void SetDefaults()
		{
			NPC.width = 32;
			NPC.height = 17;
			NPC.damage = 30;
			NPC.defense = 0;
			NPC.lifeMax = 65;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
		}
		public override void AI()
		{
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest();
			}
			Player player = Main.player[NPC.target];
			if (counter == 0)
			{
				if (npcCounter >= 4)
                {
					npcCounter = 0;
					NPC.ai[0] = 150;
				}
			}
			counter++;


			NPC.spriteDirection = NPC.direction;
		
			NPC.rotation = NPC.velocity.X * 0.1f;
			if (!dash && counter < 110)
			{
				
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
			}
			if (counter >= 110 && counter < 140)
            {
				dash = true;
				NPC.velocity *= 0.97f;
			}

			if (counter == 140)
			{
				if (StellaMultiplayer.IsHost)
				{
					Vector2 direction = player.Center - NPC.Center;
					direction.Normalize();
					direction.X *= (Main.rand.NextFloat(5f, 16f));
					direction.Y *= (Main.rand.NextFloat(5f, 13f));
					NPC.velocity = direction;
				}
			}
			if (counter == 170)
            {
				NPC.ai[0] += -25f;
				npcCounter++;
				NPC.velocity = Vector2.Zero;
				counter = 0;
				dash = false;
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
