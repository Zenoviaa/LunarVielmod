using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;


file struct BeeAttack
{
    public Vector2 spawnPosition;
    public Vector2 attackPosition;
}
public partial class Zigguratra
{
    float ReleaseTheBees_StartupTime => 30;
    float ReleaseTheBees_WaveCount => 5;
    float ReleaseTheBees_TimeBetweenWaves => 41;
    float ReleaseTheBees_OutTime => 80;
    int ReleaseTheBees_LineLength => 15;
    float ReleaseTheBees_LineStartOffset => 384;
    float ReleaseTheBees_Density => 333;
    float ReleaseTheBees_FlapTime => 60;
    private void AI_ReleaseTheBees()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    NPC.StayGroundedAndRooted();
                    NPC.SpriteFaceTarget();
                    this.AseAnimator.PlayAnimation(ANIM_FLAPWINGSBEFORE, AnimationParams.Default);
                    if(Timer % 30 == 0)
                    {
                        var sound = AssetReferences.Assets.Sounds.BeeBuzz.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(sound, NPC.position);
                    }
                    if(Timer >= ReleaseTheBees_FlapTime)
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
                        NPC.TargetClosest();
                    }
                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_RELEASE_THE_BEES, AnimationParams.NoLooping);
                    if (Timer >= ReleaseTheBees_StartupTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    if(Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        //Pick a random side,
                        //Create a line of 7 points
                        //Then pick a random point on that line and move in a straight line disabling them

                        var bees = new BeeAttack[ReleaseTheBees_LineLength];
  
                        var side = Main.rand.Next(4);
                        var offset = Vector2.UnitX;
                        switch (side)
                        {
                            default:
                            case 0:
                                offset = Vector2.UnitX;
                                break;
                            case 1:
                                offset = -Vector2.UnitX;
                                break;
                            case 2:
                                offset = -Vector2.UnitY;
                                break;
                            case 3:
                                offset = Vector2.UnitY;
                                break;
                        }
                        var perpOffset = offset.RotatedBy(MathHelper.PiOver2);
                        offset *= ReleaseTheBees_LineStartOffset;


                        var centerPos = MyTarget.Center + offset;
                        for(var i = 0; i < ReleaseTheBees_LineLength; i++)
                        {
                            ref var bee = ref bees[i];

                            var ratio = (float)i / (float)ReleaseTheBees_LineLength;
                            var topPos = centerPos + perpOffset * ReleaseTheBees_Density;
                            var bottomPos = centerPos - perpOffset * ReleaseTheBees_Density;
                            bee.spawnPosition = Vector2.Lerp(topPos, bottomPos, ratio);
                            bee.attackPosition = bee.spawnPosition + -offset * 2;
                        }

                        ShockCircles.CreateQuickWhiteFlash(NPC.Center);
                        Array.Sort(bees, (x, y) => Vector2.Distance(y.spawnPosition, MyTarget.Center).CompareTo(Vector2.Distance(x.spawnPosition, MyTarget.Center)));
                        var ignoreStart = ReleaseTheBees_LineLength / 2;
                        ignoreStart += Main.rand.Next(-1, 1);
                        var ignoreEnd = ignoreStart + 5;
                        for(var i = 0; i < ReleaseTheBees_LineLength; i++)
                        {
                            if (i >= ignoreStart && i <= ignoreEnd)
                                continue;

                            ref var bee = ref bees[i];
                            var firer = ProjFirer.From<LittleBee>(NPC);
                            firer.damage = Damage_ReleaseTheBees;
                            firer.velocity = -Vector2.UnitY * 21;
           
                            firer.ai2 = 0;
                            firer.knockback = 1;

                 
                            var littleBee = firer.NewDirect<LittleBee>();
                            littleBee.attackStartPosition = bee.spawnPosition;
                            littleBee.attackEndPosition = bee.attackPosition;
                            littleBee.attackStartupMidPosition = MyTarget.Center;
                        }
                     
                    }

                    _outliner.warning = true;
                    this.AseAnimator.PlayAnimation(ANIM_RELEASE_THE_BEES_HOLD, AnimationParams.Default);
                    if (Timer >= ReleaseTheBees_TimeBetweenWaves)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= ReleaseTheBees_WaveCount)
                        {
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 3:
                {
                    this.AseAnimator.PlayAnimation(ANIM_RELEASE_THE_BEES_OUT, AnimationParams.NoLooping);
                    if (Timer >= ReleaseTheBees_OutTime)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
}
