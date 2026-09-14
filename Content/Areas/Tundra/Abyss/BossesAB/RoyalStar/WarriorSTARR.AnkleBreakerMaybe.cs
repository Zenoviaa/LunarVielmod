using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR 
{
    private ref Vector2 RunDirection => ref _vector1;
    private ref Vector2 StartFlyingPoint => ref _vector2;

    private int Flying_Or_Ankle_Damage => 30;
    private float Flying_Or_Ankle_Prep_Time => 30;
    private float Flying_Or_Ankle_Flying_Warn_Time => 30;
    private float Flying_Or_Ankle_Flying_Time => 70;
    /*
     * Jumps backwards and then runs towards you after charging for a bit, 
     * he’ll then either do a flying spinning kick (like that one street fighter move)
     * or a low ankle breaker type kick, each telegraphed differently you have to pay attention, 
     * the flying kick will only hit you if you’re airborne and the ankle breaker only hits you if you’re grinded, 
     * it also kicks rocks so it’ll still hit you if you’re far
     */
    private void AI_AnkleBreakerMaybe()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        GruntSound();
                        NPC.TargetClosest();
                        RunDirection = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    }

                    _outliner.warning = true;
                    FaceTarget();

                    float ratio = Timer / Flying_Or_Ankle_Prep_Time;
                    float ease = EasingFunction.OutExpo(ratio);
                    NPC.velocity = Vector2.Lerp(-RunDirection * 8, Vector2.Zero, ease);
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if(Timer >= Flying_Or_Ankle_Prep_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                 
                    }
                    _jumpingTrail = true;
                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_RUN, AnimationParams.Default);
                    float xDir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xDir * 5, 0.1f);
                    FaceTarget();

                    StartFlyingPoint = NPC.Bottom;
                    float xDist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                    if(xDist <= 64 && MultiplayerHelper.IsHost)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Timer = 0;
                            AttackCycle = 31;
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle = 21;
                        }
                        NPC.netUpdate = true;
                    }
                    else
                    {

                        if (Timer <= 60)
                        {
                            Timer = 0;
                            AttackCycle = 21;
                        }
                    }

                }
                break;
            case 21:
                {
                    //telegraph for flying kick
                    _outliner.warning = true;
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_SPIN_KICK_READY, AnimationParams.Default);
                    if(Timer >= Flying_Or_Ankle_Flying_Warn_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 22:
                {
                    if (Timer == 1)
                    {
                        BigGruntSound();
                    }

                    MakeJumpingParticles();

                    //Spinng Kick
                    _jumpingTrail = true;
                    _outliner.attacking = true;
                    float yToMoveTo = StartFlyingPoint.Y + -128;
                    float yVelocity = yToMoveTo - NPC.Center.Y;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, yVelocity, 0.2f);

                    float xDir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xDir * 5, 0.1f);
                    FaceTarget();

                    this.AseAnimator.PlayAnimation(ANIM_SPIN_KICK, AnimationParams.Default);

                    if (Timer >= Flying_Or_Ankle_Flying_Time)
                    {
                        Timer = 0;
                        AttackCycle = 4;
                    }
                }
                break;
            case 31:
                {
                    _outliner.warning = true;
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_LOW_KICK_READY, AnimationParams.Default);
                    if (Timer >= Flying_Or_Ankle_Flying_Warn_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 32:
                {
                    //Low Kick
                    if (Timer == 1)
                    {
                        GruntSound();
                        KickSound();
                        NPC.TargetClosest();
                        float direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                        NPC.velocity.X = direction * 15;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer kickProj = ProjFirer.From<ANKLEBREAKER>(NPC);
                            kickProj.ai0 = NPC.whoAmI;
                            kickProj.ai1 = Flying_Or_Ankle_Flying_Time;
                            kickProj.damage = Flying_Or_Ankle_Damage;
                            kickProj.New();
                        }
                    }

                    MakeJumpingParticles();
                    _outliner.attacking = true;
                    _jumpingTrail = true;
                    NPC.velocity.X *= 0.96f;
                    NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;

                    this.AseAnimator.PlayAnimation(ANIM_LOW_KICK, AnimationParams.Default);
                    if (Timer >= Flying_Or_Ankle_Flying_Time)
                    {
                        Timer = 0;
                        AttackCycle = 4;
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
