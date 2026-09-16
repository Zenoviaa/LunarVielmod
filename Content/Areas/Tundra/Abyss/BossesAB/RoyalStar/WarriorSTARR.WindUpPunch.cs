using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using Stellamod.Core;
using Stellamod.Core.TriggersSystem;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private float _timer2;

    private float Wind_Up_Speed_Up_Mult => MathHelper.Lerp(1f, 0.75f, EasingFunction.InOutSine(AttackCounter / 5f));
    private float Wind_Up_Punch_Count => 7;
    private float Wind_Up_Punch_Teleport_In_Time => 35;
    private float Wind_Up_Punch_Prep_Time => 50;
    private float Wind_Up_Punch_Time => 32;
    private float Wind_Up_Punch_End_Time => 58;
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
    private Vector2 SetPunchPositions(float ratio)
    {
        Vector2 direction = NPC.Center.X > MyTarget.Center.X ? Vector2.UnitX : -Vector2.UnitX;
        direction *= 256;

        Vector2 positionToMoveTo = MyTarget.Center + direction;
        positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
        EasingFunction.InOutSine(ratio));

        if (AttackCounter == 0)

        {
            positionToMoveTo.X -= MathHelper.Lerp(0f, -128 * MathF.Sign(direction.X),
  EasingFunction.InOutSine(ratio));
        }

        StartDashPosition = positionToMoveTo;

        EndDashPosition = MyTarget.Center;
        Vector2 dir = EndDashPosition - StartDashPosition;
        dir = dir.SafeNormalize(Vector2.Zero);
        EndDashPosition += -dir * 64;
        return positionToMoveTo;
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
                        int side = Main.rand.NextBool(2) ? -1 : 1;
                        Vector2 direction = Vector2.UnitX * side;
                        direction *= 128;
                        Vector2 pos = MyTarget.Center + direction;
                        TeleportOutEffect(NPC.Center);
                        TeleportEffect(pos);
                        Teleport(pos);
                        if (MultiplayerHelper.IsHost)
                        {
                            _fakeOut = Main.rand.NextBool(4);
                            NPC.netUpdate = true;
                        }
                    }
                    float time = Wind_Up_Punch_Teleport_In_Time;
                    if (AttackCounter == 0)
                        time *= 2;
                    float t = Timer / time;
                    Vector2 positionToMoveTo = SetPunchPositions(t);
                    FaceTarget();
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutExpo(Timer / time));
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);

                    _afterImages = true;
                    if (Timer == 15 && !_fakeOut)
                    {
                        var zoomPunch = AssetReferences.Assets.Sounds.STARR.Starrreadypunchfast.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(zoomPunch, NPC.position);
                    } else if (Timer == 15 && _fakeOut)
                    {
                        var zoomPunch = AssetReferences.Assets.Sounds.STARR.Starrreadypunch.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(zoomPunch, NPC.position);
                    }
                    if (Timer >= time)
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
                    if (_fakeOut)
                        time *= 2;

                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutExpo(ratio);
                    Vector2 dir = StartDashPosition - EndDashPosition;
                    dir = dir.SafeNormalize(Vector2.Zero);
                    Vector2 midPoint = StartDashPosition + dir * 256;

                    if (_fakeOut)
                    {
                        Vector2 lerp1 = Vector2.Lerp(StartDashPosition, midPoint, EasingFunction.OutExpo(ratio));
                        Vector2 lerp2 = Vector2.Lerp(midPoint, EndDashPosition, EasingFunction.InExpo(ratio * 0.98f));
                        Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                        lerp3.Y -= MathHelper.Lerp(0, 80, EasingFunction.OutSine(ratio));
                        Vector2 vel = lerp3 - NPC.Center;
                        NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(Timer / 45f));
                        NPC.spriteDirection = vel.X > 0 ? 1 : -1;
                    }
                    else
                    {
                        _starRot = MathHelper.Lerp(0, MathHelper.TwoPi * 2, Timer / time);
                        _bigStarAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutExpo(Timer / 24));
                        Vector2 lerp1 = Vector2.Lerp(StartDashPosition, EndDashPosition, EasingFunction.InExpo(ratio));
                        Vector2 vel = lerp1 - NPC.Center;
                        NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(Timer / 45f));
                        NPC.spriteDirection = vel.X > 0 ? 1 : -1;
                    }

                    if(Timer >= time * 0.5f)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    }
                    else
                    {

                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
                    }
   

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

                    if(Timer == 1)
                    {
                 
                        _timer2 = 0;
                        _initialVelocity = NPC.velocity;
                        StartDashPosition = NPC.Center;
                        Vector2 direction = NPC.velocity.X > 0 ? -Vector2.UnitX : Vector2.UnitX;
                        EndDashPosition = StartDashPosition + direction * 352;
                        
                    }

                    _timer2 += MathHelper.Lerp(1f, 0.25f, Timer / 45f);
                    float time = Wind_Up_Punch_End_Time;
                    float ratio = _timer2 / time;
                    Vector2 pos = Vector2.Lerp(StartDashPosition, EndDashPosition, ratio);
                    pos.Y += MathHelper.Lerp(0, -196, EasingFunction.OutSine(ratio));
                    Vector2 vel = pos - NPC.Center;
                    NPC.velocity = vel;


                    if(Timer >= time * 0.15f)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH_BACKAIR, AnimationParams.NoLooping);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    }

                    if (Timer == 1)
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


                    if (Timer >= Wind_Up_Punch_End_Time)
                    {
                        Timer = 0;
                        AttackCycle++;

                    }
                }
                break;
            case 3:
                {
                    _afterImages = true;
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                    }
                     
                    NPC.velocity *= 0.96f;
                    this.AseAnimator.PlayAnimation(ANIM_DISC_THROW, AnimationParams.NoLooping);
                    if (Timer == 27)
                    {
                        StartDashPosition = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero) * 8;
                        NPC.velocity += StartDashPosition;
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 throwDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            ProjFirer discFirer = ProjFirer.From<STARDISC>(NPC);
                            discFirer.damage = Disc_Throw_Damage;
                            discFirer.velocity = throwDirection * 4;
                            discFirer.knockback = 1;
                            discFirer.New();
                        }
                    }
                    if(Timer >= 42)
                    {
                        Timer = 0;
                        AttackCycle=0;
                    }
                }
                break;
            case 4:
                {
                    _afterImages = true;
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                    }

                    NPC.velocity *= 0.96f;
                    this.AseAnimator.PlayAnimation(ANIM_DISC_THROW, AnimationParams.NoLooping);
                    if (Timer == 27)
                    {
                        StartDashPosition = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero) * 8;
                        NPC.velocity += StartDashPosition;
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 throwDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            for(float f = 0; f < Disc_Throw_Count; f++)
                            {
                                float ratio = f / Disc_Throw_Count;
                                float radians = MathHelper.Lerp(-0.5f, 0.5f, ratio);
                                ProjFirer discFirer = ProjFirer.From<STARDISC>(NPC);
                                discFirer.damage = Disc_Throw_Damage;
                                discFirer.velocity = throwDirection * 4;
                                discFirer.velocity = discFirer.velocity.RotatedBy(radians);
                                discFirer.knockback = 1;
                                discFirer.New();
                            }

                        }
                    }
                    if (Timer >= 42)
                    {
                        Timer = 0;
                        AttackCycle = 0;
                    }
                }
                break;
        }
    }
}
