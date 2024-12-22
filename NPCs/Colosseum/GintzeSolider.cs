using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.NPCs.Colosseum.Common;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum
{
    public class GintzeSolider : BaseColosseumNPC
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
            Main.npcFrameCount[NPC.type] = 11;
        }

        public override void SetDefaults()
        {
            NPC.width = 50; // The width of the npc's hitbox (in pixels)
            NPC.height = 52; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 37; // The amount of damage that this npc deals
            NPC.defense = 0; // The amount of defense that this npc has
            NPC.lifeMax = 180; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 5f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.25f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if (_frame >= Main.npcFrameCount[Type])
            {
                _frame = 0;
            }
            switch (State)
            {
                case AIState.Jump:
                    _frame = 6;
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return Target.Bottom.Y - 16 > NPC.Bottom.Y;
        }

        public override void AI()
        {
            base.AI();
            if (!IsColosseumActive())
            {
                DespawnExplosion();
            }

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
        }

        private void AI_Chase()
        {
            Timer++;
            float moveSpeed = 2;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            if (Target.Top.Y < NPC.Top.Y && NPC.collideY)
            {
                SwitchState(AIState.Jump);
            }
        }

        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 10;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gofria's ranks, be careful"))
            });
        }
    }
}