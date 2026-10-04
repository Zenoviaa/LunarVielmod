using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;
using Stellamod.Core.Camera;
using Terraria;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra
{
    float SplittingLightning_ChargeTime => 25;
    float SplittingLightning_AwayTime => 60;
    float SplittingLightning_FallingCrashTime => 45;
    float SplittingLightning_AxeChargeCount => 3;
    float SplittingLightning_CrashHoldTime => 53;
    float SplittingLightning_CrashCount => 1;
    void AI_SplittingLightningCrash()
    {

        Timer++;
        FocusOnMe();
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_AXE_CHARGE_DASH_OUT, AnimationParams.NoLooping);
                        var velOffset = MyTarget.velocity * 24;
                        if (velOffset.Y > 0)
                            velOffset.Y = 0;
                        Teleport(MyTarget.Top + new Vector2(0, -24) + velOffset);
                    }
                    _targetAfterIamgeAlpha = 1f;
                    if (Timer % 10 == 0)
                    {
                        var pos = NPC.Top;
                        pos += Main.rand.NextVector2Circular(32, 32);

                        var vel = -Vector2.UnitY;
                        vel *= 6;
                        vel = vel.RotatedBy(Main.rand.NextFloat(0, 6.28f));

                        var color = Color.Orange;
                        Particles.LightningSpikeySpark.Spawn(new()
                        {
                            position = pos,
                            velocity = vel,
                            scale = Main.rand.NextFloat(0.4f, 0.8f),
                            color = color,
                            timeLeft = 100
                        });
                    }
                    _outliner.attacking = true;
                    NPC.StayGroundedAndRooted();
                    this.AseAnimator.PlayAnimation(ANIM_FALLING_AXE_CRASH, AnimationParams.NoLooping);
                    if (Timer >= SplittingLightning_FallingCrashTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {

                    this.AseAnimator.PlayAnimation(ANIM_FALLING_AXE_CRASH, AnimationParams.NoLooping);
                    if (Timer == 1)
                    {
                        var firer = ProjFirer.From<AxeLightningCrash>(NPC);
                        firer.ai1 = 1;
                        firer.position = NPC.Bottom;
                        firer.damage = Damage_SplittingLightningCrash;
                        firer.knockback = 1;
                        firer.New();
                    }
                    _invisibleAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / SplittingLightning_CrashHoldTime));
                //    NPC.velocity.Y = MathHelper.Lerp(-4, -8, EasingFunction.InExpo(Timer / SplittingLightning_CrashHoldTime));
                    if (Timer >= SplittingLightning_CrashHoldTime )
                    {
                        AttackCounter++;
                        if(AttackCounter >= SplittingLightning_CrashCount)
                        {
                            SwitchState(AIState.Idle);
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle--;
                        }
       
                    }
                }
                break;
        }
    }

    private void AI_SplittingLightning()
    {
        Timer++;
        FocusOnMe();
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    _outliner.warning = true;
                    NPC.SpriteFaceTarget();
                    NPC.StayGroundedAndRooted();
                    string animation = string.Empty;
                    switch (AttackCounter)
                    {
                        default:
                        case 0:
                            animation = ANIM_AXE_CHARGE_1;
                            break;
                        case 1:
                            animation = ANIM_AXE_CHARGE_2;
                            break;
                        case 2:
                            animation = ANIM_AXE_CHARGE_3;
                            break;
                    }
                    var pos = NPC.Top;
                    switch (AttackCounter)
                    {
                        default:
                        case 0:
                            pos = AxeFirstSplittingPosition;
                            break;
                        case 1:
                            pos = AxeSecondSplittingPosition;
                            break;
                        case 2:
                            pos = AxeThirdSplittingPosition;
                            break;
                    }
                    if (Timer == 1)
                    {
                        MakeLightningParticle(pos);
                    }

                    var axePosition = pos;
                    axePosition += Main.rand.NextVector2Circular(32, 32);
                    _axeLightningAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(AttackCounter / SplittingLightning_AxeChargeCount));
                    AmbientThundercloudParticles(axePosition);
                    AmbientElectricParticles(axePosition);
                    AmbientLightningParticles(axePosition);
                    SwirlParticlesAround(axePosition);

                    if (Timer >= SplittingLightning_ChargeTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= SplittingLightning_AxeChargeCount)
                        {
                            AttackCycle++;
                        }
                    }
                    this.AseAnimator.PlayAnimation(animation, AnimationParams.NoLooping);
                }
                break;
            case 1:
                {
                    this.AseAnimator.PlayAnimation(ANIM_AXE_CHARGE_DASH_OUT, AnimationParams.NoLooping);
                    if(Timer == 30)
                    {
                        Teleport(MyTarget.Top + new Vector2(0, -24));
                    }
                    if (Timer >= SplittingLightning_AwayTime)
                    {
                        SwitchState(AIState.Splitting_Lightning_Crash);
                    }
                }
                break;
        }
    }
}
