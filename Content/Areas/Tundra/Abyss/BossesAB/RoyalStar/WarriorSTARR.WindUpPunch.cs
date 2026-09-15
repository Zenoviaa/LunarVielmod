using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Visual.Particles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{

    private float Wind_Up_Speed_Up_Mult => MathHelper.Lerp(1f, 0.5f, EasingFunction.InOutSine(AttackCounter / 5f));
    private float Wind_Up_Punch_Count => 7;
    private float Wind_Up_Punch_Teleport_In_Time => 35;
    private float Wind_Up_Punch_Prep_Time => 50;
    private float Wind_Up_Punch_Time => 24;
    private float Wind_Up_Punch_End_Time => 25;
    private int Wind_Up_Punch_Damage => 35;
    private void AI_WindUpPunch()
    {
        CreateFootsteps();
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        GruntSound();
                        _initialVelocity = NPC.velocity;
                    }
                    float time = Wind_Up_Punch_Teleport_In_Time;
                    if (AttackCounter == 0)
                        time *= 2;
                    Vector2 direction = NPC.Center.X > MyTarget.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    direction *= 256;
             
                    Vector2 positionToMoveTo = MyTarget.Center + direction;
                    positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
                    EasingFunction.InOutSine(Timer / Wind_Up_Punch_Teleport_In_Time));

                    if (AttackCounter == 0)

                    {
                        positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
              EasingFunction.InOutSine(Timer / Wind_Up_Punch_Teleport_In_Time));
                    }
                        
                    StartDashPosition = positionToMoveTo;
                
                    EndDashPosition = MyTarget.Center;
                    FaceTarget();
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutExpo(Timer / time));
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);

                    _afterImages = true;

                    if(Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    //So first he randomly teleports o either the left or right side o you
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        BigGruntSound();
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<STARROCKDASH>(NPC);
                            firer.damage = Wind_Up_Punch_Damage;
                            firer.knockback = 1;
                            firer.New();
                        }
                    }

                    _afterImages = true;
                    _jumpingTrail = true;
                    _outliner.attacking = true;
                    MakeFallingCrashParticles();
                    MakeCometParticles(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);

                    if (Main.rand.NextBool(2))
                    {
                        Vector2 startPos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                        var sp = SparkleParticle.Spawn(startPos, -NPC.velocity * 0.05f);
                        sp.outerColor = Color.Gold;
                        sp.innerColor = Color.LightGoldenrodYellow;
                        sp.dampening = 0.05f;
                        sp.gravity = 0;
                    }
                    float time = Wind_Up_Punch_Time;
                    time *= Wind_Up_Speed_Up_Mult;
                    float ratio = Timer / time;
                    float ease = EasingFunction.InExpo(ratio);
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, ease);
                    Vector2 vel = pos - NPC.Center;
                    NPC.velocity = vel;
                    NPC.spriteDirection = vel.X > 0 ? 1 : -1;
                    if(ease < 0.25f)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    }
                 
                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    if (_fakeOut && Timer >= time * 0.9f)
                    {
                        SwitchState(AIState.DiscThrow);
                    }
                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<STARPUNCH>(NPC);
                            firer.position = EndDashPosition;
                            firer.damage = Wind_Up_Punch_Damage;
                            firer.knockback = 1;
                            firer.New();
                        }
                        StartDashPosition = NPC.Center;
                        Vector2 direction = NPC.velocity.X > 0 ? -Vector2.UnitX : Vector2.UnitX;
                        EndDashPosition = StartDashPosition + direction * 256;
                  
                        AttackCounter++;
             
                    }

                    float time = Wind_Up_Punch_End_Time;
                    float ratio = Timer / time;
                    float ease = EasingFunction.OutExpo(ratio);
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, ease);
                    pos.Y -= MathHelper.Lerp(0f, 70, EasingFunction.QuadraticBump(ratio));
                    Vector2 vel = pos - NPC.Center;
                    NPC.velocity = vel;
                    if(Timer >= Wind_Up_Punch_End_Time)
                    {
                        if (AttackCounter < Wind_Up_Punch_Count)
                        {
                            Timer = 0;
                            AttackCycle = 0;
                        }
                        else
                        {
                            SwitchState(AIState.CapeOut);
                        }
                 
                    }
                }
                break;
        }
    }
}
