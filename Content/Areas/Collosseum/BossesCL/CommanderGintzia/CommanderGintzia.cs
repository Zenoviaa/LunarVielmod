using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Colosseum.Common;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia
{
    public partial class CommanderGintzia : ScarletBoss
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
        private float _hoverTimer;
        private enum AIState
        {
            Spawn,
            Idle,
            Summon_Hands,
            Summon_Hands_V2,
            Slam,
            Land,
            Recover,
            Phase_2_Transition,
            Death,
            Give_Key,
            Despawn
        }

        private Vector2 FollowCenter;

        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackCycle => ref NPC.ai[2];
        private ref float AccelTimer => ref NPC.ai[3];
        private Player Target => Main.player[NPC.target];
        private bool InPhase2 => NPC.life < NPC.lifeMax / 2;
        private bool Phase2Transition;

        private float TransitionColorProgress;
        private int ShockwaveDamage => 40;
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
            writer.Write(Phase2Transition);
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
            Phase2Transition = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 30;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 14;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gintzicane");
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 1f)
            {
                NPC.frameCounter = 0f;
                _frame++;
            }

            int frameWidth = 106;
            switch (State)
            {
                default:
                    frameHeight = 134;
                    if (_frame >= 30)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Slam:
                    frameWidth = 200;
                    frameHeight = 156;
                    if (_frame >= 2)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Land:
                    frameWidth = 200;
                    frameHeight = 156;
                    if (_frame >= 12)
                    {
                        _frame = 11;
                    }
                    break;
            }

            NPC.frame.Width = frameWidth;
            NPC.frame.Height = frameHeight;
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }


        public override bool? CanFallThroughPlatforms()
        {
            if (NPC.HasValidTarget && Target.Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }

            return false;
        }

        public override void AI()
        {
            base.AI();
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _windStorm ??= new WindStorm(3);
            _windStorm?.Update(NPC.Center);
            if(!NPC.HasValidTarget)
                NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
            if (InPhase2)
            {
                Main.windSpeedTarget = -200f * 0.01f;
            }
            NPC.spriteDirection = NPC.direction;
            if (FollowCenter == Vector2.Zero)
                FollowCenter = NPC.Center;
            FollowTarget();
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
                case AIState.Land:
                    AI_Land();
                    break;
                case AIState.Recover:
                    AI_Recover();
                    break;
                case AIState.Give_Key:
                    AI_GiveKey();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void SummonHand(int npcIndex)
        {
            TargetOutlineColor = Color.Yellow;
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 2;
            npc.netUpdate = true;
        }
        private void TransitionHand(int npcIndex)
        {
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 4;
            npc.netUpdate = true;
        }
        private void KillHand(int npcIndex)
        {
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 6;
            npc.netUpdate = true;
        }

        private void PutAwayCarpet(int npcIndex)
        {
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 1;
            npc.netUpdate = true;
        }

        private void ReSummonCarpet(int npcIndex)
        {
            NPC npc = Main.npc[npcIndex];
            npc.ai[1] = 0;
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
                if (MultiplayerHelper.IsHost)
                {
                    _evilCarpetIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<EvilCarpet>(),
                        ai2: NPC.whoAmI);
                }
            }
            if (Timer == 10)
            {
                if (MultiplayerHelper.IsHost)
                {

                    _comeHereIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<ComeHere>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 1);
                }
                PlaySound();
            }

            if (Timer == 40)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _fingerGunIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<FingerGun>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 2);
                }
                PlaySound();
            }

            if (Timer == 70)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _fistIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<Fist>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 3);
                }
                PlaySound();
            }

            if (Timer == 100)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _handShakeIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<HandShake>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 4);
                }
                PlaySound();
            }

            if (Timer == 130)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _okHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<OkHand>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 5);
                }
                PlaySound();
            }

            if (Timer == 160)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _openHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<OpenPalm>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 6);
                }
                PlaySound();
            }

            if (Timer == 190)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _scissorHandIndex = NPC.NewNPC(NPC.GetSource_FromThis(), xSpawn, ySpawn, ModContent.NPCType<ScissorHand>(),
                        ai2: NPC.whoAmI,
                        ai3: between * 7);
                }
                PlaySound();
            }

            if(Timer == 200)
            {
                ShowNamePlate();
            }

            if (Timer >= 210)
            {
               
                SwitchState(AIState.Idle);
            }
        }

        private void FollowTarget()
        {
            Vector2 targetCenter = FollowCenter + new Vector2(0, -64);
            Vector2 velToPlayer = targetCenter - NPC.Center;
            velToPlayer = velToPlayer.SafeNormalize(Vector2.Zero);

            //Home to this point
            AccelTimer++;
            float speedInterpolant = AccelTimer / 60f;

            float easedSpeed = EasingFunction.InOutSine(speedInterpolant);
            float maxSpeed = MathHelper.Lerp(0f, 3f, speedInterpolant);
            Vector2 targetVelocity = velToPlayer;
            float distance = Vector2.Distance(NPC.Center, targetCenter);
            if (distance < 120)
            {
                targetVelocity *= distance;
                NPC.velocity *= 0.98f;

                float targetX = (MathF.Sin(AccelTimer * 0.1f) * 0.5f / 0.5f);
                float targetY = (MathF.Sin(AccelTimer * 0.2f) * 0.5f / 0.5f);

                _hoverTimer++;
                float hovertime = 90;
                if (_hoverTimer >= hovertime)
                    _hoverTimer = hovertime;
                float interp = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_hoverTimer / hovertime));
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetX, interp);
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.X, targetY, interp);
            }
            else
            {
                _hoverTimer --;
                if (_hoverTimer <= 0)
                    _hoverTimer = 0;
                targetVelocity *= maxSpeed;
                NPC.velocity = targetVelocity;
                NPC.velocity.Y += MathF.Sin(Timer * 0.1f) * 1;
            }

            float targetRotation = NPC.velocity.X * 0.025f;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
        }

        private void AI_Idle()
        {
            TargetOutlineColor = Color.Transparent;
            Timer++;
            if (Timer == 1)
            {
                AccelTimer = 0;
                FollowCenter = Target.Center;
                NPC.TargetClosest();
            }
     
            if (Timer >= 200)
            {
                if (InPhase2 && !Phase2Transition)
                {
                    SwitchState(AIState.Phase_2_Transition);
                    Phase2Transition = true;
                }
                else
                {
                    if (InPhase2)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            if (Main.rand.NextBool(7))
                            {
                                SwitchState(AIState.Slam);
                            }
                            else
                            {
                                SwitchState(AIState.Summon_Hands_V2);
                            }
                        }
                    }
                    else
                    {
                        SwitchState(AIState.Summon_Hands);
                    }
                }
            }
        }

        private void AI_SummonHands()
        {
            Timer++;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
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
                    if (AttackCycle >= 4)
                    {
                        AttackCycle = 0;
                    }
                }

            }

            if (Timer >= 360)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_SummonHandsV2()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    switch (AttackCycle)
                    {
                        case 0:
                            SummonHand(_fistIndex);
                            if (Main.rand.NextBool(2))
                            {
                                SummonHand(_fingerGunIndex);
                            }
                            else
                            {
                                SummonHand(_openHandIndex);
                            }

                            break;
                        case 1:
                            SummonHand(_fingerGunIndex);
                            if (Main.rand.NextBool(2))
                            {
                                SummonHand(_comeHereIndex);
                            }
                            else
                            {
                                SummonHand(_okHandIndex);
                            }

                            break;
                        case 2:
                            SummonHand(_scissorHandIndex);
                            if (Main.rand.NextBool(2))
                            {
                                SummonHand(_handShakeIndex);
                            }
                            else
                            {
                                SummonHand(_fistIndex);
                            }
                            break;
                        case 3:
                            SummonHand(_handShakeIndex);
                            SummonHand(_fingerGunIndex);
                            break;
                    }
                    AttackCycle++;
                    if (AttackCycle >= 4)
                    {
                        AttackCycle = 0;
                    }
                }

            }

            if (Timer >= 360)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Slam()
        {
            Timer++;
            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                NPC.velocity.Y = 0;
                NPC.velocity.X = 0;
                NPC.velocity.Y -= 14;
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                PutAwayCarpet(_evilCarpetIndex);
            }

            if (NPC.velocity.Y > 0)
            {
                Vector2 startPos = NPC.position;
                Vector2 endPos = Target.position;

                //Only check vertically
                endPos.X = startPos.X;
                NPC.noTileCollide = !Collision.CanHitLine(startPos, 1, 1, endPos, 1, 1);
            }
            NPC.rotation *= 0.94f;
            if (Timer > 30 && NPC.collideY || Timer > 180)
            {
                SwitchState(AIState.Land);
            }
        }

        private void AI_Land()
        {
            Timer++;
            TargetOutlineColor = Color.Red;
            NPC.velocity.X *= 0.94f;
            NPC.rotation *= 0.94f;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    //This is the part where you spawn the cool ahh shockwaves
                    //But we have to make cool ahh shockwaves :(
                    int shockwaveDamage = ShockwaveDamage;
                    int knockback = 1;
                    Vector2 velocity = Vector2.UnitX;
                    velocity *= 4;
                    Vector2 offset = new Vector2(0, -48);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + offset, velocity,
                        ModContent.ProjectileType<SuperWindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                    velocity = -velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom + offset, velocity,
                   ModContent.ProjectileType<SuperWindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                }

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Verifall");
                soundStyle.PitchVariance = 0.12f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                for (int i = 0; i < 16; i++)
                {
                    Vector2 dustVelocity = -Vector2.UnitY;
                    dustVelocity *= Main.rand.NextFloat(3f, 7f);
                    dustVelocity = dustVelocity.RotatedByRandom(MathHelper.ToRadians(30));
                    Dust.NewDustPerfect(NPC.Bottom, DustID.GemDiamond, dustVelocity);
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 dustVelocity = -Vector2.UnitX;
                    dustVelocity *= Main.rand.NextFloat(3f, 7f);
                    dustVelocity.Y -= Main.rand.NextFloat(1f, 2f);
                    if (i % 2 == 0)
                    {
                        dustVelocity.X = -dustVelocity.X;
                    }
                    Dust.NewDustPerfect(NPC.Bottom, DustID.GemDiamond, dustVelocity);
                }

                FXUtil.ShakeCamera(NPC.position, 1024, 16);
            }


            if (Timer >= 60)
            {
                SwitchState(AIState.Recover);
            }
        }

        private void AI_Recover()
        {
            Timer++;
            if (Timer == 1)
            {
                ReSummonCarpet(_evilCarpetIndex);
            }

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Phase2Transition()
        {
            Timer++;
            if (Timer == 1)
            {
                TransitionHand(_fistIndex);
                TransitionHand(_okHandIndex);
                TransitionHand(_comeHereIndex);
                TransitionHand(_fingerGunIndex);
                TransitionHand(_handShakeIndex);
                TransitionHand(_openHandIndex);
                TransitionHand(_scissorHandIndex);
            }

            if (Timer % 10 == 0)
            {
                Vector2 dustSpawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 dustVelocity = (NPC.Center - dustSpawnPos).SafeNormalize(Vector2.Zero);
                dustVelocity *= 5;
                Dust.NewDustPerfect(dustSpawnPos, DustID.GemDiamond, dustVelocity);
            }

            NPC.velocity.X *= 0.94f;
            NPC.velocity.Y *= 0.94f;
            NPC.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            NPC.rotation *= 0.94f;

            float progress = Timer / 180f;
            float easedProgress = Easing.SpikeOutCirc(progress);
            TransitionColorProgress = easedProgress;
            if (Timer % 16 == 0)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 4);
            }

            if (Timer > 180)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Death()
        {
            Timer++;
            NPC.dontTakeDamage = true;
            if (Timer == 1)
            {
                //Play some sort of sound
                KillHand(_fistIndex);
                KillHand(_okHandIndex);
                KillHand(_comeHereIndex);
                KillHand(_fingerGunIndex);
                KillHand(_handShakeIndex);
                KillHand(_openHandIndex);
                KillHand(_scissorHandIndex);
            }

            if (Timer % 8 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemDiamond);
            }

            NPC.velocity.X *= 0.94f;
            NPC.velocity.Y *= 0.94f;
            NPC.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;

            float progress = Timer / 120f;
            TransitionColorProgress = progress;
            if (Timer >= 120)
            {
                SwitchState(AIState.Give_Key);
            }
        }

        private void AI_GiveKey()
        {
            Timer++;
            ColosseumSystem colosseum = ModContent.GetInstance<ColosseumSystem>();
            Vector2 colosseumWorld = ColosseumWaveManager.GongSpawnWorld;

            Vector2 velocity = (colosseumWorld - NPC.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(NPC.Center, colosseumWorld);
            float maxSpeed = 6;
            if (distance < maxSpeed)
            {
                velocity *= distance;
            }
            else
            {
                velocity *= maxSpeed;
            }


            NPC.rotation = NPC.velocity.X * 0.025f;

            if (Timer < 150)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, velocity, 0.3f);
                NPC.velocity.Y += MathF.Sin(Timer * 0.2f) * 0.1f;
            }

            if (Timer == 150)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(),
                        ModContent.ItemType<VoidKey>(), Main.rand.Next(1, 1));
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }
                if (MultiplayerHelper.IsHost)
                {
                    int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), NPC.getRect(),
                        ModContent.ItemType<CommanderGintziaBossRel>(), Main.rand.Next(1, 1));
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }
            }

            if (Timer > 150)
            {
                if (NPC.velocity.Y > -14)
                {
                    NPC.velocity.Y -= 0.1f;
                }
            }

            if (Timer == 240)
            {
                NPC.SetEventFlagCleared(ref DownedBossSystem.downedCommanderGintziaBoss, -1);
            }
            if (Timer == 241)
            {
                NPC.active = false;
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            if (NPC.velocity.Y > -14)
            {
                NPC.velocity.Y -= 0.1f;
            }
            if(Timer >= 60)
            {
                NPC.active = false;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 2; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
            if (NPC.life <= 0 && State != AIState.Death && State != AIState.Give_Key)
            {
                NPC.life = 1;
                SwitchState(AIState.Death);
            }

            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CommanderGintziaBossRel>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VoidKey>()));
        }
        public override void OnKill()
        {
            base.OnKill();
            ColosseumWaveManager.ColosseumEnemyKilled();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedCommanderGintziaBoss, -1);
        }
    }
}
