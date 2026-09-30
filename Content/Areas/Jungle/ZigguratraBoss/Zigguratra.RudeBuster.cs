using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    bool _rudelySwung;
    ref Vector2 JumpStartPosition => ref _vector21;
    ref Vector2 JumpEndPosition => ref _vector22;
    float RudeBuster_JumpHeight => 252;
    float RudeBuster_JumpPrepTime => 30;
    float RudeBuster_JumpTime => 60;
    float RudeBuster_SlideTime => 45;
    float RudeBuster_SwingCount => 5;
    float RudeBuster_EndTime => 35;
    private void AI_RudeBuster()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    if(Timer == 1)
                    {
                        _rudelySwung = false;
                        NPC.TargetClosest();
                    }

                    this.AseAnimator.PlayAnimation(ANIM_JUMPSTART, AnimationParams.NoLooping);
                    NPC.SpriteFaceTarget();
                    NPC.StayGroundedAndRooted();
                    if (Timer >= RudeBuster_JumpPrepTime)
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
                        JumpStartPosition = NPC.Center;
                        JumpEndPosition = JumpStartPosition.FlipX(MyTarget.Center);
                    }


                    _outliner.attacking = true;
                    var upward = -RudeBuster_JumpHeight;
                    var time = RudeBuster_JumpTime;
                    var ratio = Timer / time;
                    var outEase = EasingFunction.OutExpo(ratio);
                    var inEase = EasingFunction.InExpo(ratio);

                    var yHeight = MathHelper.Lerp(0, upward, outEase);
                    var downHeight = MathHelper.Lerp(upward, 0, inEase);
                    var height = MathHelper.Lerp(yHeight, downHeight, ratio);

                    var pos = Vector2.Lerp(JumpStartPosition, JumpEndPosition, ratio);
                    pos.Y += height;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pos;

                    
                    if(ratio < 0.45f)
                    {
                        //Jump towards player
                        //I thinks for this we'll just make two points and do easing so it's clean
                        this.AseAnimator.PlayAnimation(ANIM_JUMP, AnimationParams.Default);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_SPINBUSTER, AnimationParams.NoLooping);
                        if(!_rudelySwung && ratio >= 0.55f)
                        {
                            var firer = ProjFirer.From<RudeLightningBuster>(NPC);
                            firer.damage = Damage_RudeLightning;
                            firer.knockback = 1;
                            firer.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                            firer.New();
                        }
                    }

                    if(ratio >= 1f)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCounter++;
                    }

                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        NPC.velocity = Vector2.UnitX * NPC.spriteDirection * 15;
                    }
                    this.AseAnimator.PlayAnimation(ANIM_JUMPSTART, AnimationParams.NoLooping);
                    NPC.SpriteFaceTarget();
                    NPC.StayGroundedAndRooted();
                    if(Timer >= RudeBuster_SlideTime)
                    {
                        Timer = 0;
                        if(AttackCounter >= RudeBuster_SwingCount)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle--;
                        }
                    }
                }
                break;
            case 3:
                {
                    NPC.SpriteFaceTarget();
                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(ANIM_IDLE);
                    if(Timer >= RudeBuster_EndTime)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }

    }
}
