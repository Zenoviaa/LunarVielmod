using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Morrow
{
    public abstract class AbstractBeetle : ModNPC
	{
        public int moveSpeed = 0;
        public int moveSpeedY = 0;
        public int counter;
        public int npcCounter = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
			writer.Write(moveSpeed);
			writer.Write(moveSpeedY);
			writer.Write(counter);
			writer.Write(npcCounter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
			moveSpeed = reader.ReadInt32();
			moveSpeedY = reader.ReadInt32();
			counter = reader.ReadInt32();
			npcCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
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
            Player player = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.X * 0.1f;
            int xSpeed = 21;
            if (NPC.Center.X >= player.Center.X && moveSpeed >= -xSpeed)
            {
                moveSpeed--;
            }

            if (NPC.Center.X <= player.Center.X && moveSpeed <= xSpeed)
            {
                moveSpeed++;
            }

            NPC.velocity.X = moveSpeed * 0.09f;

            if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -50)
            {
                moveSpeedY--;
                NPC.ai[0] = 150f;
            }

            if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 50)
            {
                moveSpeedY++;
            }

            NPC.velocity.Y = moveSpeedY * 0.23f;
            if (counter >= 110 && counter < 140)
            {
                NPC.velocity *= 0.95f;
            }

            if (counter == 140)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 direction = player.Center - NPC.Center;
					direction = direction.SafeNormalize(Vector2.Zero);
                    direction.X *= 5f;
                    direction.Y *= 5f;
                    NPC.velocity = direction;
                }
            }
            if (counter == 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.ai[0] += -25f;
                NPC.velocity = Vector2.Zero;
                counter = 0;
            }


        }


    }
}