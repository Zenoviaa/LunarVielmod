using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    bool _rudelySwung;
    ref Vector2 JumpStartPosition => ref _vector21;
    ref Vector2 JumpEndPosition => ref _vector22;
    float RudeBuster_JumpHeight => 384;
    float RudeBuster_JumpPrepTime => 30;
    float RudeBuster_JumpTime => 73;
    float RudeBuster_SlideTime => 26;
    float RudeBuster_SwingCount => 5;
    float RudeBuster_EndTime => 35;
    private void AI_RudeBuster()
    {
        FocusOnMe();

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
                        SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
                        bellHit.PitchVariance = 0.2f;
                        SoundEngine.PlaySound(bellHit, NPC.position);

                        float xDirection = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
                        MakeGoldenDonut(NPC.Bottom, Vector2.UnitY);

                        _rudelySwung = false;
                        JumpStartPosition = NPC.Center;
                        JumpEndPosition = MyTarget.Center;
                        JumpEndPosition.Y = JumpStartPosition.Y;
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

                    
                    if(ratio < 0.35f)
                    {
                        //Jump towards player
                        //I thinks for this we'll just make two points and do easing so it's clean
                        this.AseAnimator.PlayAnimation(ANIM_JUMP, AnimationParams.Default);
                    }
                    else
                    {
                        this.AseAnimator.PlayAnimation(ANIM_SPINBUSTER, AnimationParams.NoLooping);
                        if(!_rudelySwung && ratio >= 0.65f)
                        {
                            var firer = ProjFirer.From<RudeLightningBuster>(NPC);
                            firer.damage = Damage_RudeLightning;
                            firer.knockback = 1;
                            firer.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 4;
                            firer.New();
                            _rudelySwung = true;
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
                    MakeJumpingParticles(NPC.Bottom);


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
