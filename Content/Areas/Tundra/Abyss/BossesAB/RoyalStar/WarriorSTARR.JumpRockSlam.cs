using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private ref Vector2 JumpStartPosition => ref _vector1;
    private ref Vector2 JumpTargetPosition => ref _vector2;
    private float Num_Falling_Rocks => 4;
    private float Jump_Rock_Prep_Time => 40;
    private float Jump_Ready_Time => 60;
    private int Jump_Rock_Crash_Damage => 45;
    private int Jump_Rock_Rock_Damage => 32;
    private float Jump_Rock_Punch_Out_Time => 60;
    private void AI_JumpRockSlam()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        GruntSound();
                    }

                    StayGrounded();
                    FaceTarget();
                    this.AseAnimator.PlayAnimation(ANIM_CROUCH, AnimationParams.NoLooping);
                    if (Timer >= Jump_Rock_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        BigGruntSound();
                        JumpStartPosition = NPC.Center;
                        JumpTargetPosition = MyTarget.Center - Vector2.UnitY * 64;
                    }

                    _outliner.warning = true;
                    MakeJumpingParticles();
                    _jumpingTrail = true;
                    float xDirection = MathF.Sign(JumpTargetPosition.X - JumpStartPosition.X);
                    Vector2 upward = (JumpTargetPosition - JumpStartPosition).RotatedBy(MathHelper.PiOver2 * -xDirection).SafeNormalize(Vector2.Zero);
                    Vector2 pullPoint = JumpStartPosition + JumpTargetPosition;
                    pullPoint *= 0.5f;
                    pullPoint += upward * 80;

                    float ratio = Timer / Jump_Ready_Time;
                    float easeOut = EasingFunction.OutExpo(ratio);
                    Vector2 lerp1 = Vector2.Lerp(JumpStartPosition, pullPoint, easeOut);
                    Vector2 lerp2 = Vector2.Lerp(pullPoint, JumpTargetPosition, easeOut);
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 targetVelocity = lerp3 - NPC.Center;
                    NPC.velocity = targetVelocity;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN_READY, AnimationParams.NoLooping);
                    if (Timer >= Jump_Ready_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _outliner.attacking = true;
                    NPC.velocity.X *= 0.5f;
                    if (NPC.velocity.Y < -0.1f)
                        NPC.velocity.Y *= 0.92f;
                    else if (NPC.velocity.Y < 1)
                        NPC.velocity.Y += 0.2f;
                    else if (NPC.velocity.Y < 16)
                        NPC.velocity.Y *= 1.1f;
                    _jumpingTrail = true;
                    MakeFallingCrashParticles();
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN, AnimationParams.NoLooping);
                    if (IsGrounded())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if (Timer == 1)
                    {

                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer crashFirer = ProjFirer.From<STARROCKCRASHSLAM>(NPC);
                            crashFirer.position = NPC.Bottom;
                            crashFirer.velocity = -Vector2.UnitY;
                            crashFirer.damage = Jump_Rock_Crash_Damage;
                            crashFirer.knockback = 1;
                            crashFirer.New();
                            for (float i = 0; i < Num_Falling_Rocks; i++)
                            {
                                ProjFirer fallingRockFirer = ProjFirer.From<STARROCKCRASH>(NPC);
                                fallingRockFirer.position = NPC.Bottom;
                                fallingRockFirer.velocity = -Vector2.UnitY * 15;
                                fallingRockFirer.velocity = fallingRockFirer.velocity.RotatedBy(MathHelper.Lerp(-45f, 45f, i / Num_Falling_Rocks));
                                fallingRockFirer.damage = Jump_Rock_Rock_Damage;
                                fallingRockFirer.New();
                            }
                        }
                    }
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_DOWN_OUT, AnimationParams.NoLooping);
                    if(Timer >= Jump_Rock_Punch_Out_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    SwitchState(AIState.CapeOut);
                }
                break;
        }
    }

}
