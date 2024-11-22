using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Ores;
using Stellamod.NPCs.Colosseum.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum
{
    internal class GintzeTumbleWeed : BaseColosseumNPC
    {
        private int _frame;
        private enum AIState
        {
            Chase,
            Jump
        }
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private Player Target => Main.player[NPC.target];
        private float DirectionToTarget
        {
            get
            {
                if (Target.Center.X < NPC.Center.X)
                {
                    return -1;
                }
                return 1;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 31;
            NPC.defense = 8;
            NPC.value = 65f;
            NPC.width = 32;
            NPC.height = 32;
            NPC.knockBackResist = 0.55f;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return Target.Bottom.Y - 16 > NPC.Bottom.Y;
        }

        public override void AI()
        {
            base.AI();
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
            }

            NPC.rotation += 0.05f;
            NPC.rotation += NPC.velocity.X * 0.1f;
        }

        private void AI_Chase()
        {
            Timer++;
            float moveSpeed = 3;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.24f);

            bool jumpWhenBelowPlayer = Target.Bottom.Y < NPC.Top.Y && NPC.collideY;
            bool jumpWhenBouncing = NPC.collideY;
            if (jumpWhenBelowPlayer)
            {
                SwitchState(AIState.Jump);
            }
            if(jumpWhenBouncing)
            {
                NPC.velocity.Y = -NPC.velocity.Y;
            }
        }

        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 12;
            }

            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Chase);
            }

            //Failsafe
            if (Timer > 120)
            {
                SwitchState(AIState.Chase);
            }
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 4; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
                    Main.dust[num].noGravity = true;
                    Dust expr_62_cp_0 = Main.dust[num];
                    expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    Dust expr_92_cp_0 = Main.dust[num];
                    expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    if (Main.dust[num].position != NPC.Center)
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzlMetal>(), 1, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
        }
    }
}
