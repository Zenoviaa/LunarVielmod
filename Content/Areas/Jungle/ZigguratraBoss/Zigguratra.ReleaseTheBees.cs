using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using Terraria;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    float ReleaseTheBees_StartupTime => 30;
    float ReleaseTheBees_WaveCount => 7;
    float ReleaseTheBees_TimeBetweenWaves => 80;
    float ReleaseTheBees_OutTime => 80;
    int ReleaseTheBees_LineLength => 15;
    float ReleaseTheBees_LineStartOffset => 384;
    float ReleaseTheBees_Density => 333;
    private void AI_ReleaseTheBees()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
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
            case 1:
                {
                    if(Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        //Pick a random side,
                        //Create a line of 7 points
                        //Then pick a random point on that line and move in a straight line disabling them
                        var line = new bool[ReleaseTheBees_LineLength];
                        for(var i=  0; i < line.Length; i++)
                        {
                            line[i] = true;
                        }

                        var randIndex = Main.rand.Next(0, ReleaseTheBees_LineLength / 2);
                        var emptyLength = 4;
                        for(var i = 0; i < emptyLength; i++)
                        {
                            var newIndex = randIndex + i;
                            line[newIndex] = false;
                        }


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
                        for(var i = 0; i < line.Length; i++)
                        {
                            if (!line[i])
                                continue;

           
                            var ratio = (float)i / (float)line.Length;
                            var topPos = centerPos + perpOffset * ReleaseTheBees_Density;
                            var bottomPos = centerPos - perpOffset * ReleaseTheBees_Density;
                            var spawnPos = Vector2.Lerp(topPos, bottomPos, ratio);
                            var attackPos = spawnPos + -offset * 2;
                            
                            var firer = ProjFirer.From<LittleBee>(NPC);
                            firer.damage = Damage_ReleaseTheBees;
                            firer.velocity = -Vector2.UnitY * 8;
                            firer.ai2 = 0;
                            firer.knockback = 1;
                            
                            var littleBee = firer.NewDirect<LittleBee>();
                            littleBee.attackStartPosition = spawnPos;
                            littleBee.attackEndPosition = attackPos;
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
            case 2:
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
