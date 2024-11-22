using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Foods;
using Stellamod.Items.Armors.Pieces.RareMetals;
using Stellamod.Items.Ores;
using Stellamod.NPCs.Colosseum.Common;
using Stellamod.NPCs.Colosseum.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum
{
    public class GintzeCaptain : BaseColosseumNPC
    {
        private int _frame;
        private enum AIState
        {
            Idle,
            Pace,
            Summon
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

        private float Radius => 256;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 18;
        }

        public override void SetDefaults()
        {
            NPC.width = 66; // The width of the npc's hitbox (in pixels)
            NPC.height = 70; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 1; // The amount of damage that this npc deals
            NPC.defense = 2; // The amount of defense that this npc has
            NPC.lifeMax = 250; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 10f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Pace:
                    AI_Pace();
                    break;
                case AIState.Summon:
                    AI_Summon();
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.33f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (State)
            {
                case AIState.Idle:
                case AIState.Summon:
                    if (_frame < 11)
                    {
                        _frame = 11;
                    }
                    if (_frame >= 18)
                    {
                        _frame = 11;
                    }
                    break;
                case AIState.Pace:
                    if (_frame < 0)
                    {
                        _frame = 0;
                    }

                    if (_frame >= 11)
                    {
                        _frame = 0;
                    }
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        private void AI_Idle()
        {
            Timer++;
            NPC.velocity.X *= 0.92f;
            if (Timer > 120 && NPC.HasValidTarget)
            {
                SwitchState(AIState.Pace);
            }
        }

        private void AI_Pace()
        {
            Timer++;
            float moveSpeed = 0.5f;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);

            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            if (distanceToTarget < Radius)
            {
                SwitchState(AIState.Summon);
            }
        }

        private void AI_Summon()
        {
            Timer++;
            NPC.velocity.X *= 0.92f;
            if (Timer > 30 && Timer % 30 == 0 && Timer < 150)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int damage = 12;
                    int knockback = 2;
                    Vector2 spawnPoint = NPC.Center;
                    spawnPoint.Y -= 48;
                    spawnPoint.X += Main.rand.NextFloat(-24, 24);
                    Vector2 velocity = (Target.Center - spawnPoint).SafeNormalize(Vector2.Zero);
                    velocity *= 7;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, velocity,
                        ModContent.ProjectileType<CaptainSpear>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 240)
            {
                SwitchState(AIState.Idle);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bannerTexture = ModContent.Request<Texture2D>(Texture + "_MiniBanner").Value;
            Vector2 drawOrigin = bannerTexture.Size() / 2f;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 10; i++)
            {
                float f = i;
                float progress = f / 10f;
                float rot = progress * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly * 0.1f;
                float rotation = 0;
                Vector2 offset = rot.ToRotationVector2() * Radius;
                Vector2 drawPos = NPC.Center - screenPos;
                float drawScale = 1f;

                drawPos += offset;
                spriteBatch.Draw(bannerTexture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            return base.PreDraw(spriteBatch, screenPos, drawColor);
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzlMetal>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzeMask>(), 80, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronBar, 5, 1, 7));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10, 1, 3));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gothivia's ranks, be careful"))
            });
        }
    }
}