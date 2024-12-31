using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Morrow
{
    public abstract class BaseBeetleNPC : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        private Player Target => Main.player[NPC.target]; 

        public float MaxXSpeed = 4;
        public float MaxYSpeed = 1;
        public float Acceleration = 0.06f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ruby Beetle");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 270;
            NPC.noGravity = true;
            NPC.value = 90f;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.75f;
            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.1f;
            if(NPC.Center.X < Target.Center.X)
            {
                float accel = Acceleration;
                float distance = Target.Center.X - NPC.Center.X;
                if(accel > distance)
                {
                    //Try to slow down
                    accel *= 0.5f;
                }
          
                if(NPC.velocity.X < MaxXSpeed)
                {
                    NPC.velocity.X += accel;
                }
            } 
            else if (NPC.Center.X > Target.Center.X)
            {
                float accel = Acceleration;
                float distance = NPC.Center.X - Target.Center.X;
                if (accel > distance)
                {
                    //Try to slow down
                    accel *= 0.5f;
                }

                if(NPC.velocity.X > -MaxXSpeed)
                {
                    NPC.velocity.X -= accel;
                }
            }

            if (NPC.Center.X < Target.Center.X)
            {
                float accel = Acceleration;
                float distance = Target.Center.Y - NPC.Center.Y;
                if (accel > distance)
                {
                    //Try to slow down
                    accel *= 0.5f;
                }

                if (NPC.velocity.Y < MaxYSpeed)
                {
                    NPC.velocity.Y += accel;
                }
            }
            else if(NPC.Center.Y > Target.Center.Y)
            {
                float accel = Acceleration;
                float distance = NPC.Center.Y - Target.Center.Y;
                if (accel > distance)
                {
                    //Try to slow down
                    accel *= 0.5f;
                }

                if (NPC.velocity.Y > -MaxYSpeed)
                {
                    NPC.velocity.Y -= accel;
                }
            }

            //NPC.velocity.Y += MathF.Sin(Timer * 0.05f); 
        }
    }
}
