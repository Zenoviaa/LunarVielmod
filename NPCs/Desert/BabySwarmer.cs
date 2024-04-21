using Microsoft.Xna.Framework;
using Stellamod.DropRules;
using Stellamod.Items.Accessories.Wings;
using Stellamod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Desert
{
    internal class BabySwarmer : ModNPC
    {
        private float Speed
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float WanderX
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float WanderY
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 34;
            NPC.damage = 54;
            NPC.defense = 8;
            NPC.lifeMax = 333;
            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.npcSlots = 0.2f;
        }

        private void AI_MoveToward(Vector2 targetCenter, float maxSpeed = 12)
        {
            //chase target
            float acceleration = 1;

            //Accelerate
            Speed += acceleration;
            Speed = MathHelper.Clamp(Speed, 0, maxSpeed);

            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            Vector2 targetVelocity = directionToTarget * Speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = -NPC.direction;
            if (StellaMultiplayer.IsHost && Main.rand.NextBool(20))
            {
                Speed /= 2;
                WanderX = Main.rand.NextFloat(-10f, 10f);
                WanderY = Main.rand.NextFloat(-10f, 10f);
                NPC.netUpdate = true;
            }

            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget && 
                Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
            {
                Vector2 targetCenter = target.Center + new Vector2(WanderX * 20, WanderY * 20);
                AI_MoveToward(targetCenter, maxSpeed: 6);
            }
            else
            {


                Vector2 targetCenter = NPC.Center + new Vector2(WanderX, WanderY);
                AI_MoveToward(targetCenter, maxSpeed: 3);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.34f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.AntlionMandible, 3, 1, 2));
            npcLoot.Add(ItemDropRule.ByCondition(new PlanteraDropRule(), ModContent.ItemType<BabySwarmerStaff>(), 20, 1, 1));
            npcLoot.Add(ItemDropRule.ByCondition(new PlanteraDropRule(), ModContent.ItemType<AntlionWings>(), 40, 1, 1));
        }
    }
}
