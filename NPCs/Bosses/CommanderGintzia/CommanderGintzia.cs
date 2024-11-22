using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.CommanderGintzia.Hands;
using Stellamod.NPCs.Colosseum.Common;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.CommanderGintzia
{
    [AutoloadBossHead]
    internal class CommanderGintzia : BaseColosseumNPC
    {
        private int _frame;
        private int _comeHereIndex;
        private int _fingerGunIndex;
        private int _fistIndex;
        private int _handShakeIndex;
        private int _okHandIndex;
        private int _openHandIndex;
        private int _scissorHandIndex;
        private int _evilCarpetIndex;
        private enum AIState
        {
            Spawn,
            Idle,
            Summon_Hands,
            Summon_Hands_V2,
            Slam,
            Phase_2_Transition,
            Death,
        }

        private Vector2 FollowCenter;

        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCycle => ref NPC.ai[2];
        private Player Target => Main.player[NPC.target];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_comeHereIndex);
            writer.Write(_fingerGunIndex);
            writer.Write(_fistIndex);
            writer.Write(_handShakeIndex);
            writer.Write(_okHandIndex);
            writer.Write(_openHandIndex);
            writer.Write(_scissorHandIndex);
            writer.Write(_evilCarpetIndex);
            writer.WriteVector2(FollowCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _comeHereIndex = reader.ReadInt32();
            _fingerGunIndex = reader.ReadInt32();
            _fistIndex = reader.ReadInt32();
            _handShakeIndex = reader.ReadInt32();
            _okHandIndex = reader.ReadInt32();
            _openHandIndex = reader.ReadInt32();
            _scissorHandIndex = reader.ReadInt32();
            _evilCarpetIndex = reader.ReadInt32();
            FollowCenter = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 30;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2400;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.BossBar = ModContent.GetInstance<CommanderGintziaBossBar>();

            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gintzicane");
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.5f;
            if(NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            if(_frame >= 30)
            {
                _frame = 0;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if(Timer % 120 == 0)
            {
                FollowCenter = Target.Center;
            }
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Summon_Hands:
                    AI_SummonHands();
                    break;
                case AIState.Summon_Hands_V2:
                    AI_SummonHandsV2();
                    break;
                case AIState.Slam:
                    AI_Slam();
                    break;
                case AIState.Phase_2_Transition:
                    AI_Phase2Transition();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
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

        private void SummonHand(int npcIndex)
        {
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 2;
            npc.netUpdate = true;
        }

        private void AI_Spawn()
        {
            Timer++;
            int xSpawn = (int)NPC.Center.X;
            int ySpawn = (int)NPC.Center.Y;
            float upper = 120f;
            float between = upper / 7;
            void PlaySound()
            {

                SoundStyle soundStyle;
                switch (Main.rand.Next(2))
                {
                    case 0:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GintzSummon");
                        soundStyle.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                        break;
                    case 1:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GintzSummon2");
                        soundStyle.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(soundStyle, NPC.position);
                        break;
                }
            }
            if (Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _scissorHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<EvilCarpet>(),
                        ai2: NPC.whoAmI);
                }
            }
            if (Timer == 10)
            {
                if (StellaMultiplayer.IsHost)
                {
        
                    _comeHereIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<ComeHere>(), 
                        ai2: NPC.whoAmI, 
                        ai3: (between * 1));
                }
                PlaySound();
            }

            if (Timer == 40)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _fingerGunIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<FingerGun>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 2));
                }
                PlaySound();
            }

            if (Timer == 70)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _fistIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<Fist>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 3));
                }
                PlaySound();
            }

            if (Timer == 100)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _handShakeIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<HandShake>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 4));
                }
                PlaySound();
            }
            
            if (Timer == 130)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _okHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<OkHand>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 5));
                }
                PlaySound();
            }
            
            if (Timer == 160)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _openHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<OpenPalm>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 6));
                }
                PlaySound();
            }

            if (Timer == 190)
            {
                if (StellaMultiplayer.IsHost)
                {
                    _scissorHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<ScissorHand>(),
                        ai2: NPC.whoAmI,
                        ai3: (between * 7));
                }
                PlaySound();
            }
 

            if (Timer >= 210)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void FollowTarget()
        {
            Vector2 targetCenter = FollowCenter + new Vector2(0, -212);
            Vector2 velToPlayer = targetCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            float maxSpeed = 24f;
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, targetCenter);
            if (distance < maxSpeed)
            {
                targetVelocity *= distance;
            }
            else
            {
                targetVelocity *= maxSpeed;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.01f);
            NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 0.02f;

            float targetRotation = NPC.velocity.X * 0.025f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
        }

        private void AI_Idle()
        {
            Timer++;
            FollowTarget();
            if (Timer >= 60)
            {   
                SwitchState(AIState.Summon_Hands);
            }
        }

        private void AI_SummonHands()
        {
            Timer++;
            if(Timer == 1)
            {
                switch (AttackCycle)
                {
                    case 0:
                        if (Main.rand.NextBool(2))
                        {
                            SummonHand(_fistIndex);
                        }
                        else
                        {
                            SummonHand(_openHandIndex);
                        }
                 
                        break;
                    case 1:
                        if (Main.rand.NextBool(2))
                        {
                            SummonHand(_comeHereIndex);
                        }
                        else
                        {
                            SummonHand(_fingerGunIndex);
                        }

                        break;
                    case 2:
                        if (Main.rand.NextBool(2))
                        {
                            SummonHand(_okHandIndex);
                        }
                        else
                        {
                            SummonHand(_scissorHandIndex);
                        }
                        break;
                    case 3:
                        SummonHand(_handShakeIndex);
                        break;
                }
                AttackCycle++;
                if(AttackCycle >= 4)
                {
                    AttackCycle = 0;
                }
            }

            FollowTarget();
            if (Timer >= 360)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_SummonHandsV2()
        {

        }

        private void AI_Slam()
        {

        }

        private void AI_Phase2Transition()
        {

        }

        private void AI_Death()
        {

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                float rot = f * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                Vector2 glowDrawPos = drawPos + offset;
                Color glowColor = drawColor * 0.6f;
                spriteBatch.Draw(texture, glowDrawPos, NPC.frame, glowColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            }
            spriteBatch.RestartDefaults();

            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedCommanderGintziaBoss, -1);
        }
    }
}
