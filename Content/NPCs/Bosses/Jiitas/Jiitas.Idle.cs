using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        private readonly Queue<ActionState> _attackCycle = new Queue<ActionState>();
        public static float IdleTime
        {
            get
            {
                return 140;
            }
        }

        private void AI_Idle()
        {
            PrimaryDrawAlpha += 0.02f;
            if (PrimaryDrawAlpha >= 1f)
                PrimaryDrawAlpha = 1f;
            NoWarn();
            PlayAnimation(AnimationState.Idle);
            //We can't have you moving lol
            //Idle - Hovers around in place a little, like wiggles around like a floating puppet, will jump a few times before doing an attack     
            //Slow down if moving too fast
            if (NPC.velocity.Length() > 3)
            {
                NPC.velocity *= 0.98f;
            }
            if (NPC.velocity.Length() <= 0.2f)
            {
                NPC.velocity += DirectionToTarget;
            }
            NPC.velocity = NPC.velocity.RotatedBy(0.01f);
            ShouldDealContactDamage = false;

            //Slowly hover towards player
            float yDirectionToTarget = NPC.Bottom.Y < Target.Bottom.Y ? -1 : 1;
            NPC.noGravity = true;
            NPC.velocity.Y -= yDirectionToTarget * 0.02f;

            //Wobble
            float rotationInterpolant = ExtraMath.Osc(0f, 1f);
            float targetRotation = MathHelper.Lerp(-0.05f, 0.05f, rotationInterpolant);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);

            Timer++;
            float idleTime = IdleTime;
            if (InPhase2)
                idleTime *= 0.6f;
            if (Timer >= idleTime)
            {
                //Decide attack
                ChooseAttack();
            }
        }

        private void ChooseAttack()
        {

            if (_attackCycle.Count == 0)
            {
                //Determine the attack pattern
                if (MultiplayerHelper.IsHost)
                {
                    int randIndex = Main.rand.Next(0, 3);
                    switch (randIndex)
                    {
                        case 0:
                            _attackCycle.Enqueue(ActionState.KnifeSpin);
                            _attackCycle.Enqueue(ActionState.SpinJump);
                            if (InPhase2)
                            {
                                _attackCycle.Enqueue(ActionState.BombDrop);
                            }
                            _attackCycle.Enqueue(ActionState.MachineGunSurprise);
                            _attackCycle.Enqueue(ActionState.Fakeout);
                            break;
                        case 1:
                            _attackCycle.Enqueue(ActionState.KnifeSpin);
                            _attackCycle.Enqueue(ActionState.SpinJump);
                            _attackCycle.Enqueue(ActionState.MachineGunSurprise);
                            _attackCycle.Enqueue(ActionState.KnifeSpin);
                            break;
                        case 2:
                            _attackCycle.Enqueue(ActionState.BombsAway);
                            _attackCycle.Enqueue(ActionState.SpinJump);
                            if (InPhase2)
                            {
                                _attackCycle.Enqueue(ActionState.BombDrop);
                            }
                            _attackCycle.Enqueue(ActionState.MachineGunSurprise);
                            _attackCycle.Enqueue(ActionState.BombsAway);
                            break;
                    }

                }
            }
            if (InPhase2 && Main.rand.NextBool(2) && !Empowered)
            {
                SwitchState(ActionState.Empower);
            }
            else
            {
                if (_attackCycle.Count > 0)
                {
                    ActionState state = _attackCycle.Dequeue();
                    SwitchState(state);
                }
            }

            if (!HasPhaseTransitioned && InPhase2)
            {
                SwitchState(ActionState.Phase2Transition);
            }
        }
    }
}
