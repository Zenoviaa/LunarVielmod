using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    private ref Vector2 DirectionToDash => ref _vector1;
    private float Wind_Up_Punch_Prep_Time => 50;
    private float Wind_Up_Punch_Time => 30;
    private float Wind_Up_Punch_End_Time => 15;
    private int Wind_Up_Punch_Damage => 35;
    private void AI_WindUpPunch()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        GruntSound();
                    }
                    _outliner.warning = true;
                    DirectionToDash = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    FaceTarget();
                    StayGrounded();
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH_READY, AnimationParams.NoLooping);
                    if (Timer >= Wind_Up_Punch_Prep_Time * 0.75f && !_eyeFlash)
                    {
                        MakeEyeFlashParticle();
                        _eyeFlash = true;
                    }
                    if (Timer >= Wind_Up_Punch_Prep_Time)
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
                        BigGruntSound();
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<STARROCKDASH>(NPC);
                            firer.damage = Wind_Up_Punch_Damage;
                            firer.knockback = 1;
                            firer.New();
                        }
                    }
       

                    _jumpingTrail = true;
                    _outliner.attacking = true;
                    Vector2 dashVelocity = DirectionToDash;
                    dashVelocity *= 20;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, dashVelocity, 0.2f);
                    this.AseAnimator.PlayAnimation(ANIM_PUNCH, AnimationParams.NoLooping);
                    if (Timer >= Wind_Up_Punch_Time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    if (_fakeOut && Timer >= Wind_Up_Punch_Time * 0.35f)
                    {
                        SwitchState(AIState.DiscThrow);
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.92f;
                    if(Timer >= Wind_Up_Punch_End_Time)
                    {
                        SwitchState(AIState.CapeOut);
                    }
                }
                break;
        }
    }
}
