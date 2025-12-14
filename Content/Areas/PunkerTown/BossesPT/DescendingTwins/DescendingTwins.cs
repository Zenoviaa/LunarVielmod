using Microsoft.Xna.Framework;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{

    public class DescendingTwins : ScarletBoss
    {
        private enum TwinAttackState
        {
            SummonTwins,
            Idle,
            DashDance_Part1,
            DashDance_Part2,
            TwinFlameSword,
            HighSpeedCrash,
            BouncingDash,
            NodeLay,
            FlameTornado,
            PhaseShift,
            SpeedyDash,
            ElectricBall,
            Death,
        }

        private bool _showNamePlate;
        private ref float Timer => ref NPC.ai[0];

        private int _retinaIndex;
        private int _spazzIndex;
        private bool _phase2;
        private NPC Retina => Main.npc[_retinaIndex];
        private NPC Spazz => Main.npc[_spazzIndex];

        private PatternManager<TwinAttackState> _patternManager;
        private PatternManager<TwinAttackState> PatternManager
        {
            get
            {
                _patternManager ??= new PatternManager<TwinAttackState>(
                    new Tuple<TwinAttackState, float>(TwinAttackState.DashDance_Part1, 1.0f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.BouncingDash, 0.5f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.TwinFlameSword, 1.0f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.HighSpeedCrash, 1.0f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.NodeLay, 0.5f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.FlameTornado, 0.1f),
                    new Tuple<TwinAttackState, float>(TwinAttackState.SpeedyDash, 1.0f));
                return _patternManager;
            }
        }

        public Vector2 GetBouncingDashAnchorPoint()
        {
            return Spazz.Center;
        }

        private bool IsAwaitingCommand(NPC npc)
        {
            DescendingTwin.TwinAIState state = (DescendingTwin.TwinAIState)npc.ai[1];
            if (state == DescendingTwin.TwinAIState.Idle)
                return true;
            return false;
        }

        private void Command(NPC npc, DescendingTwin.TwinAIState state)
        {
            npc.ai[2] = (float)state;
        }

        private bool RetinaAwaitingCommand => IsAwaitingCommand(Retina);
        private bool SpazzAwaitingCommand => IsAwaitingCommand(Spazz);
        private void CommandRetina(DescendingTwin.TwinAIState state) => Command(Retina, state);
        private void CommandSpazz(DescendingTwin.TwinAIState state) => Command(Spazz, state);

        public bool StopFiringAtNodes => SpazzAwaitingCommand;
        private TwinAttackState State
        {
            get => (TwinAttackState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[2];


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_retinaIndex);
            writer.Write(_spazzIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _retinaIndex = reader.ReadInt32();
            _spazzIndex = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 19;
            NPC.lifeMax = 18000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }


        private bool NeedsToTriggerPhase2()
        {
            return !_phase2 && State != TwinAttackState.PhaseShift && NPC.life < NPC.lifeMax / 2f;
        }

        public override void AI()
        {
            base.AI();
         
            if (!_showNamePlate)
            {
                ShowNamePlate();
                _showNamePlate = true;
            }
            if (NPC.life <= 0 && State != TwinAttackState.Death)
            {
                NPC.life = 1;
                SwitchState(TwinAttackState.Death);
            }

            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
            if (State != TwinAttackState.SummonTwins)
            {
                //Shared health pool
                NPC.life = Math.Min(Spazz.life, Retina.life);
                Spazz.life = NPC.life;
                Retina.life = NPC.life;
            }

            if (NeedsToTriggerPhase2())
            {
                SwitchState(TwinAttackState.PhaseShift);
            }

            switch (State)
            {
                case TwinAttackState.SummonTwins:
                    AI_SummonTwins();
                    break;
                case TwinAttackState.Idle:
                    AI_Idle();
                    break;
                case TwinAttackState.DashDance_Part1:
                    AI_DashDancePart1();
                    break;
                case TwinAttackState.DashDance_Part2:
                    AI_DashDancePart2();
                    break;
                case TwinAttackState.TwinFlameSword:
                    AI_TwinFlameSword();
                    break;
                case TwinAttackState.HighSpeedCrash:
                    AI_HighSpeedCrash();
                    break;
                case TwinAttackState.BouncingDash:
                    AI_BouncingDash();
                    break;
                case TwinAttackState.NodeLay:
                    AI_NodeLay();
                    break;
                case TwinAttackState.FlameTornado:
                    AI_FlameTornado();
                    break;
                case TwinAttackState.PhaseShift:
                    AI_PhaseShift();
                    break;
                case TwinAttackState.SpeedyDash:
                    AI_SpeedyDash();
                    break;
                case TwinAttackState.ElectricBall:
                    AI_ElectricBall();
                    break;
                case TwinAttackState.Death:
                    AI_Death();
                    break;
            }
        }


        /// <summary>
        /// Returns the primary color associatiated with the twin
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        public static Color GetTwinColor(int variant)
        {
            switch (variant)
            {
                default:
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Red;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Yellow;
            }
        }
        public static Color GetSecondaryTwinColor(int variant)
        {
            switch (variant)
            {
                default:
                case 0:
                    return Color.GreenYellow;
                case 1:
                    return Color.Yellow;
                case 2:
                    return Color.LightSeaGreen;
                case 3:
                    return Color.Orange;
            }
        }
        private void SwitchState(TwinAttackState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.Death);
                CommandRetina(DescendingTwin.TwinAIState.Death);
            }

            float deathTime = 300f;
            if (Timer % 12 == 0)
            {
                Vector2 spawnPoint = NPC.Top;
                spawnPoint.X += Main.rand.NextFloat(-64f, 64f);
                var fireDust = Dust.NewDustPerfect(spawnPoint, DustID.FireworkFountain_Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                fireDust.noGravity = false;
            }

            NPC.velocity = Vector2.Zero;

            if (Timer >= deathTime)
            {
                NPC.Kill();

            }
        }
        private void AI_SummonTwins()
        {
            Timer++;
            if (Timer == 3)
            {
                if (MultiplayerHelper.IsHost)
                {
                    var source = NPC.GetSource_FromThis();
                    int x = (int)NPC.Center.X;
                    int y = (int)NPC.Center.Y;
                    _retinaIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnRetina,
                        ai2: NPC.whoAmI);

                    _spazzIndex = NPC.NewNPC(source, x, y, ModContent.NPCType<DescendingTwin>(), ai0: 0,
                        ai1: (int)DescendingTwin.TwinAIState.SpawnSpazz,
                        ai2: NPC.whoAmI);

                    SwitchState(TwinAttackState.Idle);
                }
            }
        }

        private void ChooseAttack()
        {
            if (MultiplayerHelper.IsHost)
            {
                SwitchState(PatternManager.NextPattern());
                SwitchState(TwinAttackState.ElectricBall);
            }
        }

        private void AI_Idle()
        {

            //Alright, So nowe have the commander setup, let's get this dash dance attack working
            AttackNumber = 0f;
            if (SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                Timer++;
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                }


                if (Timer >= 60)
                {
                    ChooseAttack();
                }
            }
        }

        private void AI_DashDancePart1()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //So how do we want this to work?
            //It should be pretty simple actually,
            //We're going to have each twin dash 5 times
            //Alternating between each other for a total of 10 dashes
            //Then we'll wait for them to both stop and throw it into the second dash dance
            if (AttackNumber < 10)
            {
                if (Timer >= 60)
                {                //Alternate between the twins and make them dash at you
                                 //The timing between these is based on the twin itself, not the commander
                                 //If you want to make it faster or slower, just edit that
                    if (AttackNumber % 2 == 0)
                    {
                        if (SpazzAwaitingCommand)
                        {
                            CommandSpazz(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    else
                    {
                        if (RetinaAwaitingCommand)
                        {
                            CommandRetina(DescendingTwin.TwinAIState.SimpleDashStart);
                            AttackNumber++;
                        }
                    }
                    Timer = 0;
                }

            }
            else
            {
                SwitchState(TwinAttackState.DashDance_Part2);
            }
        }

        private void AI_DashDancePart2()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //Wait for both of them to finish and then put them into the dash dance state
            if (SpazzAwaitingCommand && RetinaAwaitingCommand)
            {
                CommandSpazz(DescendingTwin.TwinAIState.DashDanceStart);
                CommandRetina(DescendingTwin.TwinAIState.DashDanceStart);
                SwitchState(TwinAttackState.Idle);
            }
        }

        private void AI_TwinFlameSword()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.FlameSwordStart);
                CommandRetina(DescendingTwin.TwinAIState.FlameSwordStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
        private void AI_HighSpeedCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.HighSpeedCrashStart);
                CommandRetina(DescendingTwin.TwinAIState.HighSpeedCrashStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }

        }
        private void AI_BouncingDash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.BouncingDashStartAnchor);
                CommandRetina(DescendingTwin.TwinAIState.BouncingDashStart);
            }

            if (Timer >= 60)
            {
                if (RetinaAwaitingCommand)
                {
                    CommandSpazz(DescendingTwin.TwinAIState.BouncingDashEnd);
                    SwitchState(TwinAttackState.Idle);
                }
            }

        }

        private void AI_NodeLay()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.SpazzNodeLayWindup);
                CommandRetina(DescendingTwin.TwinAIState.RetineNodeLayStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }


        private void AI_FlameTornado()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.FlameTornadoStart);
                CommandRetina(DescendingTwin.TwinAIState.FlameTornadoStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }

        private void AI_PhaseShift()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.PhaseShiftStart);
                CommandRetina(DescendingTwin.TwinAIState.PhaseShiftStart);
            }
            _phase2 = true;
            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
        private void AI_SpeedyDash()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.SpeedyDashStart);
                CommandRetina(DescendingTwin.TwinAIState.SpeedyDashStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
        private void AI_ElectricBall()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CommandSpazz(DescendingTwin.TwinAIState.ElectricBallStart);
                CommandRetina(DescendingTwin.TwinAIState.ElectricBallStart);
            }

            if (Timer >= 60)
            {
                if (SpazzAwaitingCommand && RetinaAwaitingCommand)
                {
                    SwitchState(TwinAttackState.Idle);
                }
            }
        }
    }
}
