using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using Stellamod.Content.Dusts;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    int AxeCrash_Damage => 32;
    float AxeCrash_ChargeCount => 4;
    float AxeCrash_WalkingSpeed => 1.6f;
    float AxeCrash_WalkingTimePerCharge => 100;
    float AxeCrash_ReadyTime => 30;
    float AxeCrash_SummonTime => 80;
    float AxeCrash_HoldTime => 80;
    float AxeCrash_SlamTime => 71;

    float AxeCrash_FastSlamTime => 37;
    float AxeCrash_ChargeLevel => (AttackCounter + 1) / AxeCrash_ChargeCount;

    //Slowly walks up to you with the axe behind his back
    //every few steps he readies it more and it charges with lightning,
    //then he does a great overswing(like susie)
    //and it crashes into the ground
    //creating a large lightning shockwave and rupture,
    //this can be jumped or rolled as it has a grounded hit box
    //but must be well timed to dodge either way
    private void AI_AxeCrash()
    {
        Timer++;
        var slamAnimation = _axeLightningDashed ? ANIM_SUPER_AXE_CRASH : ANIM_AXE_SLAM;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        _axeLightningDashed = false;
                        GruntSound();
                        NPC.TargetClosest();
                    }

                    //He'll sit here and look at you for a second, 
                    //Then he'll ready his axe behind him before he starts walking
                    //So basicaly
                    this.AseAnimator.PlayAnimation(ANIM_AXE_SUMMON, AnimationParams.NoLooping);
                    NPC.StayGroundedAndRooted();
                    NPC.SpriteFaceTarget();
                    FocusOnMe();
                    SwirlParticlesAround(NPC.Center);
                    if (Timer >= AxeCrash_SummonTime)
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
                        GruntSound();
                        NPC.TargetClosest();
                    }

                    //He'll sit here and look at you for a second, 
                    //Then he'll ready his axe behind him before he starts walking
                    //So basicaly
                    this.AseAnimator.PlayAnimation(ANIM_AXE_READY, AnimationParams.NoLooping);
                    NPC.StayGroundedAndRooted();
                    NPC.SpriteFaceTarget();
                    FocusOnMe();
                    SwirlParticlesAround(NPC.Center);
                    if (Timer >= AxeCrash_ReadyTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    if (Timer == 1)
                    {
                        GruntSound();
                    }


                    if(Timer == 14)
                    {
                        MakeLightningParticle(AxeBackPosition);
                    }

                    //Here he walks towards the player
                    //AI move towards player lmao
                    var xDirection = NPC.XDirectionToTarget;
                    var walkVelocity = xDirection * AxeCrash_WalkingSpeed;
                    var xDistToTarget = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                    if (xDistToTarget < 154)
                    {
                        NPC.velocity.X *= 0.9f;

                    }
                    else
                    {
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, walkVelocity, 0.1f);

                    }
                    this.AseAnimator.PlayAnimation(ANIM_AXE_WALK, AnimationParams.Default);
                    NPC.SpriteFaceTarget();
                    FocusOnMe();

                    var axePosition = AxeBackPosition;
                    axePosition += Main.rand.NextVector2Circular(32, 32);
                    _axeLightningAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / AxeCrash_WalkingTimePerCharge));
                    AmbientThundercloudParticles(axePosition);
                    AmbientElectricParticles(axePosition);
                    AmbientLightningParticles(axePosition);
                    SwirlParticlesAround(axePosition);

                    if (AttackCounter >= AxeCrash_ChargeCount - 2)
                    {
                        _outliner.warning = true;
                    }

                    if (Timer >= AxeCrash_WalkingTimePerCharge)
                    {
                        PixelPrimitiveCircleFactory.CreateInElectricSuck(AxeBackPosition);
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= AxeCrash_ChargeCount)
                        {
                            if (xDistToTarget > 200)
                            {
                                _axeLightningDashed = true;
                                AttackCycle = 3;
                            }
                            else
                            {
                                AttackCycle = 4;
                            }
                     
                        }
                    }
                }
                break;

            case 3:
                {
                    this.AseAnimator.PlayAnimation(ANIM_AXE_CHARGE_DASH_OUT, AnimationParams.NoLooping);
                    NPC.SpriteFaceTarget();
                    FocusOnMe();
                    if(Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 4:
                {
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        if (_axeLightningDashed)
                        {
                            var side = -NPC.XDirectionToTarget;
                            var pos = MyTarget.Bottom;
                            pos.X += side * 154;
                            pos.Y -= NPC.height / 2;
                            Teleport(pos);
                        }
                        GruntSound();
                    }
                    if (!_axeLightningDashed)
                    {
                        if (Timer > 24)
                        {
                            var axePosition = AxeAboveHeadPosition;
                            axePosition += Main.rand.NextVector2Circular(32, 32);

                            if (Timer % 14 == 0)
                            {
                                MakeLightningParticle(axePosition);
                            }
                            AmbientThundercloudParticles(axePosition);
                            AmbientElectricParticles(axePosition);
                            AmbientLightningParticles(axePosition);

                        }

                    }
                    else
                    {
                        if (Timer > 12)
                        {
                            var axePosition = AxeAboveHeadPosition;
                            axePosition += Main.rand.NextVector2Circular(32, 32);
                            if (Timer % 14 == 0)
                            {
                                MakeLightningParticle(axePosition);
                            }
                            AmbientThundercloudParticles(axePosition);
                            AmbientElectricParticles(axePosition);
                            AmbientLightningParticles(axePosition);
                        }
                    }
          
                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(slamAnimation, AnimationParams.NoLooping);
                    FocusOnMe();
                    var time = _axeLightningDashed ? AxeCrash_FastSlamTime : AxeCrash_SlamTime;
                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 5:
                {
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        NPC.velocity.X += NPC.direction * 4;
                    }
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<AxeLightningCrash>(NPC);
                        firer.position = NPC.Bottom;
                        firer.position.X += NPC.direction * 196;
                        firer.damage = AxeCrash_Damage;
                        firer.knockback = 1;
                        firer.New();
                    }

                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(slamAnimation, AnimationParams.NoLooping);
                    if (Timer >= AxeCrash_HoldTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    SwitchState(AIState.Idle);
                }
                break;

        }
    }
}

