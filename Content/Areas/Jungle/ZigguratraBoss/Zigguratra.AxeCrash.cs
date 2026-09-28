using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Stellamod.Core.AssetReferences.Assets.NoiseTextures;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    int AxeCrash_Damage => 32;
    float AxeCrash_ChargeCount => 3;
    float AxeCrash_WalkingSpeed => 1.6f;
    float AxeCrash_WalkingTimePerCharge => 100;
    float AxeCrash_ReadyTime => 30;
    float AxeCrash_SummonTime => 80;
    float AxeCrash_HoldTime => 80;
    float AxeCrash_SlamTime => 71;

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
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
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
                    if(Timer == 1)
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
                    if(Timer >= AxeCrash_ReadyTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    if(Timer == 1)
                    {
                        GruntSound();
                    }

           
                    //Here he walks towards the player
                    //AI move towards player lmao
                    var xDirection = NPC.XDirectionToTarget;
                    var walkVelocity = xDirection * AxeCrash_WalkingSpeed;
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, walkVelocity, 0.1f);
                    NPC.SpriteFaceTarget();
                    FocusOnMe();
                    this.AseAnimator.PlayAnimation(ANIM_AXE_WALK, AnimationParams.Default);
                    var axePosition = AxeBackPosition;
                    axePosition += Main.rand.NextVector2Circular(32, 32);
                    _axeLightningAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / AxeCrash_WalkingTimePerCharge));
                    AmbientThundercloudParticles(axePosition);
                    AmbientElectricParticles(axePosition);
                    AmbientLightningParticles(axePosition);
                    SwirlParticlesAround(axePosition);

                    var xDistToTarget = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                   if(Timer >= AxeCrash_WalkingTimePerCharge || xDistToTarget < 154)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter >= AxeCrash_ChargeCount)
                        {
                            AttackCycle++;
                        }
                    }
                }
                break;

            case 3:
                {
                    _outliner.warning = true;
                    if (Timer == 1)
                    {
                        GruntSound();
                    }

                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(ANIM_AXE_SLAM, AnimationParams.NoLooping);
                    FocusOnMe();
                    if (Timer >= AxeCrash_SlamTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 4:
                {
                    _outliner.attacking = true;
                    if(Timer == 1)
                    {
                        NPC.velocity.X += NPC.direction * 4;
                    }
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<AxeLightningCrash>(NPC);
                        firer.position = NPC.Bottom;
                        firer.position.X += NPC.direction * 32;
                        firer.damage = AxeCrash_Damage;
                        firer.knockback = 1;
                        firer.New();
                    }

                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(ANIM_AXE_SLAM, AnimationParams.NoLooping);
                    if(Timer >= AxeCrash_HoldTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    SwitchState(AIState.Idle);
                }
                break;

        }
    }
}
