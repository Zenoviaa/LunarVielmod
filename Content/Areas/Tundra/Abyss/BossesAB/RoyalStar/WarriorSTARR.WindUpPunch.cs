using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core.TriggersSystem;
using Stellamod.Visual.Particles;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{

    private float Wind_Up_Speed_Up_Mult => MathHelper.Lerp(1f, 0.75f, EasingFunction.InOutSine(AttackCounter / 5f));
    private float Wind_Up_Punch_Count => 7;
    private float Wind_Up_Punch_Teleport_In_Time => 35;
    private float Wind_Up_Punch_Prep_Time => 50;
    private float Wind_Up_Punch_Time => 64;
    private float Wind_Up_Punch_End_Time => 25;
    private int Wind_Up_Punch_Damage => 35;
    private void SetPunchPositions()
    {
        Vector2 direction = NPC.Center.X > MyTarget.Center.X ? Vector2.UnitX : -Vector2.UnitX;
        direction *= 256;

        Vector2 positionToMoveTo = MyTarget.Center + direction;
        positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
        EasingFunction.InOutSine(1f));

        if (AttackCounter == 0)

        {
            positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
  EasingFunction.InOutSine(1f));
        }

        StartDashPosition = positionToMoveTo;

        EndDashPosition = MyTarget.Center;
        Vector2 dir = EndDashPosition - StartDashPosition;
        dir = dir.SafeNormalize(Vector2.Zero);
        EndDashPosition += -dir * 64;
    }
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
                    Vector2 dir = EndDashPosition - StartDashPosition;
                    dir = dir.SafeNormalize(Vector2.Zero);
                    EndDashPosition += -dir * 64;

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
                        SetPunchPositions();
                        if (MultiplayerHelper.IsHost)
                        {
                            _fakeOut = Main.rand.NextBool(3);
                            NPC.netUpdate = true;
                        }
                        _initialVelocity = NPC.velocity;
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
                    _outliner.warning = true;
                    MakeFallingCrashParticles();
                    MakeCometParticles(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);

                    if (Main.rand.NextBool(4))
                    {
                        Vector2 startPos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                        var sp = SparkleParticle.Spawn(startPos, -NPC.velocity * 0.05f);
                        sp.outerColor = Color.Gold;
                        sp.innerColor = Color.LightGoldenrodYellow;
                        sp.dampening = 0.05f;
                        sp.Scale *= 0.3f;
                        sp.fast = true;
                        sp.gravity = 0;
                    }
                    float time = Wind_Up_Punch_Time;
                    time *= Wind_Up_Speed_Up_Mult;

                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutExpo(ratio);
                    Vector2 dir = StartDashPosition - EndDashPosition;
                    dir = dir.SafeNormalize(Vector2.Zero);
                    Vector2 midPoint = StartDashPosition + dir * 256;

                    Vector2 lerp1 = Vector2.Lerp(StartDashPosition, midPoint, EasingFunction.OutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(midPoint, EndDashPosition, EasingFunction.InExpo(ratio * 0.98f));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 vel = lerp3 - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(Timer / 45f));
                    NPC.spriteDirection = vel.X > 0 ? 1 : -1;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);

                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    if (_fakeOut && Timer >= time * 0.9f)
                    {
                        Timer = 0;
                        AttackCycle = 4;
                    }
                }
                break;
            case 2:
                {
                    _outliner.attacking = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    if(Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        StartDashPosition = NPC.Center;
                        Vector2 direction = NPC.velocity.X > 0 ? -Vector2.UnitX : Vector2.UnitX;
                        EndDashPosition = StartDashPosition + direction * 128;
                    }

                    if (Timer == 10)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 direction = NPC.spriteDirection > 0 ? Vector2.UnitX : -Vector2.UnitX;
                            ProjFirer firer = ProjFirer.From<STARPUNCH>(NPC);
                            firer.position = NPC.Center + direction * 90;
                            firer.velocity = direction;
                            firer.damage = Wind_Up_Punch_Damage;
                            firer.knockback = 1;
                            firer.New();
                        }
     
                        AttackCounter++;
             
                    }
                    if(Timer == 9)
                    {
                        Vector2 dir = EndDashPosition - StartDashPosition;
                        dir = dir.SafeNormalize(Vector2.Zero);
                        NPC.velocity = dir * 8;
                    }
                    else if (Timer < 9)
                    {

                        NPC.velocity *= 0.23f;
                    }
                    else
                    {
                        NPC.velocity *= 0.94f;
                    }


                    if (Timer >= Wind_Up_Punch_End_Time)
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

                    }


                    float time = Wind_Up_Punch_End_Time;
                    if(Timer > time * 0.5f)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_BACK, AnimationParams.NoLooping);
                    }
                    NPC.velocity *= 0.88f;
                    if (Timer >= time)
                    {
                        if (AttackCounter < Wind_Up_Punch_Count)
                        {
                            Timer = 0;
                            AttackCycle = 1;
                        }
                        else
                        {
                            ChooseAttack();
                        }

                    }

                }
                break;
            case 4:
                {
                    if (Timer == 1)
                    {
    
                        _initialVelocity = NPC.velocity;
                        NPC.TargetClosest();
                        StartDashPosition.Y = -12;
                        float xDirection = MathF.Sign(NPC.velocity.X);
                        StartDashPosition.X = -xDirection * 5;
                    }
                    _jumpingTrail = true;
                    _afterImages = true;
                    _outliner.attacking = true;
                    FaceTarget();
                    StarBitDust();
                    NPC.noTileCollide = false;
                    StartDashPosition.X *= 0.96f;
                    StartDashPosition.Y += 0.2f;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, StartDashPosition, EasingFunction.InOutSine(Timer / 42f));
                    if(Timer < 30)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_JUMP, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_DISC_THROW, AnimationParams.NoLooping);
                        if (Timer == 42)
                        {
                            StartDashPosition = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero) * 8;
                            if (MultiplayerHelper.IsHost)
                            {
                                Vector2 throwDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                for (float f = 0; f < Disc_Throw_Count; f++)
                                {
                                    float radians = MathHelper.Lerp(-Disc_Spread, Disc_Spread, f / Disc_Throw_Count);
                                    Vector2 newDirection = throwDirection.RotatedBy(radians);
                                    ProjFirer discFirer = ProjFirer.From<STARDISC>(NPC);
                                    discFirer.damage = Disc_Throw_Damage;
                                    discFirer.velocity = newDirection * 4;
                                    discFirer.knockback = 1;
                                    discFirer.New();
                                }
                            }
                        }
                    }

                    _bigStarAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 72f));
                    if (Timer >= 96)
                    {
                        Timer = 0;
                        AttackCycle = 1;
                    }
                }
                break;
        }
    }
}
